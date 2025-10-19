namespace integration.Services.Location;

public interface IStatusTransitionPlanner
{
    /// <param name="currentStatusName">Имя текущего статуса (если есть)</param>
    /// <param name="currentStatusId">ID текущего статуса (если есть)</param>
    /// <param name="targetStatusName">Целевой статус по данным АПРО</param>
    IReadOnlyList<int> BuildPath(string? currentStatusName, int? currentStatusId, string targetStatusName);
}