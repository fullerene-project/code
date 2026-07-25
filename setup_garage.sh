#!/usr/bin/env bash
set -euo pipefail

CONTAINER_NAME="fullerene-garage"

# 1. Check if the container is running
if ! podman ps --format "{{.Names}}" | grep -q "^${CONTAINER_NAME}$"; then
    echo "Error: Container ${CONTAINER_NAME} is not running." >&2
    exit 1
fi

# 2. Get the node ID
NODE_ID=$(podman exec "${CONTAINER_NAME}" /garage node id -q | cut -d'@' -f1)
echo "Node initialization: ${NODE_ID}"

# 3. Layout configuration
LAYOUT_VERSION=$(podman exec "${CONTAINER_NAME}" /garage layout show 2>/dev/null | grep -i "Current cluster layout version" | awk '{print $NF}' || echo "0")
if [ "${LAYOUT_VERSION}" -eq "0" ]; then
    podman exec "${CONTAINER_NAME}" /garage layout assign "${NODE_ID}" --capacity 10GB --zone local
    podman exec "${CONTAINER_NAME}" /garage layout apply --version 1
fi

# Function to get or create a key
get_or_create_key() {
    local name=$1
    local info_out
    if podman exec "${CONTAINER_NAME}" /garage key info "${name}" >/dev/null 2>&1; then
        # If the key was created previously, read it
        info_out=$(podman exec "${CONTAINER_NAME}" /garage key info "${name}" --show-secret)
    else
        # Create a new key
        info_out=$(podman exec "${CONTAINER_NAME}" /garage key create "${name}")
    fi
    local access_key=$(echo "${info_out}" | grep "Key ID:" | awk '{print $NF}')
    local secret_key=$(echo "${info_out}" | grep "Secret key:" | awk '{print $NF}')
    echo "${access_key}:${secret_key}"
}

# 4. Key generation and retrieval
echo "Configuring S3 access keys..."
WORKER_CREDS=$(get_or_create_key "worker-key")
SIGNER_CREDS=$(get_or_create_key "signer-key")
MANAGER_CREDS=$(get_or_create_key "manager-key")

# Separate Access and Secret
WORKER_ACCESS=$(echo "${WORKER_CREDS}" | cut -d':' -f1)
WORKER_SECRET=$(echo "${WORKER_CREDS}" | cut -d':' -f2)

SIGNER_ACCESS=$(echo "${SIGNER_CREDS}" | cut -d':' -f1)
SIGNER_SECRET=$(echo "${SIGNER_CREDS}" | cut -d':' -f2)

MANAGER_ACCESS=$(echo "${MANAGER_CREDS}" | cut -d':' -f1)
MANAGER_SECRET=$(echo "${MANAGER_CREDS}" | cut -d':' -f2)

# Function to create a bucket
create_bucket() {
    local name=$1
    if ! podman exec "${CONTAINER_NAME}" /garage bucket info "${name}" >/dev/null 2>&1; then
        podman exec "${CONTAINER_NAME}" /garage bucket create "${name}"
    fi
}

# 5. Creating buckets
echo "Creating buckets..."
create_bucket "unsigned-artifacts"
create_bucket "signed-artifacts"

# 6. Configuring bucket access permissions
echo "Configuring access permissions..."
# Permissions for unsigned-artifacts bucket
podman exec "${CONTAINER_NAME}" /garage bucket allow --key worker-key --write unsigned-artifacts
podman exec "${CONTAINER_NAME}" /garage bucket allow --key signer-key --read unsigned-artifacts
podman exec "${CONTAINER_NAME}" /garage bucket allow --key manager-key --read --write unsigned-artifacts

# Permissions for signed-artifacts bucket
podman exec "${CONTAINER_NAME}" /garage bucket allow --key signer-key --read --write signed-artifacts
podman exec "${CONTAINER_NAME}" /garage bucket allow --key manager-key --read --write signed-artifacts

# 7. Print final keys to the console
echo "=================================================="
echo "               CONNECTION KEY DATA:               "
echo "=================================================="
echo "1. worker-key:"
echo "   S3 Access Key: ${WORKER_ACCESS}"
echo "   S3 Secret Key: ${WORKER_SECRET}"
echo "--------------------------------------------------"
echo "2. signer-key:"
echo "   S3 Access Key: ${SIGNER_ACCESS}"
echo "   S3 Secret Key: ${SIGNER_SECRET}"
echo "--------------------------------------------------"
echo "3. manager-key:"
echo "   S3 Access Key: ${MANAGER_ACCESS}"
echo "   S3 Secret Key: ${MANAGER_SECRET}"
echo "=================================================="
