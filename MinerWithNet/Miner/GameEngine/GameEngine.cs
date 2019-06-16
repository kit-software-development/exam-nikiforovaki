using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    class GameEngine
    {
        GameForm gameForm;
        int width = 10; //ширина
        int height = 10; //длина
        int offset = 30; //дистанция между кпонками
        int bombPercent = 20; //процент выпадения бомб
        bool isFirstClick = true;  //для первого клика
        bool game_end = false; //для проверки окончания игры
        bool isStartGame = false; //для начала игры
        uint seconds = 0; //под время
        bool timer_end = false; //конец отсчета

        FieldButton[,] field; // масив под кнопки
        int cellsOpened = 0;  //количество открытых кнопок
        int bombs = 0; //количество бомб

        public GameEngine(GameForm obj)
        {
            gameForm = obj;
            field = new FieldButton[width, height];
            GenerateField();
        }

        private void Timer() //таймер
        {
            while (!timer_end)
            {
                seconds++;
                gameForm.SetTime(seconds);
                Thread.Sleep(1000);
            }
        }

        public void GenerateField() //Генерация кнопок по полю
        {
            Random random = new Random(); //для бомб
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    FieldButton newButton = new FieldButton();
                    newButton.Location = new Point(x * offset, (y + 1) * offset); //позиция кнопки
                    newButton.Size = new Size(offset, offset); //размер кнопки
                    newButton.isClickable = true; //установка кликабельности 
                    newButton.TabStop = false;  //отключение перевода фокуса
                    if (random.Next(0, 100) <= bombPercent) //расставление бомб
                    {
                        newButton.isBomb = true; //установка бомбы на клетку
                        bombs++; //количество бомб
                    }
                    newButton.xCoord = x; 
                    newButton.yCoord = y;
                    gameForm.Controls.Add(newButton); //добавление на поле
                    newButton.MouseUp += new MouseEventHandler(FieldButtonClick); //приязываем клик к мышке
                    field[x, y] = newButton; //добавление в массив кнопку
                }
            }
        }

        void FieldButtonClick(object sender, MouseEventArgs e) //при клике
        {
            if (!isStartGame) //начать игру
            {
                isStartGame = true;
                Task.Run(() => Timer()); //поток запускает метод Таймера
            }

            if (!game_end)
            {
                FieldButton clickedButton = (FieldButton)sender; //преобразуем object в button
                if (e.Button == MouseButtons.Left && clickedButton.isClickable)  // отклик на левую кнопку и если кликабельный
                {
                    if (clickedButton.isBomb) 
                    {
                        // чтобы не было проигрыша при первом клике
                        if (isFirstClick)//если первый клик
                        {
                            //убираем с этой клетки бомбу 
                            clickedButton.isBomb = false;
                            isFirstClick = false;
                            bombs--;
                            OpenRegion(clickedButton.xCoord, clickedButton.yCoord, clickedButton);
                        }
                        else
                        {
                            Explode();//взрыв
                        }

                    }
                    else
                    {
                        EmptyFieldButtonClick(clickedButton); 
                    }
                    isFirstClick = false;
                }
                if (e.Button == MouseButtons.Right) //если правой мышкой, то ставим "В"
                {
                    clickedButton.isClickable = !clickedButton.isClickable;
                    if (!clickedButton.isClickable)
                    {
                        clickedButton.Text = "B";
                    }
                    else
                    {
                        clickedButton.Text = "";
                    }
                }
                CheckWin();
            }
        }

        void Explode() //взрыв
        {
 
            foreach (FieldButton button in field)//цикл для вывода всех бомб
            {
                if (button.isBomb)
                {
                    button.Text = "♥";
                }
            }
            GameEnd();
            MessageBox.Show("Вы проиграли :(");
            game_end = true;
            timer_end = true;
        }

        void EmptyFieldButtonClick(FieldButton clickedButton) //если не бомба
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (field[x, y] == clickedButton)
                    {
                        OpenRegion(x, y, clickedButton);
                    }
                }
            }
        }
        //открываем все нули и все соседние клетки нулей
        void OpenRegion(int xCoord, int yCoord, FieldButton clickedButton) //для открытия соседних клеток
        {
            Queue<FieldButton> queue = new Queue<FieldButton>();
            queue.Enqueue(clickedButton);//добавляем нажатый элемент
            while (queue.Count > 0)
            {
                FieldButton currentCell = queue.Dequeue(); //получаем первый элемент в очереди
                OpenCell(currentCell.xCoord, currentCell.yCoord, currentCell);//открытие самой ячейки
                cellsOpened++;
                if (CountBombsAround(currentCell.xCoord, currentCell.yCoord) == 0)
                {
                    for (int y = currentCell.yCoord - 1; y <= currentCell.yCoord + 1; y++)
                    {
                        for (int x = currentCell.xCoord - 1; x <= currentCell.xCoord + 1; x++)
                        {
                            if (x >= 0 && x < width && y >= 0 && y < height  )
                            {
                                //если уже не была добавлена, то добавляем в очередь
                                if (!field[x, y].wasAdded)
                                {
                                    queue.Enqueue(field[x, y]);
                                    field[x, y].wasAdded = true;
                                }

                            }
                        }
                    }
                }
            }
        }
        void OpenCell(int x, int y, FieldButton clickedButton) //открытие ячейки
        {
            //считаем количество бомб и выводим это количество, если не 0
            int bombsAround = CountBombsAround(x, y);
            if (bombsAround == 0)
            {

            }
            else
            {
                clickedButton.Text = "" + bombsAround;
            }
            clickedButton.Enabled = false;
        }

        int CountBombsAround(int xCoord, int yCoord) //количество бомб вокруг
        {
            int bombsAround = 0;
            for (int x = xCoord - 1; x <= xCoord + 1; x++)
            {
                for (int y = yCoord - 1; y <= yCoord + 1; y++)
                {
                    //условие чтоб не было ошибок, когда клик на крайние ячейки
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (field[x, y].isBomb == true)
                        {
                            bombsAround++;
                        }
                    }
                }
            }
            return bombsAround;
        }
        void CheckWin()//проверка на победу
        {
            //считаем сколько всего ячеек, после вычисляем ячейки без бомб
            int cells = width * height;
            int emptyCells = cells - bombs;
            if (cellsOpened >= emptyCells)
            {
                GameEnd();
                MessageBox.Show("Вы победили! ;)");

                //отправляем секунды 
                if (gameForm.connect.isConnect)
                {
                    gameForm.connect.SendMessage("Win:" + seconds.ToString());
                }

                game_end = true;
                timer_end = true;
            }
        }

        void GameEnd() //окончание игры
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (field[x, y].Enabled == true)
                    {
                        //открытие всех бомб
                        if (field[x, y].isBomb)
                        {
                            OpenCell(x, y, field[x, y]);
                            field[x, y].Text = "♥";
                        }
                    }
                }
            }
        }
    }
}
