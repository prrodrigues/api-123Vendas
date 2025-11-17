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

#aplicação 
http://localhost:8080/swagger/index.html


#RabbitMq
http://localhost:15672/


Branches principais:
- main: código em produção (estável).
- develop: integração de features, base para desenvolvimento.
- homolog: após a develop aprovada, sobe para homologação.

Branches auxiliares:
- feature/*: novas funcionalidades (ex: feature/CARD-0001-dev).
- fix/*: correções de bugs (ex: fix/CARDBug-0001).
- chore/*: tarefas técnicas e débitos (ex: chore/CARDDebitoTecnico-0001).
- release/*: preparação de versões (ex: release/1.0.0).
- hotfix/*: correções urgentes em produção (ex: hotfix/1.0.1-critical-bug).

Commits:
- Usar commits semânticos: feat, fix, chore, refactor, test, docs.
- Ex.: "feat: add RabbitMQ integration for order events [CARD-1234]"

Fluxo:
1. Criar feature branch a partir de develop.
2. Commits semânticos.
3. Abrir PR para develop.
4. Quando estável, criar release/* a partir de develop.
5. Testar, depois merge release -> main, tag vX.Y.Z, merge release -> develop.
6. Para hotfix, criar hotfix/* a partir de main, depois merge em main e develop.
