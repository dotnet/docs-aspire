These tests are simply a harness for leveraging
 Playwright and .NET Aspire together to automate the maintenance of images. Run the following tests individually to generate the corresponding images:

```
dotnet test --filter Capture=help-images
dotnet test --filter Capture=project-resources
dotnet test --filter Capture=themes
dotnet test --filter Capture=stop-start-resources
dotnet test --filter Capture=resource-text-visualizer
dotnet test --filter Capture=resource-details
dotnet test --filter Capture=resource-filtering
dotnet test --filter Capture=resource-errors
```
