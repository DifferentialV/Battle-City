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
        public const int square_cell = 18;
        //список объектов в игре
        public List<IObjectInGame> GameObjects;
        //строка оповещения и завершении игры
        public string GameOver { get; private set; } = null;
        public Tank PlayerTank;
        public BaseCell PlayerBase;
        public BaseCell EnemyBase;
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
            for (int i = 1; i < square_cell-1; i++)
            {
                for (int j = i> square_cell - 7 ? 6:1; j < square_cell-(i<6? 6 : 1); j++)
                {
                    if(random.Next(100) - 80 >= 0)
                    {
                        GameObjects.Add(new Stone() { X = j, Y = i, Type = random.Next() % 2 == 0 ? Stone.TypeCell.stone1 : Stone.TypeCell.stone2 });
                    }
                }
            }
            //создаем базы
            //базы устанавливаем по координатам и заполняем очередь на создания танков
            PlayerBase = new BaseCell(Entity.ETeam.Player) { X = 3, Y = square_cell - 4,TankQueue = StaticMethods.RandomTanks(Entity.ETeam.Player) };
            EnemyBase = new BaseCell(Entity.ETeam.Enemy) { X = square_cell - 4, Y = 3, TankQueue = StaticMethods.RandomTanks(Entity.ETeam.Enemy) };
            
            //Первым танком управляет игрок
            PlayerTank = PlayerBase.TankQueue.Pop();
            PlayerTank.X = 4;
            PlayerTank.Y = square_cell - 4;



            GameObjects.Add(PlayerBase);
            GameObjects.Add(EnemyBase);
            GameObjects.Add(PlayerTank);

            //PlayerTank.SetBase(Tank.EBase.B1);
            //PlayerTank.SetCannon(Tank.ECannon.PC1);

            GameObjects.Add(new Item() { Value = Tank.EBase.B2, X = 4, Y = 2 });
            GameObjects.Add(new Item() { Value = Tank.ECannon.PC2, X = 4, Y = 4 });
        }

        //Вызывает по таймеру из основной класс BattleCity
        public void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            //танк движется по направлению, которое соответствует последней нажатой клавише
            if (playerMoves.Count > 0)
                MoveTank(PlayerTank, playerMoves.Last());

            //Стрельба, ниже будет вызвана функция, которая обработает выстрел
            if (playerShoot)
            {
                PlayerTank.Shooting = true;
            }
            //игрок подбирает Модуль
            //Выбираться первый Модуль, с которым пересекается моделька игрока
            if (playerTakeItem)
            {
                Item item = GameObjects.OfType<Item>().FirstOrDefault(i => Math.Abs(i.X - PlayerTank.X) < PlayerTank.Collision && Math.Abs(i.Y - PlayerTank.Y) < PlayerTank.Collision);
                if(item != null)
                {
                    if (item.Value.GetType() == typeof(Tank.EBase)) PlayerTank.SetBase(item.Value);
                    if (item.Value.GetType() == typeof(Tank.ETower)) PlayerTank.SetTower(item.Value);
                    if (item.Value.GetType() == typeof(Tank.ECannon)) PlayerTank.SetCannon(item.Value);
                }
                GameObjects.Remove(item);
            }
            //игрок уничтожает Модуль
            //уничтожается первый Модуль, с которым пересекается моделька игрока
            if (playerDestroyItem)
            {
                Item item = GameObjects.OfType<Item>().FirstOrDefault(i => Math.Abs(i.X - PlayerTank.X) < PlayerTank.Collision && Math.Abs(i.Y - PlayerTank.Y) < PlayerTank.Collision);
                if (item != null)
                    GameObjects.Remove(item);
            }

            //Сделать расчет Выстрелов, Смертей, Работа Баз
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
            //если убили игрока или его базу то Игра завершаеться
            if (PlayerBase.Health <= 0 || PlayerTank.Health <= 0)
            {
                GameOver = "Game Over";
            }
            //"погибла" вражеская база то  Игра завершаеться победой
            if (EnemyBase.Health <= 0)
            {
                GameOver = "YOU WINNER";
            }
            //все Tank у которых закончилось ХП, один случайный модуль танка выпадает
            foreach (var tank in GameObjects.OfType<Tank>().Where(o => o.Health <= 0))
            {
                GameObjects.Add(
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
            //удаляем все Entity без ХП
            GameObjects.RemoveAll(o => o.GetType().IsAssignableTo(typeof(Entity)) && ((Entity)o).Health <= 0);
        }
        //работа базы
        void BaseTask()
        {
            func(PlayerBase);
            func(EnemyBase);
            
            void func(BaseCell @base)
            {
                //если в игре меньше 2 союзных танка и в очереди есть танки на "создание"
                if (GameObjects.OfType<Tank>().Where(t => t.Team == @base.Team).Count() < 2 && @base.TankQueue.Count > 0)
                {
                    //забираем танк из очереди
                    Tank tank = @base.TankQueue.Pop();
                    //для танков врага направление движения
                    if (@base.Team == Entity.ETeam.Enemy) { tank.Course = Tank.ECourse.Down; }
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
        private List<Tank.ECourse> playerMoves = new List<Tank.ECourse>();

        //нажата ли клавиша Выстрела
        private bool playerShoot = false;
        //подбора предмета
        private bool playerTakeItem = false;
        //Уничтожения предмета
        private bool playerDestroyItem = false;

        //Обработка нажатых клавишь
        public void KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.W: playerMoves.Add(Tank.ECourse.Top); break;
                case System.Windows.Forms.Keys.S: playerMoves.Add(Tank.ECourse.Down); break;
                case System.Windows.Forms.Keys.D: playerMoves.Add(Tank.ECourse.Right); break;
                case System.Windows.Forms.Keys.A: playerMoves.Add(Tank.ECourse.Left); break;
                case System.Windows.Forms.Keys.Space: playerShoot = true; break;
                case System.Windows.Forms.Keys.Z: playerTakeItem = true; break;
                case System.Windows.Forms.Keys.M: playerDestroyItem = true; break;
            };
        }
        //Обработка отжатых клавишь
        public void KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.W: playerMoves.RemoveAll(c => c == Tank.ECourse.Top); break;
                case System.Windows.Forms.Keys.S: playerMoves.RemoveAll(c => c == Tank.ECourse.Down); break;
                case System.Windows.Forms.Keys.D: playerMoves.RemoveAll(c => c == Tank.ECourse.Right); break;
                case System.Windows.Forms.Keys.A: playerMoves.RemoveAll(c => c == Tank.ECourse.Left); break;
                case System.Windows.Forms.Keys.Space: playerShoot = false; break;
                case System.Windows.Forms.Keys.Z: playerTakeItem = false; break;
                case System.Windows.Forms.Keys.M: playerDestroyItem = false; break;
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
    }
}
