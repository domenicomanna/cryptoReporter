# This docker file should be used with the Docker BuildKit enabled as that allows files to easily be copied from within
# the built image to the host file system. 

# Example usage: DOCKER_BUILDKIT=1 docker build --output ./build .
FROM node:20-alpine AS build-stage
WORKDIR /app
COPY . ./
RUN yarn install --immutable
RUN yarn build

FROM scratch AS export-stage
COPY --from=build-stage /app/build .