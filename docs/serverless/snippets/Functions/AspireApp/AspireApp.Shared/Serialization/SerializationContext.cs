using System.Text.Json.Serialization;

namespace AspireApp.Shared.Serialization;

[JsonSerializable(typeof(UploadResult))]
public sealed partial class SerializationContext : JsonSerializerContext
{
}
