using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterMeowmere
{
    public class ModConfig
    {
        public string ProjectileSound { get; set; } = "All";
        public bool ProjectileIsSecondaryAttack { get; set; } = true;
        public int AttackDamage { get; set; } = 20;
    }
}
