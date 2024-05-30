using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace SocketTcpServer
{
    class Program
    {
        static int port = 8005; // порт для приема входящих запросов
        static private List<Socket> clients = new List<Socket>();
        static void Main(string[] args)
        {
            String Host = Dns.GetHostName();
            Console.WriteLine("Comp name = " + Host);
            IPAddress[] IPs;
            IPs = Dns.GetHostAddresses(Host);
            foreach (IPAddress ip1 in IPs)
                Console.WriteLine(ip1);


            //получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет сервера
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    Socket handler = listenSocket.Accept();  // сокет для связи с     клиентом
                                                             // готовимся  получать  сообщение
                    clients.Add(handler);
                    Thread thread = new Thread(HandleClient);
                    thread.Start(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static bool Accept(Socket handler, byte[] InputData, StringBuilder builder)
        {
            try
            {
                var time = DateTime.Now.ToShortTimeString();
                int bytes = 0; // количество полученных байтов за 1 раз
                do
                {
                    bytes = handler.Receive(InputData);  // получаем сообщение
                    builder.Append(Encoding.Unicode.GetString(InputData, 0, bytes));
                }
                while (handler.Available > 0);
                string message = builder.ToString();
                if (message.IndexOf("/Close") != -1)
                    return false;
                else 
                {
                    message = time + " " + message;
                    SendMessage(Encoding.Unicode.GetBytes(message));
                    message = "";
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
                //clients.Remove(handler);
            }
        }

        static void HandleClient(object obj)
        {
            try
            {
                Socket handler = (Socket)obj;
                Console.WriteLine("Подключился новый клиент.");
                StringBuilder builder = new StringBuilder();
                byte[] InputData = new byte[255]; // буфер для получаемых данных
                bool InTouch = true;
                while (InTouch)
                {
                    InTouch = Accept(handler, InputData, builder);
                    builder.Remove(0, builder.Length);
                    Array.Clear(InputData);
                }
                Console.WriteLine($"Клиент {handler.ExclusiveAddressUse} отключился");
                // закрываем сокет
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                clients.Remove(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void SendMessage(byte[] bytes)
        {
            foreach(var client in clients)
            {
                client.Send(bytes);
            }
        }
    }
}