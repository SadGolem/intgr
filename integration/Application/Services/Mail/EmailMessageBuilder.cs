using System.Collections.Concurrent;
using System.Text;

public static class EmailMessageBuilder
{
    public enum ListType
    {
        getentry, setentry,
        getlocation, setlocation,
        getschedule, setschedule,
        getcontragent, setcontragent,
        getemitter, setemitter,
        getall
    }

    // ===== Ошибки =====
    private static readonly ConcurrentDictionary<ListType, ConcurrentDictionary<int, List<string>>> _errorsByOwner = new();
    private static readonly ConcurrentDictionary<ListType, List<string>> _errorsAll = new();

    private static ConcurrentDictionary<int, List<string>> OwnersErr(ListType t) =>
        _errorsByOwner.GetOrAdd(t, _ => new ConcurrentDictionary<int, List<string>>());
    private static List<string> AllErr(ListType t) => _errorsAll.GetOrAdd(t, _ => new List<string>());

    public static void PutError(ListType listType, string message, int? ownerId)
    {
        var line = (message ?? string.Empty) + "\n";

        if (ownerId.HasValue)
        {
            var bag = OwnersErr(listType).GetOrAdd(ownerId.Value, _ => new List<string>());
            lock (bag) bag.Add(line);
        }

        var all = AllErr(listType);
        lock (all) all.Add(line);
    }

    public static IReadOnlyCollection<int> GetOwnersWithErrors(ListType listType) =>
        _errorsByOwner.TryGetValue(listType, out var map) ? map.Keys.ToList() : Array.Empty<int>();

    public static string BuildOwnerErrorsHtml(ListType listType, int ownerId)
    {
        if (_errorsByOwner.TryGetValue(listType, out var map) &&
            map.TryGetValue(ownerId, out var list) && list.Count > 0)
        {
            var sb = new StringBuilder().Append("<h3>Ошибки</h3><pre>");
            lock (list) foreach (var s in list) sb.Append(s);
            sb.Append("</pre>");
            return sb.ToString();
        }
        return string.Empty;
    }

    public static string BuildAllErrorsHtml(ListType listType)
    {
        if (_errorsAll.TryGetValue(listType, out var list) && list.Count > 0)
        {
            var sb = new StringBuilder().Append("<h3>Ошибки</h3><pre>");
            lock (list) foreach (var s in list) sb.Append(s);
            sb.Append("</pre>");
            return sb.ToString();
        }
        return string.Empty;
    }

    // ===== Успехи =====
    private static readonly ConcurrentDictionary<ListType, ConcurrentDictionary<int, List<string>>> _successByOwner = new();
    private static readonly ConcurrentDictionary<ListType, List<string>> _successAll = new();

    private static ConcurrentDictionary<int, List<string>> OwnersOk(ListType t) =>
        _successByOwner.GetOrAdd(t, _ => new ConcurrentDictionary<int, List<string>>());
    private static List<string> AllOk(ListType t) => _successAll.GetOrAdd(t, _ => new List<string>());

    /// <summary>
    /// Регистрируем успешную обработку площадки.
    /// Пример строки: "Success: 12345"
    /// </summary>
    public static void PutSuccess(ListType listType, int locationId, int? ownerId, string? extra = null)
    {
        var line = $"Success: {locationId}{(string.IsNullOrWhiteSpace(extra) ? "" : " — " + extra)}\n";

        if (ownerId.HasValue)
        {
            var bag = OwnersOk(listType).GetOrAdd(ownerId.Value, _ => new List<string>());
            lock (bag) bag.Add(line);
        }

        var all = AllOk(listType);
        lock (all) all.Add(line);
    }

    public static string BuildOwnerSuccessHtml(ListType listType, int ownerId)
    {
        if (_successByOwner.TryGetValue(listType, out var map) &&
            map.TryGetValue(ownerId, out var list) && list.Count > 0)
        {
            var sb = new StringBuilder().Append("<h3>Успехи</h3><pre>");
            lock (list) foreach (var s in list) sb.Append(s);
            sb.Append("</pre>");
            return sb.ToString();
        }
        return string.Empty;
    }

    public static string BuildAllSuccessHtml(ListType listType)
    {
        if (_successAll.TryGetValue(listType, out var list) && list.Count > 0)
        {
            var sb = new StringBuilder().Append("<h3>Успехи</h3><pre>");
            lock (list) foreach (var s in list) sb.Append(s);
            sb.Append("</pre>");
            return sb.ToString();
        }
        return string.Empty;
    }

    // ===== Очистка =====
    public static void Clear(ListType listType)
    {
        _errorsByOwner.TryRemove(listType, out _);
        _errorsAll.TryRemove(listType, out _);
        _successByOwner.TryRemove(listType, out _);
        _successAll.TryRemove(listType, out _);
    }

    public static void ClearAll()
    {
        _errorsByOwner.Clear(); _errorsAll.Clear();
        _successByOwner.Clear(); _successAll.Clear();
    }
}
