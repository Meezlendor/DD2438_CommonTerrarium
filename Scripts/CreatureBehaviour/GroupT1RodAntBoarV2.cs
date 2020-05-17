using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CreatureBehaviour
{
    class GroupT1RodAntBoarV2 : CreatureAI
    {
        private Creature creature;

        private Trace trace;

        private float elapsedTime;
        private Vector3 direction;

        private float lastDanger = 0f, lastFood = 0f;
        public float rememberanceFactor = 0.95f;

        // Specie's stats (shared by all the speciemen)
        static int nOfSpeciemens;
        static float avgSize;
        static float avgSpeed;
        static float avgSensing;
        static float avgGeneration;
        static float avgEnergy;


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

            float foodTrace;
            float dangerTrace;

            //update the danger signal
            var neighbors = creature.Sensor.SensePredators(creature);
            foreach (var neighbor in neighbors)
                Debug.DrawLine(transform.position, neighbor.transform.position, Color.red);
            dangerTrace = 2 * neighbors.Count;
            //update the food
            var plants = creature.Sensor.SensePlants(creature);
            GameObject closestFood = plants.Count == 0 ? null : plants[0];
            foreach (var food in plants)
            {
                Debug.DrawLine(food.transform.position, transform.position, Color.blue);
                if (Vector3.Distance(food.transform.position, transform.position) <= Vector3.Distance(transform.position, closestFood.transform.position))
                {
                    closestFood = food;
                }
                var foodCell = trace.GetCellPos(food.transform.position);
            }
            foodTrace = plants.Count;
            if (plants.Count == 0)
                dangerTrace += 0.5f;
            if (neighbors.Count != 0)
                lastDanger = dangerTrace;
            lastFood = rememberanceFactor * lastFood+(1-rememberanceFactor)*foodTrace;
            lastDanger = rememberanceFactor * lastDanger + (1 - rememberanceFactor) * dangerTrace;




            trace.AddFoodTrace(myCell.Item1, myCell.Item2, lastFood);
            trace.AddDangerTrace(myCell.Item1, myCell.Item2, lastDanger);



            // do we have a close piece of food ?
            if (closestFood != null)
            {
                //reach it depending on the associated danger
                var foodCell = trace.GetCellPos(closestFood.transform.position);
                float risk = trace.danger[foodCell.Item1, foodCell.Item2] / (trace.food[foodCell.Item1, foodCell.Item2] + trace.danger[foodCell.Item1, foodCell.Item2]);
                if (trace.danger[foodCell.Item1, foodCell.Item2] < 0.1f && trace.food[foodCell.Item1, foodCell.Item2] < 0.1f)
                    risk = 0f;
                if (UnityEngine.Random.Range(0f, 1f) > risk)// || creature.Energy<creature.EnergyManager.EatingReward(closestFood,creature.CreatureRegime))
                {
                    //go to the food and accept risk
                    direction = (closestFood.transform.position - transform.position);
                    return;
                }
                //if not choosing this risk, then go elsewhere (previous code)
            }

            if (elapsedTime < 1)
                return;
            elapsedTime = 0;

            // Go to the gravity center of attraction/repulsion
            Vector3 gCenter = Vector3.zero;
            int count = 0;
            bool explore = UnityEngine.Random.Range(0f, 1f) < 0.05f;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int cellx = i + myCell.Item1;
                    int cellz = j + myCell.Item2;
                    if (cellx >= 0 && cellz >= 0 && cellx < trace.food.GetLength(0) && cellz < trace.food.GetLength(1))
                    {
                        Vector3 pos = trace.GetVectorPos(cellx, cellz);
                        if (explore && trace.food[cellx, cellz] < 0.1f && trace.danger[cellx, cellz] < 0.1f)
                        {
                            gCenter += (pos - transform.position).normalized;
                            count++;
                        }
                        else
                        {
                            gCenter += (pos - transform.position).normalized * (trace.food[cellx, cellz] - trace.danger[cellx, cellz]);
                            count++;
                        }
                    }
                }
            }
            direction = gCenter / count;

        }

        public override void OnAccessibleFood(GameObject food)
        {
            if (creature.Energy > 4 * creature.EnergyManager.EatingReward(food, creature.CreatureRegime))
            {
                creature.Reproduce();
                creature.Eat(food);
            }
            else
                creature.Eat(food);
        }

        public override void updateStats()
        {
            List<GameObject> agents = GameObject.FindGameObjectsWithTag("carnivore").ToList();
            agents.AddRange(GameObject.FindGameObjectsWithTag("herbivore").ToList());

            agents = agents.FindAll(c => c.GetComponent<CreatureAI>().specieID == specieID);

            nOfSpeciemens = agents.Count;

            avgSensing = 0;
            avgSize = 0;
            avgSpeed = 0;
            avgGeneration = 0;
            avgEnergy = 0;

            foreach (GameObject agent in agents)
            {
                Creature c = agent.GetComponent<Creature>();

                avgSensing += c.Sensor.SensingRadius;
                avgSize += c.Size;
                avgSpeed += c.MaxSpeed;
                avgGeneration += c.Generation;
                avgEnergy += c.Energy;
            }

            avgSensing = avgSensing / (float)nOfSpeciemens;
            avgEnergy = avgEnergy / (float)nOfSpeciemens;
            avgSize = avgSize / (float)nOfSpeciemens;
            avgSpeed = avgSpeed / (float)nOfSpeciemens;
            avgGeneration = ((float)avgGeneration) / (float)nOfSpeciemens;

            Debug.Log(specieName + " " + nOfSpeciemens + " avgSize=" + avgSize + " avgSensing=" + avgSensing + " avgSpeed=" + avgSpeed + " avgGeneration=" + avgGeneration);

            string[] line = { Time.time.ToString() + ";" + avgSensing.ToString() + ";" + avgEnergy.ToString() + ";" + avgSize.ToString() + ";" + avgSpeed.ToString() + ";" + avgGeneration.ToString() + ";" + nOfSpeciemens.ToString() + ";" };
            string docPath = Path.GetFullPath("Assets/Logs/");
            File.AppendAllLines(Path.Combine(docPath, "OutcomesPigT1.csv"), line);
        }
    }
}
