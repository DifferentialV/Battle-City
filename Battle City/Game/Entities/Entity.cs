using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game.Entities
{
    public abstract class Entity : IObjectInGame
    {
        public enum ETeam
        {
            Player,
            Enemy
        }
        //ХП
        public int Health { get; set; }
        public ETeam Team { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Collision { get; set; }
    }
}
