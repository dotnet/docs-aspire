#!/bin/bash

filters=(
    "help-images"
    "project-resources"
    "themes"
    "stop-start-resources"
    "resource-text-visualizer"
    "resource-details"
    "resource-filtering"
    "resource-errors"
    "structured-logs-errors",
    "structured-logs",
    "trace-logs"
)

for filter in "${filters[@]}"; do
    echo "Running tests for Capture=$filter..."
    dotnet test --filter "Capture=$filter"
done
