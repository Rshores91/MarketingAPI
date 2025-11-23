#!/bin/bash

mkdir -p /tmp/tailscale
tailscaled \
    --state=/tmp/tailscale/tailscaled.state \
    --socket=/tmp/tailscale/tailscaled.sock \
    --tun=userspace-networking \
    --socks5-server=localhost:1055 &

sleep 5

tailscale up  \
    --authkey=${TAILSCALE_AUTHKEY} \
    --hostname=marketing-api \
    --accept-dns=false \
    --accept-routes \
    --advertise-tags=tag:render

sleep 10

tailscale status

exec dotnet MarketingAPI.dll