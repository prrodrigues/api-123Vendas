using Microsoft.AspNetCore.Mvc;
using Sales.Api.Contracts;
using Sales.Application.Common.Correlation;
using Sales.Application.Sales;
using Sales.Application.Sales.Dtos;

namespace Sales.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;
    private readonly ISaleRepository _repository;
    private readonly ILogger<SalesController> _logger;
    private readonly ICorrelationContextAccessor _correlationAccessor;

    public SalesController(
        ISaleService saleService,
        ISaleRepository repository,
        ILogger<SalesController> logger,
        ICorrelationContextAccessor correlationAccessor)
    {
        _saleService = saleService;
        _repository = repository;
        _logger = logger;
        _correlationAccessor = correlationAccessor;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SaleResponse>>>> GetAll(CancellationToken ct)
    {
        var sales = await _repository.GetAllAsync(ct);

        var result = sales.Select(MapToResponse);
        return Ok(ApiResponse<IEnumerable<SaleResponse>>.FromSuccess(result, _correlationAccessor));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<SaleResponse>>> GetById(Guid id, CancellationToken ct)
    {
        var sale = await _repository.GetByIdAsync(id, ct);
        if (sale is null)
            return NotFound(ApiResponse<SaleResponse>.FromFailure("Venda não encontrada.", _correlationAccessor));

        return Ok(ApiResponse<SaleResponse>.FromSuccess(MapToResponse(sale), _correlationAccessor));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SaleResponse>>> Create([FromBody] CreateSaleRequest request, CancellationToken ct)
    {
        var sale = await _saleService.CreateSaleAsync(request, ct);

        // Exemplo simples de “evento” logado:
        _logger.LogInformation("CompraEfetuada: {SaleId}", sale.Id);

        var response = ApiResponse<SaleResponse>.FromSuccess(MapToResponse(sale), _correlationAccessor, "Venda criada com sucesso.");
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, response);
    }

    // PUT e DELETE/Cancel ficariam semelhantes, atualizando/Cancelando e logando
    // CompraAlterada / CompraCancelada / ItemCancelado

    private static SaleResponse MapToResponse(Domain.Sales.Sale sale)
    {
        return new SaleResponse(
            sale.Id,
            sale.Number,
            sale.Date,
            sale.CustomerId,
            sale.CustomerName,
            sale.BranchId,
            sale.BranchName,
            sale.Total,
            sale.Status.ToString(),
            sale.Items.Select(i => new SaleItemResponse(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.DiscountPercent,
                i.Total,
                i.IsCanceled
            )).ToList()
        );
    }
}
