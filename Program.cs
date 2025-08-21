using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server_Online_Shop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer();
            server.Start();

            Console.WriteLine("Нажмите Enter для остановки сервера...");
            Console.ReadLine();
            server.Stop();
        }
    }

    public class ChatServer
    {
        private TcpListener listener;
        private bool isRunning = true;

        public void Start()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            listener.Start();
            Console.WriteLine("Сервер чата запущен...");

            while (isRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено сообщение: {message}");

                    string response = "Сообщение получено поддержкой. Мы ответим вам в ближайшее время.";
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при работе с клиентом: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        public void Stop()
        {
            isRunning = false;
            listener.Stop();
        }
    }
}