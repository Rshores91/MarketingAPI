#!/bin/bash
set -e

# Configuration
MAX_WAIT=60  # Maximum seconds to wait
POLL_INTERVAL=2  # Seconds between checks

echo "=== Starting Tailscale Setup ==="
echo "Timestamp: $(date)"

# Start Tailscale daemon
echo "Starting tailscaled..."
tailscaled --state=/var/lib/tailscale/tailscaled.state --tun=userspace-networking --socks5-server=localhost:1055 &
TAILSCALED_PID=$!
echo "Tailscaled PID: $TAILSCALED_PID"

# Wait for tailscaled to be ready
echo "Waiting for tailscaled to be ready..."
ELAPSED=0
while [ $ELAPSED -lt $MAX_WAIT ]; do
  if tailscale status &>/dev/null; then
    echo "Tailscaled is ready after ${ELAPSED}s"
    break
  fi
  
  if ! kill -0 $TAILSCALED_PID 2>/dev/null; then
    echo "ERROR: Tailscaled process died"
    exit 1
  fi
  
  sleep $POLL_INTERVAL
  ELAPSED=$((ELAPSED + POLL_INTERVAL))
done

if [ $ELAPSED -ge $MAX_WAIT ]; then
  echo "ERROR: Tailscaled failed to start within ${MAX_WAIT}s"
  exit 1
fi

# Connect to Tailscale
echo "Connecting to Tailscale network..."
tailscale up \
  --authkey=${TAILSCALE_AUTHKEY} \
  --hostname=render-app \
  --accept-dns=false \
  --accept-routes || {
    echo "ERROR: Failed to connect to Tailscale"
    exit 1
  }

# Wait for Tailscale to be fully connected
echo "Waiting for Tailscale connection..."
ELAPSED=0
while [ $ELAPSED -lt $MAX_WAIT ]; do
  STATUS=$(tailscale status --json 2>/dev/null || echo '{}')
  
  # Check if we have a valid IP (indicates successful connection)
  if echo "$STATUS" | grep -q '"TailscaleIPs"'; then
    echo "Tailscale connected after ${ELAPSED}s"
    break
  fi
  
  sleep $POLL_INTERVAL
  ELAPSED=$((ELAPSED + POLL_INTERVAL))
done

if [ $ELAPSED -ge $MAX_WAIT ]; then
  echo "ERROR: Tailscale failed to connect within ${MAX_WAIT}s"
  tailscale status
  exit 1
fi

# Show status
echo "=== Tailscale Status ==="
tailscale status

# Get our IP
echo "=== Our Tailscale IP ==="
OUR_IP=$(tailscale ip -4)
echo "IP: $OUR_IP"

# Wait for database server to be reachable via Tailscale
if [ ! -z "$DB_SERVER_IP" ]; then
  echo "=== Testing Database Server Reachability ==="
  ELAPSED=0
  DB_REACHABLE=false
  
  while [ $ELAPSED -lt $MAX_WAIT ]; do
    echo "Attempting to ping $DB_SERVER_IP (attempt $((ELAPSED / POLL_INTERVAL + 1)))..."
    
    if tailscale ping $DB_SERVER_IP -c 1 --timeout=5s &>/dev/null; then
      echo "Database server reachable after ${ELAPSED}s"
      DB_REACHABLE=true
      break
    fi
    
    sleep $POLL_INTERVAL
    ELAPSED=$((ELAPSED + POLL_INTERVAL))
  done
  
  if [ "$DB_REACHABLE" = false ]; then
    echo "WARNING: Database server not reachable via Tailscale after ${MAX_WAIT}s"
    echo "Proceeding anyway - application will retry connections"
  fi
  
  # Test MySQL connection
  echo "=== Testing MySQL Connection ==="
  if command -v mysql &>/dev/null; then
    timeout 10 mysql -h $DB_SERVER_IP -u myappuser -p${DB_PASSWORD} -e "SELECT 1 AS test;" 2>&1 && \
      echo "MySQL connection successful!" || \
      echo "WARNING: MySQL connection failed - check credentials and database configuration"
  else
    echo "MySQL client not installed, skipping database test"
  fi
fi

echo "=== Starting Application ==="
exec dotnet MarketingAPI.dll