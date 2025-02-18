using System.Globalization;
using System.Net;
using System.Net.Sockets;
using integration.Managers;

public static class TimeManager 
{
    private const string DateTimeFormat = "O";
    private static string timeZone = "Europe/London";
    private static FileManager _fileManager; static TimeManager()
    {
        _fileManager = new FileManager( DateTimeFormat);
    }
    public static DateTime GetLastUpdateTime(string key)
    {
        return _fileManager.GetText(key);
    }

    public static void SetLastUpdateTime(string key)
    {
        DateTime lastUpdate = GetNetworkTimeUtc();
        TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(lastUpdate, cstZone);

        var lines = new List<string>();
        bool updated = false;
        _fileManager.SetText(lines, updated, cstTime, key);
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
