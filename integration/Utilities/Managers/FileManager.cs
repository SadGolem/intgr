using System.Globalization;

namespace integration.Managers;

public class FileManager
{
    const string _filePath = "C:\\Projects\\intgr\\saves\\save01.txt";
    public static FileManager fileManager;
    private string DateTimeFormat = "";

    public FileManager(string dateTimeFormat)
    {
        DateTimeFormat = dateTimeFormat;
        fileManager = this;
    }

    public DateTime GetText(string key)
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var lines = File.ReadAllLines(_filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2 && parts[0] == key)
                    {
                        if (DateTime.TryParseExact(parts[1], DateTimeFormat, CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out DateTime lastUpdate))
                        {
                            return lastUpdate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading last update time from file: {ex.Message}");
                return DateTime.MinValue;
            }
        }
        return DateTime.MinValue;
    }

    public void SetText(List<string> lines, bool updated, DateTime cstTime, string key)
    {
        if (File.Exists(_filePath))
        {
            try
            {
                lines.AddRange(File.ReadAllLines(_filePath));
                for (int i = 0; i < lines.Count; i++)
                {
                    var parts = lines[i].Split('=');
                    if (parts.Length == 2 && parts[0] == key)
                    {
                        lines[i] = $"{key}={cstTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)}";
                        updated = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading last update time from file: {ex.Message}");
            }

        }

        if (!updated)
        {
            lines.Add($"{key}={cstTime.ToString(DateTimeFormat, CultureInfo.InvariantCulture)}");
        }

        try
        {
            File.WriteAllLines(_filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing last update time to file: {ex.Message}");
        }
    }
}