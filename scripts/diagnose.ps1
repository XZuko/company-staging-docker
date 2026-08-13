$ErrorActionPreference = 'Continue'
docker compose ps
docker compose logs --tail 200 database api frontend gateway
docker compose exec api wget -qO- http://localhost:8080/health

