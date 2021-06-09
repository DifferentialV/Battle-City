using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battle_City.Game.Entities;
using Battle_City.Game.NonEntities;

namespace Battle_City.Game
{
    //Класс, который управляет состояние игры
    public class Game
    {
        //Размерность игрового поля (реальное поле будет на 2 меньше, по краям стена из камней)
        public const int square_cell = 24;
        //список объектов в игре
        public List<IObjectInGame> GameObjects;
        public List<Bot> Bots;
        //строка оповещения и завершении игры
        public string GameOver { get; private set; } = null;
        public Tank Player1Tank;
        public Tank Player2Tank;
        public BaseCell Player1Base;
        public BaseCell Player2Base;
        Random random = new Random((int)DateTime.Now.Ticks);
        public Game()
        {
            GameObjects = new List<IObjectInGame>();
            //устанавливаем стену из камней по краям игрового поля
            for (int i=1;i< square_cell-1; i++)
            {
                GameObjects.Add(new Stone() {X=0,Y=i, Type = Stone.TypeCell.stone2 });
                GameObjects.Add(new Stone() {X= square_cell - 1, Y=i, Type = Stone.TypeCell.stone2 });
                GameObjects.Add(new Stone() {X= i, Y=0, Type = Stone.TypeCell.stone1 });
                GameObjects.Add(new Stone() { X = i, Y= square_cell - 1, Type = Stone.TypeCell.stone1 });
            }
            //устанавливаем камни на случайные клетки кроме клеток в радиусе 2 вокруг базы
            for (int i = 1; i < square_cell - 1; i++)
            {
                for (int j = i > square_cell - 7 ? 6 : 1; j < square_cell - (i < 6 ? 6 : 1); j++)
                {
                    if (random.Next(100) - 80 >= 0)
                    {
                        GameObjects.Add(new Stone() { X = j, Y = i, Type = random.Next() % 2 == 0 ? Stone.TypeCell.stone1 : Stone.TypeCell.stone2 });
                    }
                }
            }
            //создаем базы
            //базы устанавливаем по координатам и заполняем очередь на создания танков
            Player1Base = new BaseCell(Entity.ETeam.Player) { X = 3, Y = square_cell - 4,TankQueue = StaticMethods.RandomTanks(Entity.ETeam.Player) };
            Player2Base = new BaseCell(Entity.ETeam.Enemy) { X = square_cell - 4, Y = 3, TankQueue = StaticMethods.RandomTanks(Entity.ETeam.Enemy) };
            
            //Первым танком управляет игрок1
            Player1Tank = Player1Base.TankQueue.Pop();
            Player1Tank.X = 4;
            Player1Tank.Y = square_cell - 4;

            //Первым танком управляет игрок2
            Player2Tank = Player2Base.TankQueue.Pop();
            Player2Tank.Course = Tank.ECourse.Down;
            Player2Tank.X = square_cell - 5;
            Player2Tank.Y = 3;

            GameObjects.Add(Player1Base);
            GameObjects.Add(Player2Base);
            GameObjects.Add(Player1Tank);
            GameObjects.Add(Player2Tank);

            //bot1 = new Bot();
        }
        //Bot bot1;


        //Вызывает по таймеру из основной класс BattleCity
        public void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            //первый игрок
            //танк движется по направлению, которое соответствует последней нажатой клавише
            if (player1Moves.Count > 0)
                MoveTank(Player1Tank, player1Moves.Last());

            //Стрельба, ниже будет вызвана функция, которая обработает выстрел
            if (player1Shoot)
            {
                Player1Tank.Shooting = true;
            }
            //игрок подбирает Модуль
            //Выбираться первый Модуль, с которым пересекается моделька игрока
            if (player1TakeItem)
            {
                Item item = GameObjects.OfType<Item>().FirstOrDefault(i => Math.Abs(i.X - Player1Tank.X) < Player1Tank.Collision && Math.Abs(i.Y - Player1Tank.Y) < Player1Tank.Collision);
                if(item != null)
                {
                    if (item.Value.GetType() == typeof(Tank.EBase)) Player1Tank.SetBase(item.Value);
                    if (item.Value.GetType() == typeof(Tank.ETower)) Player1Tank.SetTower(item.Value);
                    if (item.Value.GetType() == typeof(Tank.ECannon)) Player1Tank.SetCannon(item.Value);
                }
                GameObjects.Remove(item);
            }
            //игрок уничтожает Модуль
            //уничтожается первый Модуль, с которым пересекается моделька игрока
            if (player1DestroyItem)
            {
                Item item = GameObjects.OfType<Item>().FirstOrDefault(i => Math.Abs(i.X - Player1Tank.X) < Player1Tank.Collision && Math.Abs(i.Y - Player1Tank.Y) < Player1Tank.Collision);
                if (item != null)
                    GameObjects.Remove(item);
            }

            //второй игрок
            //танк движется по направлению, которое соответствует последней нажатой клавише
            if (player2Moves.Count > 0)
                MoveTank(Player2Tank, player2Moves.Last());

            //Стрельба, ниже будет вызвана функция, которая обработает выстрел
            if (player2Shoot)
            {
                Player2Tank.Shooting = true;
            }
            //игрок подбирает Модуль
            //Выбираться первый Модуль, с которым пересекается моделька игрока
            if (player2TakeItem)
            {
                Item item = GameObjects.OfType<Item>().FirstOrDefault(i => Math.Abs(i.X - Player2Tank.X) < Player2Tank.Collision && Math.Abs(i.Y - Player2Tank.Y) < Player2Tank.Collision);
                if (item != null)
                {
                    if (item.Value.GetType() == typeof(Tank.EBase)) Player2Tank.SetBase(item.Value);
                    if (item.Value.GetType() == typeof(Tank.ETower)) Player2Tank.SetTower(item.Value);
                    if (item.Value.GetType() == typeof(Tank.ECannon)) Player2Tank.SetCannon(item.Value);
                }
                GameObjects.Remove(item);
            }
            //игрок уничтожает Модуль
            //уничтожается первый Модуль, с которым пересекается моделька игрока
            if (player2DestroyItem)
            {
                Item item = GameObjects.OfType<Item>().FirstOrDefault(i => Math.Abs(i.X - Player2Tank.X) < Player2Tank.Collision && Math.Abs(i.Y - Player2Tank.Y) < Player2Tank.Collision);
                if (item != null)
                    GameObjects.Remove(item);
            }




            //Сделать расчет Выстрелов, Смертей, Работа Баз
            //BotTask();
            CannonTask();
            EntityDeathTask();
            BaseTask();
        }
        //стрельба танков
        void CannonTask()
        {
            //все танки
            foreach (var tank in GameObjects.OfType<Tank>())
            {
                //если танк на перезарядке, уменьшаем количество оставшихся тиков
                if (tank.CannonCooldown > 0)
                {
                    tank.CannonCooldown--;
                }
                //если танк собирается выстрелить и оружие не на перезарядке 
                if (tank.Shooting && tank.CannonCooldown == 0)
                {
                    //растояние на которое стреляем
                    tank.CannonRange = tank.CannonMaxRange;
                    //стрельба происходит из центра модели танка
                    float x = tank.X + 0.5F, y = tank.Y + 0.5F;
                    //ближайший Объект в который попал выстрел и коллизия больше нуля (если такой имеется)
                    IObjectInGame hit = tank.Course switch
                    {
                        Tank.ECourse.Top => GameObjects.Where(o => o.Collision > 0 && o.X < x && x < o.X + 1 && y > o.Y + 1 && o.Y + 1 >= tank.Y - tank.CannonMaxRange).OrderBy(o => tank.Y - o.Y).FirstOrDefault(),
                        Tank.ECourse.Down => GameObjects.Where(o => o.Collision > 0 && o.X < x && x < o.X + 1 && y < o.Y && o.Y <= tank.Y + tank.CannonMaxRange).OrderBy(o => o.Y - tank.Y).FirstOrDefault(),
                        Tank.ECourse.Left => GameObjects.Where(o => o.Collision > 0 && o.Y < y && y < o.Y + 1 && x > o.X + 1 && o.X + 1 >= tank.X - tank.CannonMaxRange).OrderBy(o => tank.X - o.X).FirstOrDefault(),
                        Tank.ECourse.Right => GameObjects.Where(o => o.Collision > 0 && o.Y < y && y < o.Y + 1 && x < o.X && o.X <= tank.X + tank.CannonMaxRange).OrderBy(o => o.X - tank.X).FirstOrDefault(),
                        _ => null
                    };
                    //есть во что-то попали
                    if (hit != null)
                    {
                        //расстояние которое "прошел" выстрел
                        tank.CannonRange = (float)Math.Sqrt(Math.Pow(tank.X - hit.X, 2) + Math.Pow(tank.Y - hit.Y, 2)) - 0.25F;
                        //если попали в Entity, согласно "меткости" танка рассчитываем отнялось ли ХП у цели
                        if (hit.GetType().IsAssignableTo(typeof(Entity)))
                        {
                            if (tank.Accuracy - random.Next(100) >= 0)
                                ((Entity)hit).Health -= tank.Damage;
                            if (tank.DoubleShot && tank.Accuracy - random.Next(100) >= 0)
                                ((Entity)hit).Health -= tank.Damage;
                        }
                    }
                    //отправляем на перезарядку
                    tank.CannonCooldown = Tank.CannonCooldownConst;
                }
                //нет стрельбы нет стрельбы
                else
                {
                    tank.Shooting = false;
                }
            }
        }
        //смерти
        void EntityDeathTask()
        {
            //Выиграл второй игрок
            if (Player1Base.Health <= 0)
            {
                GameOver = "WINNER PLAYER 2";
            }
            //Выиграл первый игрок
            if (Player2Base.Health <= 0)
            {
                GameOver = "WINNER PLAYER 1";
            }
            //все Tank у которых закончилось ХП, один случайный модуль танка выпадает
            List<Item> temp_items = new List<Item>();
            foreach (var tank in GameObjects.OfType<Tank>().Where(o => o.Health <= 0))
            {
                temp_items.Add(
                    new Item()
                    {
                        X = tank.X,
                        Y = tank.Y,
                        Value = random.Next(3) switch
                        {
                            0 => tank.Base,
                            1 => tank.Tower,
                            2 => tank.Cannon,
                        }
                    });
            }
            GameObjects.AddRange(temp_items);
            //удаляем все Entity без ХП
            GameObjects.RemoveAll(o => o.GetType().IsAssignableTo(typeof(Entity)) && ((Entity)o).Health <= 0);
        }
        //работа базы
        void BaseTask()
        {
            func(Player1Base);
            func(Player2Base);
            
            void func(BaseCell @base)
            {
                //если в игре меньше 2 союзных танка и в очереди есть танки на "создание"
                if (GameObjects.OfType<Tank>().Where(t => t.Team == @base.Team).Count() < 1 && @base.TankQueue.Count > 0)
                {
                    //забираем танк из очереди
                    Tank tank = @base.TankQueue.Pop();
                    //для танков врага направление движения
                    if (@base.Team == Entity.ETeam.Enemy) { tank.Course = Tank.ECourse.Down; Player2Tank = tank; }
                    else { tank.Course = Tank.ECourse.Top; Player1Tank = tank; }
                    //и если танку ничего не мешаем ставим его на одну из соседних клеток с базой, без диагональных
                    if (!Crossing<Tank>(tank, @base.X + 1, @base.Y))
                    {
                        tank.X = @base.X + 1;
                        tank.Y = @base.Y;
                        GameObjects.Add(tank);
                    }
                    else if (!Crossing<Tank>(tank, @base.X - 1, @base.Y))
                    {
                        tank.X = @base.X - 1;
                        tank.Y = @base.Y;
                        GameObjects.Add(tank);
                    }
                    else if (!Crossing<Tank>(tank, @base.X, @base.Y + 1))
                    {
                        tank.X = @base.X;
                        tank.Y = @base.Y + 1;
                        GameObjects.Add(tank);
                    }
                    else if (!Crossing<Tank>(tank, @base.X, @base.Y + 1))
                    {
                        tank.X = @base.X;
                        tank.Y = @base.Y - 1;
                        GameObjects.Add(tank);
                    }
                    //если всё занято то возращаем танк в очередь
                    else
                        @base.TankQueue.Push(tank);
                }
            }
        }

        //движение танка
        //передаеться сам танк и направление движения
        void MoveTank(Tank tank, Tank.ECourse course)
        {
            //задаем направление движения
            tank.Course = course;
            float x = tank.X, y = tank.Y;
            //новые координаты танка согласно направлению и скорости
            switch (course)
            {
                case Tank.ECourse.Top: y -= tank.Speed; break;
                case Tank.ECourse.Down: y += tank.Speed; break;
                case Tank.ECourse.Left: x -= tank.Speed; break;
                case Tank.ECourse.Right: x += tank.Speed; break;
            }
            //если по новым координатам танку ничего не мешает, то передвигаем его
            if (!Crossing<IObjectInGame>(tank, x, y))
            {
                tank.X = x;
                tank.Y = y;
            }
        }



        //-----------------------------------
        //нажатия кнопок игроком

        //список нажатых игроком клавиш передвижения
        private List<Tank.ECourse> player1Moves = new List<Tank.ECourse>();
        private List<Tank.ECourse> player2Moves = new List<Tank.ECourse>();

        //нажата ли клавиша Выстрела
        private bool player1Shoot = false;
        private bool player2Shoot = false;
        //подбора предмета
        private bool player1TakeItem = false;
        private bool player2TakeItem = false;
        //Уничтожения предмета
        private bool player1DestroyItem = false;
        private bool player2DestroyItem = false;

        //Обработка нажатых клавишь
        public void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.W: player1Moves.Add(Tank.ECourse.Top); break;
                case System.Windows.Forms.Keys.S: player1Moves.Add(Tank.ECourse.Down); break;
                case System.Windows.Forms.Keys.D: player1Moves.Add(Tank.ECourse.Right); break;
                case System.Windows.Forms.Keys.A: player1Moves.Add(Tank.ECourse.Left); break;
                case System.Windows.Forms.Keys.F: player1Shoot = true; break;
                case System.Windows.Forms.Keys.E: player1TakeItem = true; break;
                case System.Windows.Forms.Keys.R: player1DestroyItem = true; break;

                case System.Windows.Forms.Keys.Up: player2Moves.Add(Tank.ECourse.Top); break;
                case System.Windows.Forms.Keys.Down: player2Moves.Add(Tank.ECourse.Down); break;
                case System.Windows.Forms.Keys.Right: player2Moves.Add(Tank.ECourse.Right); break;
                case System.Windows.Forms.Keys.Left: player2Moves.Add(Tank.ECourse.Left); break;
                case System.Windows.Forms.Keys.Delete: player2Shoot = true; break;
                case System.Windows.Forms.Keys.PageUp: player2TakeItem = true; break;
                case System.Windows.Forms.Keys.PageDown: player2DestroyItem = true; break;
            };
        }
        //Обработка отжатых клавишь
        public void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.W: player1Moves.RemoveAll(c => c == Tank.ECourse.Top); break;
                case System.Windows.Forms.Keys.S: player1Moves.RemoveAll(c => c == Tank.ECourse.Down); break;
                case System.Windows.Forms.Keys.D: player1Moves.RemoveAll(c => c == Tank.ECourse.Right); break;
                case System.Windows.Forms.Keys.A: player1Moves.RemoveAll(c => c == Tank.ECourse.Left); break;
                case System.Windows.Forms.Keys.F: player1Shoot = false; break;
                case System.Windows.Forms.Keys.E: player1TakeItem = false; break;
                case System.Windows.Forms.Keys.R: player1DestroyItem = false; break;

                case System.Windows.Forms.Keys.Up: player2Moves.RemoveAll(c => c == Tank.ECourse.Top); break;
                case System.Windows.Forms.Keys.Down: player2Moves.RemoveAll(c => c == Tank.ECourse.Down); break;
                case System.Windows.Forms.Keys.Right: player2Moves.RemoveAll(c => c == Tank.ECourse.Right); break;
                case System.Windows.Forms.Keys.Left: player2Moves.RemoveAll(c => c == Tank.ECourse.Left); break;
                case System.Windows.Forms.Keys.Delete: player2Shoot = false; break;
                case System.Windows.Forms.Keys.PageUp: player2TakeItem = false; break;
                case System.Windows.Forms.Keys.PageDown: player2DestroyItem = false; break;
            };
        }


        //пересекается ли танк, который стоит по координатам x, y с чем-то
        //T для выбора типа объектов с которыми будем проверять пересечение
        private bool Crossing<T>(Tank tank, float x, float y)
        {
            //выбираем объекты которые можно привести к типу T
            //если любой из выбранных пересекается с танком с учетом коллизии, то возвращаем TRUE

            return GameObjects.Where(o => o.GetType().IsAssignableTo(typeof(T))).Any(e => e != tank && Math.Abs(e.X - x) < (tank.Collision + e.Collision - 1) && Math.Abs(e.Y - y) < (tank.Collision + e.Collision - 1));
        }







        ////A*
        //void BotTask()
        //{
        //    if (bot1.Control is null)
        //        bot1.Control = GameObjects.OfType<Tank>().FirstOrDefault(t => t != PlayerTank);
        //    if (bot1.Target is null && bot1.Control is not null)
        //        bot1.Target = GameObjects.OfType<Tank>().FirstOrDefault(t => t.Team != bot1.Control.Team);
        //    if (bot1.Target is null && bot1.Control is null) return;
        //    bot1.Ways.RemoveAll(q => q.Count <= 0);
        //    if(bot1.Ways.Count == 0)
        //    {
        //        bot1.Ways = Ways(bot1);
        //    }
        //    if(bot1.Ways.Count > 0)
        //    {
        //        MoveTank(bot1.Control, bot1.Ways.First().Course);
        //        bot1.Ways.First().Count--;
        //    }

        //}

        //List<Way> Ways(Bot bot)
        //{
        //    List<Node> nodes = new List<Node>();

        //    Node finishe = new Node() { X = bot.Target.X, Y = bot.Target.Y };
        //    nodes.Add(new Node() { X = bot.Control.X, Y = bot.Control.Y, Opening = true, G = 0 });

        //    nodes[0].H = func_h(nodes[0]);

        //    while (nodes.Any(n => n.Opening))
        //    {
        //        Node curr = nodes.Where(n => n.Opening).OrderBy(n => n.F).First();
        //        if (curr.X == finishe.X && curr.Y == finishe.Y) break;
        //        curr.Opening = false;
        //        foreach (var neighbour in func_neighbours(curr))
        //        {
        //            double temp_g = curr.G + 1;

        //            if (nodes.Contains(neighbour))
        //            {
        //                if (neighbour.G > temp_g)
        //                {
        //                    neighbour.G = temp_g;
        //                    neighbour.Before = curr;
        //                }
        //            }
        //            else
        //            {
        //                neighbour.G = temp_g;
        //                neighbour.Before = curr;
        //                neighbour.H = func_h(neighbour);
        //                nodes.Add(neighbour);
        //            }
        //        }
        //    }
        //    List<Way> way = new List<Way>();
        //    finishe.Before = nodes.Where(n => !n.Opening).OrderBy(n => n.H).First();
        //    Node before = finishe;
        //    while (before.Before != null)
        //    {
        //        Tank.ECourse course;
        //        if (before.X > before.Before.X)
        //            course = Tank.ECourse.Right;
        //        else if (before.X < before.Before.X)
        //            course = Tank.ECourse.Left;
        //        else if (before.Y > before.Before.Y)
        //            course = Tank.ECourse.Down;
        //        else
        //            course = Tank.ECourse.Top;
        //        way.Add(new Way() { Course = course, Count = (int)(0.5F / bot.Control.Speed) });
        //        before = before.Before;
        //    }
        //    return way;


        //    double func_h(Node node)
        //    {
        //        return Math.Sqrt(Math.Pow(node.X - finishe.X, 2) + Math.Pow(node.Y - finishe.Y, 2));
        //    }
        //    List<Node> func_neighbours(Node node)
        //    {
        //        List<Node> neighbours = new List<Node>();
        //        neighbours.Add(func_neighbour(node.X - 0.5F, node.Y));
        //        neighbours.Add(func_neighbour(node.X + 0.5F, node.Y));
        //        neighbours.Add(func_neighbour(node.X, node.Y + 0.5F));
        //        neighbours.Add(func_neighbour(node.X, node.Y - 0.5F));
        //        neighbours.RemoveAll(e => e == null);

        //        return neighbours;
        //    }
        //    Node func_neighbour(float x, float y)
        //    {
        //        Node temp_neighbour = null;
        //        if (!Crossing<IObjectInGame>(bot.Control, x, y) && !nodes.Any(n => !n.Opening && n.X == x && n.Y == y))
        //        {
        //            temp_neighbour = nodes.FirstOrDefault(n => n.Opening && n.X == x && n.Y == y);
        //            if (temp_neighbour == null)
        //                temp_neighbour = new Node() { X = x, Y = y, Opening = true };
        //        }
        //        return temp_neighbour;
        //    }
        //}
    }
}
