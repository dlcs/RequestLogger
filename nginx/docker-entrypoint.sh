#!/usr/bin/env sh
set -eu

envsubst '${EXTERNAL_PORT} ${NGINX_PROXY_PASS_LOCATION} ${NGINX_PROXY_PASS_MIRROR_LOCATION} ${SERVICE} ${NGINX_RESOLVER}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf

exec "$@"