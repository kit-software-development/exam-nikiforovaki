using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        TcpListener tcpListener; // сервер для прослушивания
        public List<Client> clients = new List<Client>(); // все подключения

        //добавление клиентов
        public void AddConnection(Client clientObject)
        {
            clients.Add(clientObject);
        }
        //удаление клиентов
        public void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            Client client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        public void Listen()
        {
            try
            {
                //соединение с любого ip адреса по указанному нами порту
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                //готов слушать порт
                tcpListener.Start(); 
                while (true)
                {
                    
                    TcpClient tcpClient = tcpListener.AcceptTcpClient(); 
                    Client clientObject = new Client(tcpClient, this);
                    AddConnection(clientObject);

                    Task.Run(() => clientObject.Process());
                }
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }

        // отключение всех клиентов
        public void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиентов
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
