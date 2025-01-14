﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SocketTcpClient
{
    class Program
    {
        // адрес и порт сервера, к которому будем подключаться
        static int port = 8005; // порт сервера
        static string address = "127.0.0.1"; // адрес сервера
        static void Main(string[] args)
        {
            try
            {    //создаем конечную точку
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                //создаем сокет
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                StringBuilder builder = new StringBuilder();
                while (true)
                {
                    Console.Write("Введите сообщение: ");
                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);

                    //посылаем сообщение
                    socket.Send(data);
                    // готовимся получить ответ
                    data = new byte[256]; // буфер для ответа
                    int bytes = 0; // количество полученных байт
                                   // получаем ответ
                    do
                    {
                        bytes = socket.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()}:" + builder.ToString());
                    builder.Remove(0, builder.Length);
                    Array.Clear(data);
                }
                // закрываем сокет
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}