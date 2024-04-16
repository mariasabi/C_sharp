// See https://aka.ms/new-console-template for more information
using WarriorWars.Enum;
namespace WarriorWars
{
    class Program
    {
        static Random rng=new Random();
        static void Main()
        {
            Warrior goodGuy = new Warrior("Mary", Faction.GoodGuy);
            Warrior badGuy = new Warrior("Raj", Faction.BadGuy);
            while(goodGuy.IsAlive && badGuy.IsAlive)
            {
                if(rng.Next(0,10)<5)
                {
                    goodGuy.Attack(badGuy);
                        
                }
                else
                {
                    badGuy.Attack(goodGuy);
                }
                Thread.Sleep(200);
            }
            
        }
    }
}