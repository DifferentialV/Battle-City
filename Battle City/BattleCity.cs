using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Battle_City
{

    public partial class BattleCity : Form
    {
        public BattleCity()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Game = new Game.Game();

            //изображения танков и базы
            entitesViews = StaticMethods.entitesViewsInit(Math.Min(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height));
            //изображения Игрового поля
            BattlePlace = StaticMethods.BattlePlaceInit(Math.Min(Screen.GetWorkingArea(this).Width, Screen.GetWorkingArea(this).Height),Game);

            //нажатия клавишь
            KeyDown += new KeyEventHandler(Game.KeyDown);
            KeyUp += new KeyEventHandler(Game.KeyUp);

            //Таймер который запускает процесс обновления состояния игры
            PushTimer = new Timer
            {
                Interval = 1000/30
            };
            //обновление состояния
            PushTimer.Tick += new EventHandler(Game.TimerEventProcessor);
            //обновление изображения на экране
            PushTimer.Tick += new EventHandler(UpdateGraphics);
            PushTimer.Start();
        }
        //Таймер который запускает процесс обновления состояния игры
        Timer PushTimer;

        Game.Game Game;
        //изображения Игрового поля
        Bitmap BattlePlace;
        //изображения танков и базы
        Dictionary<string, Bitmap> entitesViews;

        //обновление изображения
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphic = e.Graphics;

            //размер ячейки такой чтобы Игровое поле заняло все окно
            int cell_size;
            //положение Игрового поля
            Point nullpoint;
            if(ClientSize.Width < ClientSize.Height)
            {
                cell_size = ClientSize.Width / Battle_City.Game.Game.square_cell;
                nullpoint = new Point(0, (ClientSize.Height - ClientSize.Width)/2);
            }
            else
            {
                cell_size = ClientSize.Height / Battle_City.Game.Game.square_cell;
                nullpoint = new Point((ClientSize.Width - ClientSize.Height) / 2, 0);
            }
            
            //Изображение Игрового поля
            graphic.DrawImage(BattlePlace, nullpoint.X, nullpoint.Y, cell_size * Battle_City.Game.Game.square_cell, cell_size * Battle_City.Game.Game.square_cell);

            //Изображения танков
            foreach(var tank in Game.GameObjects.OfType<Battle_City.Game.Entities.Tank>())
            {
                //рисуем танк
                Bitmap bitmap = TankView(tank);
                graphic.DrawImage(bitmap, (int)(nullpoint.X + cell_size * tank.X), (int)(nullpoint.Y + cell_size * tank.Y), cell_size, cell_size);
                //если танк Стреляет то рисуем Линии выстрела
                if(tank.Shooting)
                {
                    Pen pen = new Pen(tank.Team == Battle_City.Game.Entities.Entity.ETeam.Player ? Color.Red : Color.Blue);
                    int x = (int)(nullpoint.X + cell_size * (tank.X+0.5));
                    int y = (int)(nullpoint.Y + cell_size * (tank.Y+0.5));
                    int range = (int)(cell_size * tank.CannonRange);
                    int ds = (int)(cell_size * 0.06);
                    if (tank.DoubleShot)
                    {
                        switch (tank.Course)
                        {
                            case Battle_City.Game.Entities.Tank.ECourse.Top: graphic.DrawLine(pen, x - ds, y, x - ds, y - range); graphic.DrawLine(pen, x + ds, y, x + ds, y - range); break;
                            case Battle_City.Game.Entities.Tank.ECourse.Down: graphic.DrawLine(pen, x - ds, y, x - ds, y + range); graphic.DrawLine(pen, x + ds, y, x + ds, y + range); break;
                            case Battle_City.Game.Entities.Tank.ECourse.Left: graphic.DrawLine(pen, x, y - ds, x - range, y - ds); graphic.DrawLine(pen, x, y + ds, x - range, y + ds); break;
                            case Battle_City.Game.Entities.Tank.ECourse.Right: graphic.DrawLine(pen, x, y - ds, x + range, y - ds); graphic.DrawLine(pen, x, y + ds, x + range, y + ds); break;
                        }
                    }
                    else
                    {
                        switch (tank.Course)
                        {
                            case Battle_City.Game.Entities.Tank.ECourse.Top: graphic.DrawLine(pen, x, y, x, y - range); break;
                            case Battle_City.Game.Entities.Tank.ECourse.Down: graphic.DrawLine(pen, x, y, x, y + range); break;
                            case Battle_City.Game.Entities.Tank.ECourse.Left: graphic.DrawLine(pen, x, y, x - range, y); break;
                            case Battle_City.Game.Entities.Tank.ECourse.Right: graphic.DrawLine(pen, x, y, x + range, y); break;
                        }
                    }
                }
            }
            //базы команд
            foreach (var @base in Game.GameObjects.OfType<Battle_City.Game.Entities.BaseCell>())
            {
                Bitmap bitmap = @base.Team == Battle_City.Game.Entities.Entity.ETeam.Player ? entitesViews["basePlayer"] : entitesViews["baseEnemy"];
                graphic.DrawImage(bitmap, (int)(nullpoint.X + cell_size * @base.X), (int)(nullpoint.Y + cell_size * @base.Y), cell_size, cell_size);
            }
            //предметы рисуються просто текстом
            foreach(var item in Game.GameObjects.OfType<Battle_City.Game.NonEntities.Item>())
            {
                Bitmap bitmap = new Bitmap(26, 26);
                Graphics temp_graphic = Graphics.FromImage(bitmap);
                temp_graphic.DrawString(item.Value.ToString(), new Font("Arial", 9), new SolidBrush(Color.Purple), 0, 0);
                graphic.DrawImage(bitmap, (int)(nullpoint.X + cell_size * item.X), (int)(nullpoint.Y + cell_size * item.Y), cell_size, cell_size);
            }
            //сообщение о завершении игры
            if(!string.IsNullOrEmpty(Game.GameOver))
            {
                PushTimer.Stop();
                Bitmap bitmap = new Bitmap(400, 100);
                Graphics temp_graphic = Graphics.FromImage(bitmap);
                temp_graphic.DrawString(Game.GameOver, new Font("Arial", 26), new SolidBrush(Color.Purple), 0, 0);
                graphic.DrawImage(bitmap, nullpoint.X, nullpoint.Y, cell_size * Battle_City.Game.Game.square_cell, cell_size * Battle_City.Game.Game.square_cell);
            }
        }

        //выберает танк из списка entitesViews и поворачивает согласно Course
        Bitmap TankView(Game.Entities.Tank tank)
        {
            return tank.Course switch
            {
                Battle_City.Game.Entities.Tank.ECourse.Top => StaticMethods.RotateImage(entitesViews[$"tank{tank.Team}{tank.Base}{tank.Tower}{tank.Cannon}"], 90),
                Battle_City.Game.Entities.Tank.ECourse.Right => StaticMethods.RotateImage(entitesViews[$"tank{tank.Team}{tank.Base}{tank.Tower}{tank.Cannon}"], 180),
                Battle_City.Game.Entities.Tank.ECourse.Down => StaticMethods.RotateImage(entitesViews[$"tank{tank.Team}{tank.Base}{tank.Tower}{tank.Cannon}"], 270),
                _ => entitesViews[$"tank{tank.Team}{tank.Base}{tank.Tower}{tank.Cannon}"]
            };
        }
        //вызываеться по таймеру или при изменении размера окна
        private void UpdateGraphics(object sender, EventArgs e)
        {
            this.Invalidate();
            GC.Collect();
        }

        //нажатие паузы
        private void BattleCity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 'p')
            {
                if (PushTimer.Enabled)
                    PushTimer.Stop();
                else
                    PushTimer.Start();
            }
        }
    }

    //Create by Volodymyr Horlan
}
