using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// This class has to be inherithed by all AnimalAI classes
    /// </summary>
    public class CreatureAI : MonoBehaviour
    {
        public string specieName;   // The name of the specie (to be set in the prefab)
        public int specieID;        // The ID of the specie (setted by GameManager)

        /// <summary>
        /// This function is called when your creature is within an acceptable
        /// range to eat some food, adapted to your regime of course.
        /// You do not need to necessarily eat it of course.
        /// </summary>
        /// <param name="food"></param>
        public virtual void OnAccessibleFood(GameObject food)
        {
        }

        /// <summary>
        /// This function is called once per second and specie by GameManager
        /// It is implemented in each AnimalAI to update stats of that specie.
        /// Data are logged csv on a txt file.
        /// </summary>
        public virtual void updateStats()
        {
        }
    }
}