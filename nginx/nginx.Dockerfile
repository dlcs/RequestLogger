FROM nginx

COPY ./nginx.default.conf /etc/nginx/conf.d/default.conf.template

COPY docker-entrypoint.sh /
RUN chmod +x entrypoint.sh
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]