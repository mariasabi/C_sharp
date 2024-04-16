using WarriorWars.Enum;
using static System.Net.Mime.MediaTypeNames;

namespace WarriorWars.Equip
{
    class Armor
    {

        private const int GOOD_GUY_ARMOR=5;
        private const int BAD_GUY_ARMOR=5;
        private int armorpoints;
        public int Armorpoints
        {
            get
            {
                return armorpoints;
            }
        }
        public Armor(Faction faction)
        {
            switch (faction)
            {
                case Faction.GoodGuy:
                    armorpoints = GOOD_GUY_ARMOR;
                    break;
                case Faction.BadGuy:
                    armorpoints = BAD_GUY_ARMOR;
                    break;
                default:
                    break;
            }
        }
    }
}