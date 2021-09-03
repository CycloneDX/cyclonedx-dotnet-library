#!/usr/bin/env bash
# Generates a test coverage report under `coverage-report` directory
# Requires ReportGenerator to be installed https://github.com/danielpalme/ReportGenerator
# `dotnet tool install --global dotnet-reportgenerator-globaltool`
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
reportgenerator -reports:*/coverage.*.cobertura.xml -targetdir:./coverage-report