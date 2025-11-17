using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Sales.Application.Common.Correlation;
using Sales.Application.Common.Messaging;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Sales.Infrastructure.Messaging;

public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly ICorrelationContextAccessor _correlationAccessor;

    public RabbitMqEventBus(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqEventBus> logger,
        ICorrelationContextAccessor correlationAccessor)
    {
        _options = options.Value;
        _logger = logger;
        _correlationAccessor = correlationAccessor;

        _connectionFactory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true
        };

        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true);

        _channel.QueueDeclare(
            queue: _options.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _channel.QueueBind(
            queue: _options.Queue,
            exchange: _options.Exchange,
            routingKey: "sales.*");

        _logger.LogInformation(
            "Connected to RabbitMQ exchange {Exchange} with queue {Queue} on {Host}:{Port}",
            _options.Exchange,
            _options.Queue,
            _options.HostName,
            _options.Port);
    }

    public Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default)
    {
        if (!EnsureConnection())
        {
            _logger.LogWarning(
                "Skipping publish of message {@Message} because RabbitMQ is unavailable at {Host}:{Port}",
                message,
                _options.HostName,
                _options.Port);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Publishing message {@Message} with routing key {RoutingKey}", message, routingKey);
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = _channel!.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2; // persistente
        props.MessageId = Guid.NewGuid().ToString();
        props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        var correlationId = _correlationAccessor.CorrelationId ?? Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.Headers ??= new Dictionary<string, object>();
        props.Headers["x-correlation-id"] = Encoding.UTF8.GetBytes(correlationId);

        _logger.LogInformation(
            "Publishing message {MessageId} to {Exchange} with routing {RoutingKey}",
            props.MessageId,
            _options.Exchange,
            routingKey);

        _channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: routingKey,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing RabbitMQ resources");
        _channel?.Dispose();
        _connection?.Dispose();
    }

    private bool EnsureConnection()
    {
        if (_channel?.IsOpen == true && _connection?.IsOpen == true)
        {
            return true;
        }

        try
        {
            _connection?.Dispose();
            _channel?.Dispose();

            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _options.Exchange,
                type: ExchangeType.Topic,
                durable: true);

            _channel.QueueDeclare(
                queue: _options.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(
                queue: _options.Queue,
                exchange: _options.Exchange,
                routingKey: "sales.*");

            _logger.LogInformation(
                "Connected to RabbitMQ exchange {Exchange} with queue {Queue} on {Host}:{Port}",
                _options.Exchange,
                _options.Queue,
                _options.HostName,
                _options.Port);

            return true;
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(
                ex,
                "Unable to connect to RabbitMQ at {Host}:{Port}. Events will be skipped until connectivity is restored.",
                _options.HostName,
                _options.Port);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error connecting to RabbitMQ at {Host}:{Port}",
                _options.HostName,
                _options.Port);
            return false;
        }
    }
}
