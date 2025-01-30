namespace integration.Context
{

    public class StatusCoder
    {
        private static Dictionary<int, string> _statusMapping = new Dictionary<int, string>
    {
        { 179, "Выполнено" },
        { 43, "Добавлена в план" },
        { 61, "На согласовании" },
        { 301, "Не выполнена" },
        { 166, "Отклонено" },
        { 282, "Отменено" },
        { 302, "Новая" }, //согласовано и принято в работу
        { 300, "Требуется информация" },
        { 52, "Черновик" }
    };
     /*   private static Dictionary<int, int> _containersMapping = new Dictionary<int, int>
    {
        { 28, "сетка металлическая на колесах" },
        { 27, "Евроконтейнер" }, //пластик
        { 20, "Сетка металлическая на колесах" },
        { 19, "Свободный объём" },
        { 18, "Заглубленный" },
        { 10, "Урна" },
        { 9,  107 }, //бункер 8.0
        { 8,  "Пресс-компактор" },
        { 7,  "Пакет/мешок/биг-бэг" },
        { 6,  "Фандомат" },
        { 5,  "Пластиковый в форме колокола" }, //пластик
        { 4,  "Евроконтейнер" },
        { 3,  "Металлический контейнер" }, //металл 0.75
        { 2,  "Евроконтейнер металл" }  // 0/75
    };*/


        public string ToCorrectStatus(EntryData wasteData)
        {
            int status = wasteData.Status?.Id ?? 0;
            if (status == 0)
                return "";
            else if (status != 179 && status != 302 && status != 282)
                return "";
            else if (_statusMapping.ContainsKey(status))
            {
                return _statusMapping[status];  // Перекодируем статус если он существует в словаре
            }
            return "";
        }

        public int ToCorrectContainer(EntryData wasteData)
        {
            if (wasteData.IdContainerType == null) return -1;

            int containerId = wasteData.IdContainerType.Id;

            /*if (_containersMapping.ContainsKey(containerId))
            {
                return _containersMapping[containerId];
            }
            else
            {
                // Логика по умолчанию или обработка случая, когда ID не найден
                //  Например, можно вернуть -1, null, или другой идентификатор.
                return -1; //  -1 если не найдено.
            }*/
            return 5;
        }
    }
}
