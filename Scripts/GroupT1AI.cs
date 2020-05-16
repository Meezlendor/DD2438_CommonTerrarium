using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class GroupT1AI : CreatureAI
    {
        private Creature creature;

        private Trace trace;

        private float elapsedTime;
        private Vector3 direction;

        public void Start()
        {
            creature = GetComponent<Creature>();
            trace = GameObject.FindGameObjectWithTag("trace").GetComponent<Trace>();
            elapsedTime = 0;
            direction = Vector3.forward;
        }

        public void Update()
        {
            creature.Move(direction, 1f);
            elapsedTime += Time.deltaTime;
            var myCell = trace.GetCellPos(transform.position);
            //always leave a small trace of where you are located
            //trace.food[myCell.Item1, myCell.Item2] += 0.01f;


            //time to make a new decision ?
            if (elapsedTime > 1)
            {
                elapsedTime = 0;
            }
            else
                return;

            //update the danger signal
            var neighbors = creature.Sensor.SensePredators(creature);
            /*foreach(var neighbor in neighbors)
            {
                var dangerCell = trace.GetCellPos(neighbor.transform.position);
                trace.danger[dangerCell.Item1, dangerCell.Item2] += 2f;
            }*/
            trace.danger[myCell.Item1, myCell.Item2] += neighbors.Count;
            int sense = neighbors.Count;

            //update the food
            var plants = creature.Sensor.SensePlants(creature);
            GameObject closestFood = plants.Count==0 ? null : plants[0];
            foreach(var food in plants)
            {
                if (Vector3.Distance(food.transform.position, transform.position) <= Vector3.Distance(transform.position, closestFood.transform.position))
                {
                    closestFood = food;
                }
                var foodCell = trace.GetCellPos(food.transform.position);
                //trace.food[foodCell.Item1, foodCell.Item2] += 1f;
                trace.food[myCell.Item1, myCell.Item2] += 1f;
            }

            sense += plants.Count;
            if (sense == 0)
                trace.danger[myCell.Item1, myCell.Item2] = 0.5f;//discourage going here, nothing yet

            // weight all surrounding cells and accept one randomly
            float[] weightedProbs = new float[9];
            float sum=0f;
            for(int i=-1;i<2; i++)
            {
                for(int j=-1; j<2; j++)
                {
                    int cellx = i + myCell.Item1;
                    int cellz = j + myCell.Item2;
                    if (cellx>=0 && cellz>=0 && cellx<trace.food.GetLength(0) && cellz < trace.food.GetLength(1))
                    {
                        float risk=0;
                        if (trace.food[cellx, cellz] != 0)
                            risk = trace.food[cellx, cellz] / (trace.food[cellx, cellz] + trace.danger[cellx, cellz]);
                        else if (trace.danger[cellx, cellz] == 0)//no danger, yet unexplored, give some credit
                            risk = 0.5f;
                        weightedProbs[(i + 1) * 3 + (j + 1)] = risk;
                        sum += risk;
                    }
                }
            }
            // U is a uniform variable to pick up around the 
            float u = UnityEngine.Random.Range(0f, 1f);
            float localSum = 0f;
            int chosenCell;
            for(chosenCell=0;chosenCell<weightedProbs.Length && localSum<u; chosenCell++)
            {
                localSum += weightedProbs[chosenCell] / sum;
            }
            chosenCell--;
            int targetCellx = chosenCell / 3 - 1 + myCell.Item1;
            int targetCellz = chosenCell % 3 - 1 + myCell.Item2;
            if (targetCellx == myCell.Item1 && targetCellz == myCell.Item2)
                direction = Vector3.zero;
            else
                direction = trace.GetVectorPos(targetCellx, targetCellz) - transform.position;

            // do we have a close piece of food ?
            if (closestFood != null)
            {
                //reach it depending on the associated danger
                var foodCell = trace.GetCellPos(closestFood.transform.position);
                float risk = trace.danger[foodCell.Item1, foodCell.Item2] / (trace.food[foodCell.Item1, foodCell.Item2] + trace.danger[foodCell.Item1, foodCell.Item2]);
                if (UnityEngine.Random.Range(0f, 1f) > risk)
                {
                    //go to the food and accept risk
                    direction = (closestFood.transform.position - transform.position);
                }
                //if not choosing this risk, then go elsewhere (previous code)
            }

        }

        public override void OnAccessibleFood(GameObject food)
        {
            creature.Eat(food);
        }

        public override void updateStats()
        {
            float totalEnergy=0;
            int totalSpecies = 0;
            foreach(var animal in GameObject.FindGameObjectsWithTag("herbivore"))
            {
                if (animal.GetComponent<CreatureAI>().specieID == this.specieID)
                {
                    totalSpecies++;
                    totalEnergy += animal.GetComponent<Creature>().Energy;
                }
            }
            var text = GameObject.Find("Text");
            if (text != null)
                text.GetComponent<Text>().text = $"Count :{totalSpecies}, avg Energy : {totalEnergy / totalSpecies}";
        }
    }
}
