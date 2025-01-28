using System.Collections.Generic;
using static EmailMessageBuilder;

public static class EmailMessageBuilder
{
    public enum ListType
    {
        GetEntryInfo,
        SetEntryInfo
    }

    private static Dictionary<ListType, List<string>> _lists = new Dictionary<ListType, List<string>>
    {
        { ListType.GetEntryInfo, new List<string>() },
        { ListType.SetEntryInfo, new List<string>() }
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
        //foreach()
        //_lists[listType].Clear();
    }

    public static void AddNewList(ListType listType)
    {
        if (!_lists.ContainsKey(listType))
        {
            _lists.Add(listType, new List<string>());
        }
    }
}
