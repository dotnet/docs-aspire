namespace Projects;

public class Web : IProjectMetadata { string IProjectMetadata.ProjectPath => "."; }

public class ApiService : IProjectMetadata { string IProjectMetadata.ProjectPath => "."; }

public class ClientApp : IProjectMetadata { string IProjectMetadata.ProjectPath => "."; }