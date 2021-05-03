#!/bin/bash
sudo systemctl stop hentaipages.service
dotnet publish -c Release -o ~/programs/HentaiPagesRelease/
sudo systemctl restart hentaipages.service
