using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game.NonEntities
{    
    //хранит состояние предмета на игровом поле
    //создавался для хранения Выпавших модулей
    class Item : IObjectInGame
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Collision { get; set; } = 0;
        //хранимый предмет
        public dynamic Value { get; set; }
    }
}
