using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    //обработчик сообщений, который описывает как работать с принятым сообщением
    class MsgController
    {
        Client client;

        //инициализация обработчика сообщений
        public MsgController(Client obj)
        {
            client = obj;
        }
        //обработка команд
        public void doMsg(string message)
        {
            //обработка полученного сообщения 
            //(обработка текста, выделение текста команды и атрибуты)
            string command;
            string atr = String.Empty;
            if (message.IndexOf(":") > -1)
            {
                command = message.Substring(0, message.IndexOf(":"));
                atr = message.Substring(message.IndexOf(":") + 1);
            }
            else
            {
                command = message;
            }
            
            //проверка текста команды и выполнение нужных действий по команде
            using (ContextDB context=new ContextDB())
            {
                //запись данных в БД(формируется объект класса таблицы (Result), 
                //туда записываются нужные нам поля (id, time), 
                //после записываются в таблицу и сохраняются
                if (command == "Win") 
                {
                    Result r = new Result();
                    var user = (from t in context.Result
                                select new { Uid = t.id });
                    if (user.ToList().Count == 0)
                    {
                        r.id = 1;
                        r.Time = atr;
                    }
                    //вычесление следущего id (max+1)
                    else
                    {
                        int max = int.Parse(context.Result.
                                        Select(c => c.id).
                                        DefaultIfEmpty(0).
                                        Max().ToString());
                        r.id = max + 1;
                        r.Time = atr;
                    }
                    //добавление и сохранение результата
                    context.Result.Add(r);
                    context.SaveChanges();
                }
                //извлекаем время из БД, переводим его в нужный формат (время|время|время)
                //и отправляем клиенту.
                if (command == "GetBestResults") //получение лучших результатов
                {
                    string res = "";
                    int[] list_time;

                    //получение времени
                    var list = (from t in context.Result
                                select new { Time = t.Time });
                    //перевод List в массив, после чего сортировка и запись в строку для отправки
                    if (list.ToList().Count > 0)
                    {
                        list_time = new int[list.ToList().Count];
                        int counter = 0;
                        foreach (var i in list)
                        {
                            list_time[counter] = int.Parse(i.Time.Trim());
                            counter++;
                        }
                        Array.Sort(list_time);

                        foreach (var i in list_time)
                        {
                            res += i + "|";
                        }
                        res = res.Substring(0, res.Length - 1).Trim();
                    }
                    else
                    {
                        res = String.Empty;
                    }
                    client.SendMessage(res); //отправка результата
                }
            }
        }

        //получение команды
        public void GetCommand(ref string command, ref string message)
        {
            try
            {
                command = message.Substring(0, message.IndexOf(":"));
                message = message.Substring(message.IndexOf(":") + 1);
            }
            catch { };
        }
    }
}
