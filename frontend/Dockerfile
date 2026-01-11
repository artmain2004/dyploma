FROM node:18-alpine AS build
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm install
COPY . .
ARG VITE_AUTH_URL
ARG VITE_CATALOG_URL
ARG VITE_ORDERS_URL
ENV VITE_AUTH_URL=$VITE_AUTH_URL
ENV VITE_CATALOG_URL=$VITE_CATALOG_URL
ENV VITE_ORDERS_URL=$VITE_ORDERS_URL
RUN npm run build

FROM node:18-alpine AS runner
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5173
CMD ["npm", "run", "preview", "--", "--host", "0.0.0.0", "--port", "5173"]
