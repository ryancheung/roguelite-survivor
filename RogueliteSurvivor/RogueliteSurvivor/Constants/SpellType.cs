using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueliteSurvivor.Constants
{
    public enum SpellType
    {
        None,
        Projectile,
        SingleTarget,
    }

    public static class SpellTypeExtensions
    {
        public static SpellType GetSpellTypeFromString(this string spellTypeString)
        {
            switch (spellTypeString)
            {
                case "None":
                    return SpellType.None;
                case "Projectile":
                    return SpellType.Projectile;
                case "SingleTarget":
                    return SpellType.SingleTarget;
                default:
                    return SpellType.None;
            }
        }
    }
}
