FROM harbor.ad.glshk.com/baseimgproxy/node:18.12.1-alpine

WORKDIR /app

COPY ./ .

RUN mv package.json package.json_backup \
    && cd packages/web-service \
    && yarn install

WORKDIR /app/packages/web-service

CMD ["yarn", "start"]