using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class CanivorousMooMooAI : CreatureAI
    {


        private Creature creature;

        public void Start()
        {
            this.creature = GetComponent<Creature>();
        }

        public void Update()
        {
            Vector3 closest = Vector3.zero;
            var preys = creature.Sensor.SensePreys(creature);
            foreach(var prey in preys)
            {
                if (Vector3.Distance(prey.transform.position, transform.position) < Vector3.Distance(closest, transform.position))
                    closest = prey.transform.position;
            }
            creature.Move(closest - transform.position, 1f);
            creature.Sensor.SensePreys(creature);
        }

        public override void OnAccessibleFood(GameObject food)
        {
            creature.Eat(food);
            Debug.Log($"Moo moo eat !");
            //food.transform.position = new Vector3(food.transform.position.x, 100, food.transform.position.z);
            if (creature.Energy > 200)
                creature.Reproduce();
        }
    }
}
