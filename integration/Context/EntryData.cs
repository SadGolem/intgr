using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

public class EntryData
{
    [JsonPropertyName("client_contact")]
    public string? ConsumerName { get; set; } // Может быть null

    [JsonPropertyName("datetime_create")]
    public DateTime DateTimeCreate { get; set; }

    [JsonPropertyName("datetime_update")]
    public DateTime DateTimeUpdate { get; set; }

    [JsonPropertyName("id")]
    public int BtNumber { get; set; }

    [JsonPropertyName("author")]
    public AuthorData? Author { get; set; } //  Объект Author

    [JsonPropertyName("status")]
    public StatusData? Status { get; set; } // Объект Status

    [JsonPropertyName("type")]
    [JsonConverter(typeof(TypeConverter))]  // Применяем конвертер
    public TypeData? Type { get; set; } //Сделаем nullable, а далее будем использовать TypeName

    [JsonPropertyName("date")]
    public string? PlanDateRO { get; set; } // Обработка в сеттере сохранена

    [JsonPropertyName("volume")]
    public float? Volume { get; set; }

    public string CreationDate
    {
        get => DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
    }
    public int? IdContainerType { get; set; } // Предположим, может быть null

    public int? CommentByRO { get; set; } // Предположим, может быть null

    //Здесь наоборот json из МТ в БТ поэтому так
    public string? Ext_id { get; set; }

    public string LocationStatus
    {
        get
        {
            return this.Status?.Name;
        }
    }
    public string AuthorName
    {
        get
        {
            return this.Author?.Name;
        }
    }

    public int? Amount { get; set; }

    public string EntryType
    {
        get
        {
            return "Заявка";
        }
    }
    public string? TypeName
    {
        get
        {
            return this.Type?.Name;
        }
    }

    [JsonIgnore]
    public string StatusId
    {
        get { return Status?.Id.ToString(); } // Не отправляем json
    }
    [JsonIgnore]
    public string StatusName
    {
        get { return Status?.Name.ToString(); } // Не отправляем json
    }
    [JsonIgnore]
    public string StatusCategory
    {
        get { return Status?.Category; } // Не отправляем json
    }
    [JsonIgnore]
    public string StatusCode
    {
        get { return Status?.Code; } // Не отправляем json
    }
    [JsonIgnore]
    public string StatusColor
    {
        get { return Status?.Color; } // Не отправляем json
    }
    [JsonIgnore]
    public string StatusIcon
    {
        get { return Status?.Icon; } // Не отправляем json
    }
    [JsonIgnore]
    public bool? StatusIsActive
    {
        get { return Status?.IsActive; } // Не отправляем json
    }
    [JsonIgnore]
    public int? StatusSort
    {
        get { return Status?.Sort; } // Не отправляем json
    }
    [JsonIgnore]
    public bool? StatusMeanDeleted
    {
        get { return Status?.MeanDeleted; } // Не отправляем json
    }

}
// Классы для вложенных объектов
public class AuthorData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class StatusData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("category")]
    public string? Category { get; set; }
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }
    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
    [JsonPropertyName("sort")]
    public int? Sort { get; set; }
    [JsonPropertyName("mean_deleted")]
    public bool? MeanDeleted { get; set; }
}
