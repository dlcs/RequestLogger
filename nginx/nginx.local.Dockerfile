FROM nginx
RUN rm /etc/nginx/nginx.conf /etc/nginx/conf.d/default.conf
COPY ./nginx.local.conf /etc/nginx/nginx.conf