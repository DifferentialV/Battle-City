using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game.NonEntities
{
    //камень
    public class Stone : IObjectInGame
    {
        //тип камня отличаются картинкой 
        public TypeCell Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Collision { get; set; } = 0.9F;

        public enum TypeCell{
            stone1,
            stone2,
        }
    }
}
