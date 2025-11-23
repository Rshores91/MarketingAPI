#!/bin/bash

tailscaled --tun=userspace-networking --socks5-server=localhost:1055 &

sleep 5

tailscale up --authkey=${TAILSCALE_AUTHKEY} --hostname=marketing-api

sleep 5

exec dotnet MarketingAPI.dll