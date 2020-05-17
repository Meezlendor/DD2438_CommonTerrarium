using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CreatureBehaviour
{
    class PigReproduction : IReproduction
    {

        public void CreateBaby(Creature parent, ref Creature baby)
        {
            /*
             * This is a demo of how you may finetune your newborn
             * As you may, see mutation happens here !
             */
            baby.CreatureRegime = parent.CreatureRegime;
            baby.Size = parent.Size;
            baby.MaxSpeed = parent.MaxSpeed;
            baby.Sensor = new CircularSensor(parent.Sensor.SensingRadius);
            baby.MaxEnergy = parent.MaxEnergy;
            baby.Generation = parent.Generation + 1;
        }
    }
}
