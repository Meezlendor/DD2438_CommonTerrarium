using Assets.Scripts.CreatureBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class PigCreature: Creature
    {
        public override void Start()
        {
            base.Start();
            this.Reproducer = new PigReproduction();
        }
    }
}
