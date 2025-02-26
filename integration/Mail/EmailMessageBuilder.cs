public static class EmailMessageBuilder 
{
    public enum ListType
    {
        getentry,
        setentry,
        getlocation,
        setlocation,
        getschedule,
        setschedule,
        getcontragent,
        setcontragent,
        getemitter,
        setemitter
    }

    private static Dictionary<ListType, List<string>> _lists = new Dictionary<ListType, List<string>>
    {
        { ListType.getcontragent, new List<string>() },
        { ListType.setcontragent, new List<string>() },
        { ListType.getemitter, new List<string>() },
        { ListType.setemitter, new List<string>() },
        { ListType.getlocation, new List<string>() },
        { ListType.setlocation, new List<string>() },
        { ListType.getschedule, new List<string>() },
        { ListType.setschedule, new List<string>() },
        { ListType.getentry, new List<string>() },
        { ListType.setentry, new List<string>() }
    };

    public static void PutInformation(ListType listType, string s)
    {
        _lists[listType].Add(s + "\n");
    }

    public static string GetList(ListType listType)
    {
        string info = "";
        if (_lists.ContainsKey(listType))
        {
            foreach (string s in _lists[listType])
            {
                info = info + s;
            }
        }

        return info;
    }

    public static void ClearList(ListType listType)
    {
        _lists[listType].Clear();
    }
    public static void ClearList()
    {
        foreach (var list in (_lists))
        {
            list.Value.Clear();
        }
    }

    public static void AddNewList(ListType listType)
    {
        if (!_lists.ContainsKey(listType))
        {
            _lists.Add(listType, new List<string>());
        }
    }


}
