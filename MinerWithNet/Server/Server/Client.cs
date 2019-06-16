using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
  
    public class Client
    {
        //Информация о соединении
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        private TcpClient client;
        private Server server; //объект сервера

        MsgController controller;
        LoggerController log;

        //инициализация клиента
        public Client(TcpClient tcpClient, Server serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            controller = new MsgController(this); 
            log = new LoggerController(this);

            log.WriteConnectionLog("Connected!");
        }

        //прослушивание входящих сообщений
        public void Process()
        {
            try
            {
                //Получаем поток
                Stream = client.GetStream();
                //Переменная для записи сообщения
                string message;
                // Получение собщения
                while (true)
                {
                    try
                    {
                        message = string.Empty;
                        //Получаем сообщение
                        message = GetMessage();
                        //Обрабатываем сообщение
                        log.WriteConnectionLog(message);
                        controller.doMsg(message);
                        //Очищаем текст сообщения после обработки
                        Thread.Sleep(10);//нужна задержка, чтобы не склеивались несколько сообщений
                    }
                    catch
                    {
                        //Если какие-то ошибки, выходим из цикла
                        break;
                    }
                }
            }
            finally
            {
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку (от сервера)
        private string GetMessage()
        {
            byte[] data = new byte[64]; //буфер для получаемых данных
            StringBuilder builder = new StringBuilder(); //динамическая строка, выделяется больше памяти 
            int bytes = 0;
            do
            {
                //прочитывает данные
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        //Отправка текстового сообщения (на сервер)
        public void SendMessage(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length); //передача данных
        }

        //Закрытие подключения
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
            server.RemoveConnection(this.Id);
            log.WriteConnectionLog("Disconnected!");
        }
    }
}
