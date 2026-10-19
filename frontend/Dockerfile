# Stage 1: build the static files with Node.
FROM node:24-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
# Vite writes the API address into the built files, so it is set at build time.
ARG VITE_API_URL=http://localhost:5208
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build

# Stage 2: serve the built files with nginx.
FROM nginx:1.27-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
