#!/bin/bash
dotnet publish -c Release -o ~/programs/HentaiPagesRelease/
sudo systemctl restart hentaipages.service
