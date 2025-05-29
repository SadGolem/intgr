using System.Text.Json.Serialization;
using integration.Services.Integration;
using integration.Services.Integration.Interfaces;

namespace integration.Context
{
    public abstract class DataResponse : IIntegratableEntity
    {
        [JsonPropertyName("datetime_create")] public DateTime datetime_create { get; set; }
        [JsonPropertyName("datetime_update")] public DateTime datetime_update { get; set; }
        public int ext_id { get; set; }

        public virtual int GetIntegrationExtId()
        {
            return ext_id;
        }

        public virtual void UpdateIntegrationId(int newId)
        {
            ext_id = newId;
        }
    }

    public class Container
    {
        [JsonPropertyName("id")] public int? id { get; set; }
        [JsonPropertyName("type")] public Type? type { get; set; }
    }

    public class AuthorData
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    public class Type
    {
        [JsonPropertyName("id")] public int? id { get; set; }
    }

    public class ClientContact
    {
        [JsonPropertyName("id")] public int? id { get; set; }
        [JsonPropertyName("name")] public string? name { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("id")] public int id { get; set; }
    }

    public class Capacity
    {
        [JsonPropertyName("id")] public int id { get; set; }
    }

    public class StatusData
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("category")] public string? Category { get; set; }
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("color")] public string? Color { get; set; }
        [JsonPropertyName("icon")] public string? Icon { get; set; }
        [JsonPropertyName("is_active")] public bool? IsActive { get; set; }
        [JsonPropertyName("sort")] public int? Sort { get; set; }
        [JsonPropertyName("mean_deleted")] public bool? MeanDeleted { get; set; }
    }

    public class Status
    {
        [JsonPropertyName("id")] public int id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }

    public class Author
    {
        [JsonPropertyName("id")] public int id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
}
