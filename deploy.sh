#!/usr/bin/env bash
set -euo pipefail

echo "Publishing app..."
dotnet publish CartFlow.Web -c Release -o ./publish

echo "Writing production config..."
cat >./publish/appsettings.json <<JSON
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "constr": "$1"
  },
  "StripeKeys": {
    "SecretKey": "$2",
    "PublishableKey": "$3"
  }
}
JSON

echo "Done. Now upload publish/ to MonsterASP via FTP or File Manager."
echo "Usage: bash deploy.sh <db_connection_string> <stripe_secret_key> <stripe_publishable_key>"
