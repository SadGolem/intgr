using System.Globalization;
using System.Net;
using System.Net.Sockets;

public static class LastUpdateTextFileManager 
{
    private const string _filePath = "C:\\Projects\\intgr\\saves\\save01.txt";
    private const string DateTimeFormat = "O";
    private static string timeZone = "Europe/London";

    public static DateTime GetLastUpdateTime(string key)
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
                        if (DateTime.TryParseExact(parts[1], DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastUpdate))
                        {
                            return lastUpdate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading last update time from file: {ex.Message}");
            }
        }
        return DateTime.MinValue;
    }

    public static void SetLastUpdateTime(string key)
    {
        DateTime lastUpdate = GetNetworkTimeUtc();
        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(lastUpdate, cstZone);
        //lastUpdate = cstTime;
        var lines = new List<string>();
        bool updated = false;
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

    public static DateTime GetNetworkTimeUtc(string ntpServer = "time.nist.gov")
    {
        var ntpData = new byte[48];

        ntpData[0] = 0x1B;

        var addresses = Dns.GetHostEntry(ntpServer).AddressList;

        var ipEndPoint = new IPEndPoint(addresses[0], 123);
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        {
            socket.Connect(ipEndPoint);

            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
        }

        const byte serverReplyTime = 40;

        ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

        ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

        intPart = SwapEndianness(intPart);
        fractPart = SwapEndianness(fractPart);

        var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
        return (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
    }

    static uint SwapEndianness(ulong x)
    {
        return (uint)(((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24));
    }

}
