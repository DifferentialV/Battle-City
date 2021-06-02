using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game
{
    public class Bot
    {
        public List<Way> Ways { get; set; } = new List<Way>();
        public Entities.Tank Control { get; set; }
        public IObjectInGame Target { get; set; }
    }
    public class Way
    {
        public Entities.Tank.ECourse Course { get; set; }
        public int Count { get; set; }
    }

    public class Node
    {
        public float X { get; set; }
        public float Y { get; set; }
        public double G { get; set; }
        public double H { get; set; }
        public double F { get { return G + H; } }
        public Node Before { get; set; }
        public bool Opening { get; set; }
    }

}
