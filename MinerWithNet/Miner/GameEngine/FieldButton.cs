using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
   public class FieldButton : Button
    {
        public bool isBomb; //бомба или нет
        public bool isClickable; //кликабельность
        public bool wasAdded;//была ли добавлена в очередь
        //позиции в двумерном массиве
        public int xCoord;
        public int yCoord;
    }
}
