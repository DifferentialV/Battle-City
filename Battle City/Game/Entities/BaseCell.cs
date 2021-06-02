using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game.Entities
{
    //класс который хранит состояние Базы
    public class BaseCell : Entity
    {
        public BaseCell(ETeam team)
        {
            Health = 10;
            Team = team;
            Collision = 1;
        }
        //Очередь танков на "создание"
        public Stack<Tank> TankQueue = new Stack<Tank>();
    }
}
