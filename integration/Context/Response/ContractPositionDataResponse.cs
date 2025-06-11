using System.Text.Json.Serialization;
using integration.Context;

public class ContractPositionDataResponse : DataResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("number")]
    public string Number { get; set; } = null!;
    
    [JsonPropertyName("estimation_value")]
    public double? EstimationValue { get; set; }
    
    [JsonPropertyName("value")]
    public double? Value { get; set; }
    
    [JsonPropertyName("value_manual")]
    public double? ValueManual { get; set; }
    
    [JsonPropertyName("date_start")]
    public string DateStart { get; set; } = null!;
    
    [JsonPropertyName("date_end")]
    public string DateEnd { get; set; } = null!;
    
    [JsonPropertyName("contract")]
    public ContractDataResponse Contract { get; set; } = null!;
    
    [JsonPropertyName("waste_site")]
    public WasteSite WasteSite { get; set; } = null!;
    
    [JsonPropertyName("waste_source")]
    public WasteSource WasteSource { get; set; } = null!;
    
    [JsonPropertyName("status")]
    public Status Status { get; set; } = null!;
}

public class ContractDataResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("root_id")]
    public string RootId { get; set; } = null!;
    
    [JsonPropertyName("participant")]
    public Participant Participant { get; set; } = null!;
    
    [JsonPropertyName("status")]
    public Status Status { get; set; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

public class WasteSite
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("datetime_create")]
    public DateTime DatetimeCreate { get; set; }
    
    [JsonPropertyName("datetime_update")]
    public DateTime DatetimeUpdate { get; set; }
    
    [JsonPropertyName("lon")]
    public double Lon { get; set; }
    
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
    
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    
    [JsonPropertyName("author")]
    public Author Author { get; set; } = null!;
    
    [JsonPropertyName("participant")]
    public Participant? Participant { get; set; }
    
    [JsonPropertyName("status")]
    public Status Status { get; set; } = null!;
}

public class WasteSource
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("author")]
    public Author Author { get; set; } = null!;
    
    [JsonPropertyName("participant")]
    public Participant Participant { get; set; } = null!;
    
    [JsonPropertyName("waste_source_category")]
    public WasteSourceCategory WasteSourceCategory { get; set; } = null!;
    
    [JsonPropertyName("status")]
    public Status Status { get; set; } = null!;
    
    [JsonPropertyName("normative_unit_value_exist")]
    public bool NormativeUnitValueExist { get; set; }
}

public class Status
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class Participant
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    
    [JsonPropertyName("short_name")]
    public string? ShortName { get; set; }
    
    [JsonPropertyName("inn")]
    public string? Inn { get; set; }
    
    [JsonPropertyName("kpp")]
    public string? Kpp { get; set; }
    
    [JsonPropertyName("ogrn")]
    public string? Ogrn { get; set; }
    
    [JsonPropertyName("doc_type")]
    public DocType? DocType { get; set; }
    
    [JsonPropertyName("root_company")]
    public object? RootCompany { get; set; }
    
    [JsonPropertyName("waste_person")]
    public object? WastePerson { get; set; }
}

public class Author
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

public class WasteSourceCategory
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

public class DocType
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}