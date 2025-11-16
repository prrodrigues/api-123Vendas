# Projeto 123Vendas

Este repositório contém os serviços do 123Vendas. Para executar localmente via Docker Compose, garanta os pré-requisitos abaixo.

## Pré-requisitos

- **Docker** instalado e em execução. Em ambientes Windows, o Docker Desktop deve estar iniciado e com a integração WSL habilitada.
- **Docker Compose v2** (disponível no Docker Desktop ou nos pacotes atuais do Docker Engine).

## Executando

1. Certifique-se de que o Docker Engine está ativo. No Windows, inicie o Docker Desktop antes de continuar.
2. Na raiz do repositório, execute:

   ```bash
   docker compose up --build
   ```

Se a execução falhar com erro semelhante a `open //./pipe/dockerDesktopLinuxEngine`, o Docker Desktop não está em execução ou a integração WSL não está habilitada. Inicie o Docker Desktop e tente novamente.


aplicação 
http://localhost:8080/swagger/index.html
http://localhost:15672/