FROM node:20

WORKDIR /app

RUN npm install -g pnpm

COPY . .

RUN npm install --shamefully-hoist

COPY package.json pnpm-lock.yaml ./

RUN pnpm build

EXPOSE 3000

CMD ["pnpm", "start", "-p", "3000"]