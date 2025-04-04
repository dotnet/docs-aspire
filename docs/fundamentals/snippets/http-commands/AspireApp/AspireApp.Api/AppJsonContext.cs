using System.Text.Json.Serialization;

namespace AspireApp.Api;

[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
public partial class AppJsonContext : JsonSerializerContext;

