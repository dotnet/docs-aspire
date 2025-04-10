$filters = @(
    "help-images",
    "project-resources",
    "themes",
    "stop-start-resources",
    "resource-text-visualizer",
    "resource-details",
    "resource-filtering",
    "resource-errors",
    "structured-logs-errors",
    "structured-logs",
    "trace-logs"
)

foreach ($filter in $filters) {
    Write-Host "Running tests for Capture=$filter..."
    dotnet test --filter "Capture=$filter"
}
