using System.Text.Json.Serialization;

namespace integration.Context;

public class ContractPositionData : Data
{
    [JsonPropertyName("id")] //позиция договора
    public int id { get; set; }

    [JsonPropertyName("number")]
    public string number { get; set; }

    [JsonPropertyName("status")]
    public Status status { get; set; }

    [JsonPropertyName("waste_source")]
    public EmitterData waste_source { get; set; } // Тип изменен на WasteSource

    [JsonPropertyName("waste_site")]
    public LocationData waste_site { get; set; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string value { get; set; } // Может быть nullable string (string?), если может отсутствовать

    [JsonPropertyName("value_manual")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string value_manual { get; set; } // Может быть nullable string (string?), если может отсутствовать

    [JsonPropertyName("contract")]
    public ContractData contract { get; set; }

    [JsonPropertyName("estimation_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string estimation_value { get; set; }

    [JsonPropertyName("date_start")]
    public string DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public string DateEnd { get; set; }
}

