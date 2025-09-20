using System.Text.Json.Serialization;

namespace integration.Context.Request;

public sealed class AgreMTResponse
{
    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }
    
    [JsonPropertyName("data")]
    public int Data { get; init; }
}

public interface IMessageBroker
{
    /// <summary>
    /// Публикует ACK в МТ как массив sourceKey. 
    /// Возвращает ответ МТ (message/timestamp/data).
    /// </summary>
    Task<AgreMTResponse> PublishAckAsync(IEnumerable<string> sourceKeys, CancellationToken ct = default);
}