This is a [Next.js](https://nextjs.org) project bootstrapped with [`create-next-app`](https://nextjs.org/docs/app/api-reference/cli/create-next-app).

## Getting Started

First, run the development server:

```bash
npm run dev
# or
yarn dev
# or
pnpm dev
# or
bun dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

You can start editing the page by modifying `app/page.tsx`. The page auto-updates as you edit the file.

This project uses [`next/font`](https://nextjs.org/docs/app/building-your-application/optimizing/fonts) to automatically optimize and load [Geist](https://vercel.com/font), a new font family for Vercel.

## Learn More

To learn more about Next.js, take a look at the following resources:

- [Next.js Documentation](https://nextjs.org/docs) - learn about Next.js features and API.
- [Learn Next.js](https://nextjs.org/learn) - an interactive Next.js tutorial.

You can check out [the Next.js GitHub repository](https://github.com/vercel/next.js) - your feedback and contributions are welcome!

## Deploy on Vercel

The easiest way to deploy your Next.js app is to use the [Vercel Platform](https://vercel.com/new?utm_medium=default-template&filter=next.js&utm_source=create-next-app&utm_campaign=create-next-app-readme) from the creators of Next.js.

Check out our [Next.js deployment documentation](https://nextjs.org/docs/app/building-your-application/deploying) for more details.

## Sessão do cliente

O navegador mantém os tokens em cookies HttpOnly persistentes. O access token
usa o caminho /api e o refresh token /api/auth. Em produção os cookies exigem
HTTPS (Secure). Não salve os tokens em localStorage.

Configure Jwt:AccessTokenExpirationMinutes e Jwt:RefreshTokenExpirationDays
na API (appsettings.json e appsettings.Development.json, ou variáveis
Jwt__AccessTokenExpirationMinutes e Jwt__RefreshTokenExpirationDays).
Em produção e desenvolvimento, o access token dura 15 minutos e a sessão dura 3 dias. Para 30 dias, configure
RefreshTokenExpirationDays = 30 e faça um novo login; sessões existentes mantêm
o prazo original.

O frontend restaura a sessão ao abrir a aplicação, renova perto do vencimento
e verifica novamente ao voltar à aba ou recuperar a conexão. Web Locks coordena
as abas em contextos seguros. Requisições de perfil e endereços usam
authenticatedFetch para verificar a sessão e tentar novamente uma vez após 401.
Falhas de rede não apagam os cookies; a verificação é repetida após 15 segundos.
O logout revoga o refresh token na API e apaga os três cookies quando confirmado.

A API guarda hashes de refresh tokens e faz rotação em transação, preservando
a expiração original. Datas do MySQL são restauradas como UTC. O access token
também fica limitado à data final da sessão.

Validação automatizada: execute node --test tests/auth-client.test.cjs na pasta
system. Para conferir a integração com a API e o MySQL: faça login, recarregue,
feche/reabra o navegador e use duas abas; deixe o access token vencer com a aba
fechada e volte antes do prazo do refresh token. Confira também logout, troca
de senha, perda/retorno da conexão e expiração final. Para um teste rápido,
reduza AccessTokenExpirationMinutes em desenvolvimento antes de um novo login.

Referências: https://developer.mozilla.org/en-US/docs/Web/API/Web_Locks_API
e https://developer.mozilla.org/en-US/docs/Web/API/Window/setTimeout


Teste de expiração: reinicie a API em Development e faça logout/login para
receber os novos prazos. Durante os 3 dias a rotação mantém a data final
original. Ao vencer, a aplicação encerra o estado autenticado, bloqueia as
páginas protegidas e mostra "Sua sessão expirou. Realize o login novamente
para continuar.", com um link para login. Uma aba suspensa verifica ao voltar.
O sessionStorage guarda apenas a data para preservar o aviso ao recarregar;
tokens continuam exclusivamente nos cookies HttpOnly.
Cada rotação invalida o token anterior. Registros antigos no banco não são
sessões válidas adicionais; logins em outros dispositivos podem ter sessões
independentes.

## Dispositivo e IP da sessão

No login, device recebe um JSON compacto com browser, os e type
(desktop/mobile/tablet/unknown), derivado do User-Agent. A classificação é
aproximada (por exemplo, tablets em modo desktop podem parecer computadores).
O JSON cabe no VARCHAR(255) existente; não é necessária migração.
ip_address guarda o IP observado no login (IPv4/IPv6). Os novos registros da
rotação herdam esses dados e a expiração original. Logins anteriores continuam
com os metadados antigos; faça novo login para preencher os campos.

Cada navegador/perfil mantém seus próprios cookies e sua própria sessão.
Device não é uma identificação única de hardware e não determina a sessão.
O IP pode ser compartilhado ou mudar; não é utilizado para bloquear o refresh.

O Next.js encaminha apenas o cabeçalho configurado em TRUSTED_CLIENT_IP_HEADER,
exigindo um único IP válido. Configure esse nome somente quando a entrada
confiável sobrescrever o cabeçalho e o Next.js não estiver exposto diretamente.
A API aceita esse IP apenas quando a conexão vem de LoginProxy:KnownProxies.
A lista é definida apenas no Compose, com um único proxy. A âncora YAML
frontend_ip reutiliza o mesmo endereço na API e no frontend, sem repetir o valor.
Não há proxies locais confiáveis por padrão no appsettings.Development.json.
Sem essa cadeia configurada, o login funciona mas o IP encaminhado fica nulo;
não se grava o IP do servidor Next.js como se fosse do cliente.

O docker-compose.yml principal inicia MySQL, API, Next.js e Nginx. Para usar:
docker compose up --build -d
Acesse http://localhost:3000. O Nginx sobrescreve X-Real-IP com o IP da conexão.
O frontend recebe endereço fixo na rede privada, confiado explicitamente pela
API. Ajuste a sub-rede e os endereços juntos se houver conflito local.
Não publique portas diretas do frontend ou da API ao usar essa confiança.

Em Docker Desktop/NAT, o IP observado pode ser o gateway do Docker.
Para acesso pelo celular via IP da LAN e na VPS, configure HTTPS na entrada:
os cookies do frontend de produção são Secure. Se adicionar CDN/load balancer,
configure o real IP no Nginx apenas para os proxies confiáveis, antes de usar
remote_addr. Este Compose é para teste (API em Development e MySQL exposto
pelo Compose principal); não é uma configuração final de produção.

Testes: node --test tests/auth-client.test.cjs tests/login-metadata.test.cjs
na pasta system; dotnet run --project api/Tests/LoginMetadata/LoginMetadata.csproj
na raiz do repositório.





## Configuração sem segredos

appsettings.json contém os parâmetros da API e valores padrão não sensíveis.
Jwt:SecretKey e ConnectionStrings:DefaultConnection ficam vazios propositalmente;
não são credenciais válidas nem placeholders que o sistema substitui sozinho.
Localmente, appsettings.Development.json (ignorado no Git) fornece os valores.
No Docker, copie .env.example para .env SOMENTE se ainda não tiver .env,
preencha os valores e execute docker compose up --build -d.

Variáveis usadas pelo Compose:
- MYSQL_DATABASE: nome do banco.
- MYSQL_ROOT_PASSWORD: senha do MySQL usado neste ambiente.
- JWT_SECRET_KEY: segredo aleatório com no mínimo 32 caracteres.
- ASPNETCORE_ENVIRONMENT: Development localmente; Production no servidor.

Para executar a API fora do Compose, configure diretamente:
- Jwt__SecretKey
- ConnectionStrings__DefaultConnection
- Jwt__Issuer (padrão LaStore.Api)
- Jwt__Audience (padrão LaStore)
- Jwt__AccessTokenExpirationMinutes (padrão 15)
- Jwt__RefreshTokenExpirationDays (padrão 3)
- LoginProxy__KnownProxies__0 (IP do Next.js autorizado a encaminhar o IP)
- AllowedHosts (domínios permitidos, separados por ponto e vírgula)

Sem proxies configurados, a API não confia em IP encaminhado.
O Compose já define a conexão MySQL e o proxy interno. Para o Next.js, ele define
API_BASE_URL e TRUSTED_CLIENT_IP_HEADER=x-real-ip; NODE_ENV=production vem da
imagem. Fora do Docker, use API_BASE_URL no ambiente do servidor Next.js e
configure TRUSTED_CLIENT_IP_HEADER apenas atrás de uma entrada confiável.
HTTPS é necessário para os cookies Secure em produção.
