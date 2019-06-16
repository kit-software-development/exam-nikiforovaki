using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //выводит информацию в консоль на сервер
    public class LoggerController
    {

        Client client;

        //инициализация логгер
        public LoggerController(Client obj)
        {
            client = obj;
        }
         //вывод лога в консоль
        public void WriteConnectionLog(string message)
        {
            Console.WriteLine("[" + client.Id + "]: " + message);
        }
    }
}
