FROM docker.io/dxflrs/garage:v2.3.0 AS garage-bin
FROM docker.io/library/alpine:3.21

COPY --from=garage-bin /garage /usr/local/bin/garage
RUN ln -s /usr/local/bin/garage /garage

RUN cat <<'EOF' > /entrypoint.sh && chmod +x /entrypoint.sh
#!/bin/sh
cat <<EOC > /etc/garage.toml
metadata_dir = "${GARAGE_META_DIR}"
data_dir = "${GARAGE_DATA_DIR}"
db_engine = "${GARAGE_DB_ENGINE}"
replication_factor = ${GARAGE_REPLICATION_FACTOR}

rpc_bind_addr = "${GARAGE_RPC_BIND_ADDR}"
rpc_secret = "${GARAGE_RPC_SECRET}"

[s3_api]
api_bind_addr = "${GARAGE_S3_BIND_ADDR}"
s3_region = "${GARAGE_S3_REGION}"

[s3_web]
bind_addr = "${GARAGE_WEB_BIND_ADDR}"
root_domain = "${GARAGE_WEB_ROOT_DOMAIN:-.web.garage}"

[admin]
api_bind_addr = "${GARAGE_ADMIN_BIND_ADDR}"
admin_token = "${GARAGE_ADMIN_TOKEN}"
EOC

exec /usr/local/bin/garage server
EOF

ENTRYPOINT ["/entrypoint.sh"]
