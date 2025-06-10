using System.ComponentModel;

namespace integration.Context
{
    public static class StatusCoder
    {
        private static Dictionary<int, string> _statusEntryAPRO = new Dictionary<int, string>
        {
            { 179, "Выполнено" },
            { 43, "Добавлена в план" },
            { 61, "На согласовании" },
            { 301, "Не выполнена" },
            { 166, "Отклонено" },
            { 282, "Отменено" },
            { 302, "Согласовано и принято в работу" }, //согласовано и принято в работу
            { 300, "Требуется информация" },
            { 52, "Черновик" }
        };
        private static List<(string, string)> _statusEntryAPROtoMT = new List<(string, string)>
        {
            {( "Выполнена", "Выполено" )},
            {( "Закрыта", "Выполено" )},
            {( "Передано перевозчику", "Добавлена в план" )},
            {( "Отменено", "Отменена") },
            {( "Согласовано и принято в работу", "Новая") },
        };



        private static List<(int Key, string Value)> _statusLocation = new List<(int Key, string Value)>
        {
            ( 52, "Новая" ),
            ( 74, "Заявка на проверку" ),
            ( 167, "Проверенная" ),
            ( 301, "Изменена" ), //не готово
            ( 167, "Инспекция" ),
            ( 14, "Действующая" ),
            ( 67, "Фактическая" ),
            ( 74, "Плановая" ),
            ( 159, "Неизвестная" ),
            ( 70, "Закрыта" ),
            ( 167, "Проинспектирована" )
        };

        private static Dictionary<int, double> _containersCapacityMapping = new Dictionary<int, double>
        {
            { 1, 1.1 },
            { 2, 0.77 },
            { 3, 0.66 },
            { 4, 0.36 },
            { 5, 0.24 },
            { 6, 0.12 },
            { 11, 0.8 },
            { 15, 0.9 },
            { 21, 1.1 },
            { 39, 1.1 },
            { 42, 0.12 },
            { 45, 0.75 },
            { 54, 0.24 },
            { 124, 0.1 },
            { 432, 24 },
            { 1184, 0.75 },
            { 1187, 0.6 },
            { 1255, 0.66 },
            { 1262, 0.77 },
            { 1263, 0.6 },
            { 1267, 0.65 },
            { 1268, 0.4 },
            { 1270, 0.63 },
            { 1273, 0.55 },
            { 1274, 0.64 },
            { 1288, 1 },
            { 1293, 0.65 },
            { 1586, 1 },
            { 2402, 8 },
            { 2424, 0.61 },
            { 2446, 0.36 },
            { 2449, 6 },
            { 2450, 0.57 },
            { 2452, 72 },
            { 2453, 36 },
            { 2455, 0.59 },
            { 2459, 0.11 },
            { 2460, 0.67 },
            { 2462, 1.2 },
            { 2516, 0.2 },
            { 2517, 11 },
            { 2520, 27 },
            { 2521, 30 },
            { 2522, 6.3 },
            { 2523, 6.4 },
            { 2524, 6.5 },
            { 2525, 6.7 },
            { 2526, 6.9 },
            { 2527, 7.2 },
            { 2528, 7.3 },
            { 2529, 7.8 },
            { 2530, 7 },
            { 2531, 0.62 },
            { 2532, 4 },
            { 2533, 6.24 },
            { 2534, 48 }
        };

        private static List<(int Key, int Value)> _containersMapping = new List<(int Key, int Value)>
        {
            (3, 153), (19, 99), (7, 142), (7, 103), (4, 98), (38, 102), (3, 111),
            (4, 34), (3, 114), (3, 105), (3, 137), (3, 139), (3, 132), (3, 138),
            (3, 136), (3, 135), (2, 145), (3, 143), (4, 37), (3, 134), (3, 106),
            (3, 2), (4, 160), (4, 96), (3, 148), (3, 146), (29, 36), (3, 108),
            (4, 97), (20, 152), (3, 157), (29, 117), (29, 113), (8, 110), (29, 133),
            (29, 115), (29, 129), (29, 128), (29, 140), (29, 107), (29, 130),
            (29, 118), (29, 19), (29, 112), (29, 151), (3, 155), (8, 154), (4, 131),
            (4, 104), (4, 150), (3, 165), (3, 159), (8, 109), (8, 162), (29, 166),
            (29, 164), (8, 161), (3, 141), (3, 156), (19, 101), (19, 149), (19, 158),
            (19, 163), (19, 100), (19, 144), (4, 147)
        };
        

        public static string ToCorrectStatusEntryToMT(EntryDataResponse entry)
        {
            if (entry?.status == null)
                return "Новая";
            
            int statusId = entry.status;
            
            if (!_statusEntryAPRO.TryGetValue(statusId, out string aproStatus))
            {
                return "Новая";
            }

            string result = aproStatus;
            
            foreach (var mapping in _statusEntryAPROtoMT)
            {
                if (mapping.Item1 == aproStatus)
                {
                    result = mapping.Item2;
                }
            }

            return result;
        }
        
        
        public static string ToCorrectStatus(EntryDataResponse wasteDataResponse)
        {
            int status = wasteDataResponse.status;
            if (status == 0)
                return "";
            else if (status != 179 && status != 302 && status != 282)
                return "";
            else if (_statusEntryAPRO.TryGetValue(status, out string? value))
            {
                return value;  // Перекодируем статус если он существует в словаре
            }
            return "";
        }
        

        public static double ToCorrectCapacity(int idCapacity)
        {
            if (idCapacity == null) return -1;

            if (_containersCapacityMapping.ContainsKey(idCapacity))
            {
                return _containersCapacityMapping[idCapacity];
            }
            else
            {
                return -1;
            }
        }

        public static string ToCorrectLocationStatus(int id) //Возвращаем список
        {
            string results = "";
            foreach (var item in _statusLocation)
            {
                if (item.Key == id)
                {
                    results = item.Value;
                    return results;
                }
            }
            return results; // Вернет пустой список, если ничего не найдено
        }

    }
}
