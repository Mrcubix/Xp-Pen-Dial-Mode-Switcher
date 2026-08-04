#!/usr/bin/env bash

dotnet restore

if [ ! -d "build" ]; then
    mkdir build
fi

dotnet publish src -c Release -o build/plugin/0.6.x

(
  cd build
  # zip the DialModeSwitcher.dll
  jar -cfM DialModeSwitcher.zip ./*.dll

  sha256sum DialModeSwitcher.zip >> hashes.txt
)