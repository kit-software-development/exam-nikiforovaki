using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public class Connection
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private TcpClient client;
        private NetworkStream stream;
        private string message;
        public bool isConnect = false;

        GameForm gameForm;
        
        //передача объекта формы в объект соединения, чтобы работать с ней
        public Connection(GameForm obj)
        {
            gameForm = obj;
        }

        //установка соединения
        public void StartConnection()
        {
            if (!isConnect)
            {
                client = new TcpClient();
                try
                {
                    client.Connect(host, port); //подключение клиента
                    stream = client.GetStream(); // получаем поток

                    // запускаем новый поток для получения данных
                    isConnect = true;
                    Task.Run(() => ReceiveMessage());
                }
                catch
                {
                }
            }
        }

        //отправка сообщения
        public void SendMessage(string msg) 
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }

        //получения сообщения
        private void ReceiveMessage()
        {
            StringBuilder builder = new StringBuilder();//динамическая строка, выделяется больше памяти 
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; //буфер для получаемых данных
                    builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        //прочитывает данные
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    message = builder.ToString();

                    gameForm.LoadStat(message);
                    Thread.Sleep(10);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            if (isConnect)
            {
                stream?.Close(); //отключение потока
                client?.Close(); //отключение клиента
                isConnect = false;
            }
        }
    }
}
