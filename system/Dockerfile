FROM node:20

WORKDIR /app

RUN npm install -g pnpm
RUN npm install
RUN pnpm add next react react-dom
RUN pnpm add lucide-react
RUN pnpm add react-country-flag

COPY . .

COPY package.json pnpm-lock.yaml ./

RUN pnpm build

EXPOSE 3000

CMD ["pnpm", "start", "-p", "3000"]