using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game
{
    //объекты в игре должны иметь свое расположение, и коллизию
    public interface IObjectInGame
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Collision { get; set; }
    }
}
