using integration.Services.Location;

public sealed class LocationStatusTransitionPlanner : IStatusTransitionPlanner
{
    private static string L(string s) => (s ?? string.Empty).Trim().ToLowerInvariant();

    // Канонические имена, чтобы убрать вариации формулировок/рода и синонимы
    // Ключ: любая входная форма -> Значение: каноническое имя
    private static readonly IReadOnlyDictionary<string, string> _alias = new Dictionary<string, string>
    {
        [L("неизвестная")] = "не проверено",
        [L("не проверено")] = "не проверено",

        [L("временно закрыта")] = "временно закрыто",
        [L("временно закрыто")] = "временно закрыто",

        [L("инспекция")] = "запланировано",
        [L("запланировано")] = "запланировано",

        [L("проверено")] = "проверено",
        [L("фактическая")] = "проверено", // трактуем как проверено

        [L("изменено")] = "изменено",
        [L("изменена")] = "изменено", // нормализуем род

        [L("новая")] = "новая",
        [L("черновик")] = "черновик",
        [L("обслуживается")] = "обслуживается",
    };

    private static string Canon(string? name)
    {
        var k = L(name ?? string.Empty);
        return _alias.TryGetValue(k, out var v) ? v : k; // если нет алиаса — работаем с нормализованной строкой
    }

    private readonly List<(string From, string To, int TransitionId)> _edges;
    private readonly Dictionary<string, List<(string To, int Id)>> _adj;

    // Фолбэк на «один шаг в целевой», если путь не найден
    // (по желанию можно удалить/сузить, если хотим строго только по графу)
    private static readonly IReadOnlyDictionary<string, int> _fallbackTargetOnly =
        new Dictionary<string, int>
        {
            [L("не проверено")] = 235,   // у тебя именно 235 идёт как переход к "Не проверено" из некоторых состояний
            [L("проверено")]    = 654,   // «изменено -> проверено», как наиболее частый завершающий шаг
            [L("временно закрыто")] = 236,
            [L("запланировано")] = 307,  // «черновик -> запланировано» — прямой шаг в целевой
            [L("изменено")]     = 651,   // «запланировано -> изменено», если цель «изменено»
            [L("обслуживается")] = 302,  // «проверено -> обслуживается»
        };

    public LocationStatusTransitionPlanner()
    {
        // ТВОИ РЕБРА (без изменений), но через Canon, чтобы вход нормализовать
        _edges = new List<(string, string, int)>
        {
            (Canon("Новая"),            Canon("Временно закрыто"), 236),
            (Canon("Временно закрыто"), Canon("Не проверено"),     235),

            (Canon("Новая"),            Canon("Черновик"),         387),
            (Canon("Черновик"),         Canon("Запланировано"),    307),

            (Canon("Запланировано"),    Canon("Изменено"),         651),
            (Canon("Изменено"),         Canon("Проверено"),        654),

            (Canon("Проверено"),        Canon("Изменено"),         653),
            (Canon("Изменено"),         Canon("Не проверено"),     235),

            (Canon("Обслуживается"),    Canon("Изменено"),         659),
            (Canon("Изменено"),         Canon("Временно закрыто"), 661),

            // Прямые переходы
            (Canon("Не проверено"),     Canon("Изменено"),         656),

            (Canon("Проверено"),        Canon("Обслуживается"),    302),
        };

        _adj = _edges
            .GroupBy(e => e.From)
            .ToDictionary(g => g.Key, g => g.Select(x => (x.To, x.TransitionId)).ToList());
    }

    public IReadOnlyList<int> BuildPath(string? currentStatusName, int? currentStatusId, string targetStatusName)
    {
        var cur = Canon(currentStatusName);
        var trg = Canon(targetStatusName);

        // 1) «Неизвестная» => «Не проверено» уже покрыто Canon
        if (!string.IsNullOrWhiteSpace(cur) && cur == trg)
            return Array.Empty<int>();

        // 2) Спец-правило: если текущий статус id == 74 и целевой «Не проверено»,
        //    выполняем строго: Новая -> Временно закрыто (236) -> Не проверено (235)
        if (currentStatusId == 74 && trg == Canon("Не проверено"))
            return new[] { 236, 235 };

        // 3) BFS по графу (если знаем имя текущего)
        if (!string.IsNullOrWhiteSpace(cur))
        {
            var path = Bfs(cur, trg);
            if (path is { Count: > 0 })
                return path;
        }

        // 4) Фолбэк — один шаг в цель, если известен id
        if (_fallbackTargetOnly.TryGetValue(trg, out var single))
            return new[] { single };

        return Array.Empty<int>();
    }

    private List<int>? Bfs(string start, string goal)
    {
        var q = new Queue<string>();
        var seen = new HashSet<string> { start };
        var prev = new Dictionary<string, (string Prev, int ViaId)>();

        q.Enqueue(start);

        while (q.Count > 0)
        {
            var v = q.Dequeue();
            if (v == goal) return Reconstruct(goal, prev);

            if (!_adj.TryGetValue(v, out var nexts)) continue;

            foreach (var (to, id) in nexts)
            {
                if (!seen.Add(to)) continue;
                prev[to] = (v, id);
                q.Enqueue(to);
            }
        }
        return null;
    }

    private static List<int> Reconstruct(string goal, Dictionary<string, (string Prev, int ViaId)> prev)
    {
        var ids = new List<int>();
        var cur = goal;
        while (prev.TryGetValue(cur, out var p))
        {
            ids.Add(p.ViaId);
            cur = p.Prev;
        }
        ids.Reverse();
        return ids;
    }
}