using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battle_City.Game.Entities
{
    //класс хранит состояние танка
    public class Tank : Entity
    {
        //Направления движения
        public enum ECourse
        {
            Top,
            Down,
            Left,
            Right
        }
        //корпусы танка
        public enum EBase
        {
            B1,
            B2,
            B3,
        }
        //оружия
        public enum ECannon
        {
            FC1,
            FC2,
            FC3,
            PC1,
            PC2,
            PC3,
        }
        //башни
        public enum ETower
        {
            T1,
            T2,
            T3,
        }
        public Tank(EBase @base, ECannon cannon, ETower tower,ETeam team)
        {
            SetBase(@base);
            SetCannon(cannon);
            SetTower(tower);
            Team = team;
            //по умолчанию танк смотрит на верх
            Course = ECourse.Top;
            //коллизия 
            Collision = 0.9F;
        }
        //установка корпуса
        public void SetBase(EBase @base)
        {
            Base = @base;
            //ХП танка
            Health = @base switch
            {
                EBase.B1 => 2,
                EBase.B2 => 4,
                EBase.B3 => 6,
                _=>1
            };
            //скорость с корпусом первого уровня
            const float max_speed = 0.3F; 
            Speed = @base switch
            {
                EBase.B1 => max_speed,
                EBase.B2 => max_speed/1.5F,
                EBase.B3 => max_speed/2,
                _ => 1
            };
        }
        //установка башни
        public void SetTower(ETower tower)
        {
            Tower = tower;
            //точность
            Accuracy = tower switch
            {
                ETower.T1 => 70,
                ETower.T2 => 80,
                ETower.T3 => 90,
                _ => 100
            };
        }
        //установка оружия
        public void SetCannon(ECannon cannon)
        {
            Cannon = cannon;
            //Урон, Максимальная дальность выстрела, одиночный двойной выстрел
            switch (cannon)
            {
                case ECannon.FC1:Damage = 1;CannonMaxRange = 3;DoubleShot = true; break;
                case ECannon.FC2:Damage = 2;CannonMaxRange = 4;DoubleShot = true; break;
                case ECannon.FC3:Damage = 3;CannonMaxRange = 5;DoubleShot = true;  break;
                case ECannon.PC1:Damage = 2;CannonMaxRange = 4; DoubleShot = false; break;
                case ECannon.PC2:Damage = 4;CannonMaxRange = 6; DoubleShot = false; break;
                case ECannon.PC3:Damage = 6;CannonMaxRange = 8; DoubleShot = false; break;
            }
        }
        //направление  движения
        public ECourse Course { get; set; }
        //корпус
        public EBase Base { get; set; }
        public ECannon Cannon { get; set; }
        public ETower Tower { get; set; }

        public float Speed { get; private set; }
        public int Damage { get; private set; }
        public bool DoubleShot { get; private set; }
        public float Accuracy { get; private set; }

        //стреляет ли сейчас танк
        public bool Shooting { get; set; } = false;

        public float CannonMaxRange { get; private set; }
        //на какое растояние сейчас стреляет танк
        public float CannonRange { get; set; }

        //перезарядка
        public const int CannonCooldownConst = 10;
        //оставшаяся перезарядка
        public int CannonCooldown { get; set; } = 0;

    }
}
