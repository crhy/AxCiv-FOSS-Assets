#!/bin/sh
export DOTNET_ROOT=/app/lib/dotnet
cd /app/lib/rhyciv || exit 1
exec /app/lib/dotnet/dotnet RaylibUI.dll "$@"
