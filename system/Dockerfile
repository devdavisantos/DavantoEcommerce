# Build stage
FROM node:20-alpine AS builder

WORKDIR /app

# RUN npm install -g pnpm
# RUN npm install -g pnpm@12.3.4
RUN npm install -g pnpm@10.17.1

COPY package.json pnpm-lock.yaml ./

RUN pnpm install --frozen-lockfile

COPY . .

RUN pnpm build

# Runtime stage
FROM node:20-alpine

WORKDIR /app

ENV NODE_ENV=production

COPY --from=builder /app/.next/standalone ./
COPY --from=builder /app/.next/static ./.next/static
COPY --from=builder /app/public ./public

EXPOSE 3000

CMD ["node", "server.js"]