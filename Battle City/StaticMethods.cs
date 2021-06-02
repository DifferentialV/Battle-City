using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Battle_City
{
    static class StaticMethods
    {
        //создает Изображение Игрового поля, (земля и камни) 
        public static Bitmap BattlePlaceInit(int screen, Game.Game game)
        {
            //размеры клетки подбираем чтобы Итоговый размер поля был не больше размера экрана
            float cell_size = (float)Math.Round((double)screen / Battle_City.Game.Game.square_cell,5);
            
            Bitmap BattlePlace = new Bitmap((int)Math.Ceiling(cell_size * Battle_City.Game.Game.square_cell), (int)Math.Ceiling(cell_size * Battle_City.Game.Game.square_cell));
            Graphics graphic = Graphics.FromImage(BattlePlace);
            //изображения камней и земли
            Bitmap ground = Properties.Resources.Ground;
            Bitmap stone1 = Properties.Resources.Stone1;
            Bitmap stone2 = Properties.Resources.Stone2;

            //на все клетки "устанавливаем землю"
            for (int i = 0; i < Battle_City.Game.Game.square_cell; i++)
            {
                for (int j = 0; j < Battle_City.Game.Game.square_cell; j++)
                {
                    graphic.DrawImage(ground, cell_size * j, cell_size * i, cell_size, cell_size);
                }
            }
            GC.Collect();

            //Устанавливаем камни которые есть в игре на свои места
            foreach (var stone in game.GameObjects.OfType<Battle_City.Game.NonEntities.Stone>())
            {
                switch (stone.Type)
                {
                    case Battle_City.Game.NonEntities.Stone.TypeCell.stone1: graphic.DrawImage(stone1, cell_size * stone.X, cell_size * stone.Y, cell_size, cell_size); break;
                    case Battle_City.Game.NonEntities.Stone.TypeCell.stone2: graphic.DrawImage(stone2, cell_size * stone.X, cell_size * stone.Y, cell_size, cell_size); break;
                }
            }
            GC.Collect();
            return BattlePlace;

        }

        //Создает и возвращает список изображений всех возможных танков и баз
        public static Dictionary<string, Bitmap> entitesViewsInit(int screen)
        {
            //размеры клетки подбираем по размеру экрана
            float cell_size = (float)screen / Battle_City.Game.Game.square_cell;

            Dictionary<string, Bitmap> tankViews = new Dictionary<string, Bitmap>();
            //обходим все Команды, Корпуса, Пушки, Башни
            foreach (var team in Enum.GetValues<Game.Entities.Entity.ETeam>())
            {
                foreach (var @base in Enum.GetValues<Game.Entities.Tank.EBase>())
                {
                    foreach (var tower in Enum.GetValues<Game.Entities.Tank.ETower>())
                    {
                        foreach (var cannon in Enum.GetValues<Game.Entities.Tank.ECannon>())
                        {
                            Bitmap bitmap = new Bitmap((int)Math.Ceiling(cell_size), (int)Math.Ceiling(cell_size));
                            Graphics graphic = Graphics.FromImage(bitmap);
                            graphic.DrawImage(Properties.Resources.Trucks, 0, 0, cell_size, cell_size);
                            if (team == Battle_City.Game.Entities.Entity.ETeam.Player)
                            {
                                switch (@base)
                                {
                                    case Battle_City.Game.Entities.Tank.EBase.B1: graphic.DrawImage(Properties.Resources.Player_Base1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.EBase.B2: graphic.DrawImage(Properties.Resources.Player_Base2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.EBase.B3: graphic.DrawImage(Properties.Resources.Player_Base3, 0, 0, cell_size, cell_size); break;
                                }
                                switch (tower)
                                {
                                    case Battle_City.Game.Entities.Tank.ETower.T1: graphic.DrawImage(Properties.Resources.Player_Tower1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ETower.T2: graphic.DrawImage(Properties.Resources.Player_Tower2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ETower.T3: graphic.DrawImage(Properties.Resources.Player_Tower3, 0, 0, cell_size, cell_size); break;
                                }
                                switch (cannon)
                                {
                                    case Battle_City.Game.Entities.Tank.ECannon.FC1: graphic.DrawImage(Properties.Resources.Player_FastCannon1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.FC2: graphic.DrawImage(Properties.Resources.Player_FastCannon2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.FC3: graphic.DrawImage(Properties.Resources.Player_FastCannon3, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.PC1: graphic.DrawImage(Properties.Resources.Player_PoweredCannon1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.PC2: graphic.DrawImage(Properties.Resources.Player_PoweredCannon2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.PC3: graphic.DrawImage(Properties.Resources.Player_PoweredCannon3, 0, 0, cell_size, cell_size); break;
                                }
                            }
                            else
                            {
                                switch (@base)
                                {
                                    case Battle_City.Game.Entities.Tank.EBase.B1: graphic.DrawImage(Properties.Resources.Enemy_Base1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.EBase.B2: graphic.DrawImage(Properties.Resources.Enemy_Base2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.EBase.B3: graphic.DrawImage(Properties.Resources.Enemy_Base3, 0, 0, cell_size, cell_size); break;
                                }
                                switch (tower)
                                {
                                    case Battle_City.Game.Entities.Tank.ETower.T1: graphic.DrawImage(Properties.Resources.Enemy_Tower1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ETower.T2: graphic.DrawImage(Properties.Resources.Enemy_Tower2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ETower.T3: graphic.DrawImage(Properties.Resources.Enemy_Tower3, 0, 0, cell_size, cell_size); break;
                                }
                                switch (cannon)
                                {
                                    case Battle_City.Game.Entities.Tank.ECannon.FC1: graphic.DrawImage(Properties.Resources.Enemy_FastCannon1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.FC2: graphic.DrawImage(Properties.Resources.Enemy_FastCannon2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.FC3: graphic.DrawImage(Properties.Resources.Enemy_FastCannon3, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.PC1: graphic.DrawImage(Properties.Resources.Enemy_PoweredCannon1, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.PC2: graphic.DrawImage(Properties.Resources.Enemy_PoweredCannon2, 0, 0, cell_size, cell_size); break;
                                    case Battle_City.Game.Entities.Tank.ECannon.PC3: graphic.DrawImage(Properties.Resources.Enemy_PoweredCannon3, 0, 0, cell_size, cell_size); break;
                                }
                            }
                            //КЛЮЧ состоит наименований соответствующих частей танка
                            tankViews.Add($"tank{team}{@base}{tower}{cannon}", bitmap);
                            GC.Collect();
                        }
                    }
                }
            }
            //добавляем базы играка и противника
            {
                Bitmap bitmap = new Bitmap((int)Math.Ceiling(cell_size), (int)Math.Ceiling(cell_size));
                Graphics graphic = Graphics.FromImage(bitmap);
                graphic.DrawImage(Properties.Resources._Base, 0, 0, cell_size, cell_size);
                graphic.DrawImage(Properties.Resources.Player_Flag, 0, 0, cell_size, cell_size);
                tankViews.Add($"basePlayer", RotateImage(bitmap, -90));
            }
            {
                Bitmap bitmap = new Bitmap((int)Math.Ceiling(cell_size), (int)Math.Ceiling(cell_size));
                Graphics graphic = Graphics.FromImage(bitmap);
                graphic.DrawImage(Properties.Resources._Base, 0, 0, cell_size, cell_size);
                graphic.DrawImage(Properties.Resources.Enemy_Flag, 0, 0, cell_size, cell_size);
                tankViews.Add($"baseEnemy", RotateImage(bitmap, 90));
            }
            GC.Collect();
            return tankViews;
        }
        //функция поворота изображения
        public static Bitmap RotateImage(Bitmap b, float angle)
        {
            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
                g.DrawImage(b, 0, 0, b.Width, b.Width);
                g.ResetTransform();
            }
            return returnBitmap;
        }

        //возвращает очередь танков
        public static Stack<Game.Entities.Tank> RandomTanks(Game.Entities.Entity.ETeam team)
        {
            Stack<Game.Entities.Tank> tanks = new Stack<Game.Entities.Tank>();
            do
            {
                //набор частей таков
                List<int> bases = new List<int> (){0,1,2,0,1,2 };
                List<int> towers = new (){0,1,2,0,1,2 };
                List<int> cannons = new (){0,1,2,0,1,2 };
                List<bool> cannon_doubles = new (){true,false,true,false,true,false };
                //случайным образом перемешиваем 
                bases.Shuffle();
                towers.Shuffle();
                cannons.Shuffle();
                cannon_doubles.Shuffle();
                for (int i=0;i<6;i++)
                {
                    tanks.Push(new Game.Entities.Tank(
                        (Game.Entities.Tank.EBase)bases[i],
                        (Game.Entities.Tank.ECannon)(cannons[i] * (cannon_doubles[i]?1:2)),
                        (Game.Entities.Tank.ETower)towers[i],
                        team));
                }
            }
            while (tanks.Count < 30);
            return tanks;
        }

        static Random random = new Random((int)DateTime.Now.Ticks);
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(list.Count);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
