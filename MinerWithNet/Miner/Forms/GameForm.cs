using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public partial class GameForm : Form
    {
        public Connection connect;
        private GameEngine engine;

        public GameForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connect = new Connection(this);
            connect.StartConnection();
            BestList.TabStop = false;
            engine = new GameEngine(this);

            if (connect.isConnect)
            {
                connect.SendMessage("GetBestResults:");
            }
            else
            {
                MessageBox.Show("Отсутствует соединение с сервером! Игра будет запущена в оффлайн режиме!");
                BestList.Text = "В оффлайн режиме статистика не загружается!";
            }
        }

        public void LoadStat(string answer)
        {
            if (!String.IsNullOrEmpty(answer))
            {
                for (int i = 0; i < 10; i++)
                {
                    if (answer.IndexOf("|") > -1)
                    {
                        string res = answer.Substring(0, answer.IndexOf("|"));
                        answer = answer.Substring(answer.IndexOf("|") + 1);
                        BestList.Text += (i + 1).ToString() + " Место: " + res + Environment.NewLine;
                    }
                    else
                    {
                        BestList.Text += (i + 1).ToString() + " Место: " + answer + Environment.NewLine;
                        answer = string.Empty;
                    }
                }
            }
        }

        private void StartItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void About_MenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.Show();
        }

        public void SetTime(uint time)
        {
            TimeLabel.Text = time.ToString();
        }
    }
}