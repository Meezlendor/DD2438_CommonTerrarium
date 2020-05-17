using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Trace : MonoBehaviour
    {
        [NonSerialized]
        public float[,] food;
        [NonSerialized]
        public float[,] danger;

        private float xMin, xMax, zMin, zMax;

        public int nXCells, nZCells;

        public float evaporation;

        private float elapsedTime;
        public float updateFrequency;

        public void Start()
        {
            elapsedTime = 0;
            updateFrequency = updateFrequency == 0 ? 1f : updateFrequency;
            foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name == "mod_ground_forrest")
                {
                    Debug.Log($"I got {obj}");
                    Bounds b = obj.GetComponent<MeshFilter>().mesh.bounds;
                    Debug.Log($"Dims are : {b.min}, {b.center}, {b.max}");
                    Debug.Log($"Parent's scale is {obj.transform.parent.transform.localScale} / ({obj.transform.parent.name})");
                    Vector3 scale = obj.transform.parent.transform.localScale;
                    xMin = b.min.x * scale.x;
                    xMax = b.max.x * scale.x;
                    zMin = b.min.z * scale.z;
                    zMax = b.max.z * scale.z;
                }
                else
                {
                    Debug.Log($"Studying object : {obj}, {obj.name}");
                }
            }
            food = new float[nXCells, nZCells];
            danger = new float[nXCells, nZCells];
            for(int i=0; i<nXCells; i++)
            {
                for(int j=0; j<nZCells; j++)
                {
                    food[i, j] = danger[i, j] = 0;
                }
            }

            Vector3 topL = new Vector3(xMin, 0, zMax);
            Vector3 topR = new Vector3(xMax, 0, zMax);
            Vector3 bottomL = new Vector3(xMin, 0, zMin);
            Vector3 bottomR = new Vector3(xMax, 0, zMin);
            Debug.DrawLine(topL, bottomR, Color.black, 100f);
            Debug.DrawLine(topR, bottomL, Color.black, 100f);

            evaporation = Mathf.Clamp(evaporation, 0f, 1f);
        }

        public void Update()
        {

            float factor = Mathf.Pow(1 - evaporation, Time.deltaTime);
            for (int i = 0; i < food.GetLength(0); i++)
            {
                for (int j = 0; j < food.GetLength(1); j++)
                {
                    food[i, j] = factor * food[i, j];
                    danger[i, j] = factor * danger[i, j];
                }
            }

        }

        public Vector3 GetVectorPos(int i, int j)
        {
            float x = Mathf.Lerp(xMin, xMax, i / (float)nXCells)+(xMax-xMin)/(2*nXCells);
            float z = Mathf.Lerp(zMin, zMax, j / (float)nZCells) + (zMax - zMin) / (2 * nZCells);
            return new Vector3(x, 0, z);
        }

        public Tuple<int,int> GetCellPos(Vector3 v)
        {
            int i = (int)Mathf.Lerp(0, nXCells, (v.x-xMin) / (xMax - xMin));
            int j = (int)Mathf.Lerp(0, nZCells, (v.z - zMin) / (zMax - zMin));
            return new Tuple<int, int>(Mathf.Clamp(i,0,nXCells-1), Mathf.Clamp(j,0,nZCells-1));
        }

        public void AddFoodTrace(int i, int j, float value)
        {
            value = value < 0 ? 0 : value*Time.deltaTime;
            i = Mathf.Clamp(i,0, nXCells-1);
            j = Mathf.Clamp(j,0, nZCells-1);
            food[i, j] += value;
            food[i, j] = Mathf.Min(food[i, j], 10);
        }

        public void AddDangerTrace(int i, int j, float value)
        {
            value = value < 0 ? 0 : value * Time.deltaTime;
            i = Mathf.Clamp(i, 0, nXCells - 1);
            j = Mathf.Clamp(j, 0, nZCells - 1);
            danger[i, j] += value;
            danger[i, j] = Mathf.Min(danger[i, j], 10);
        }

        public void OnDrawGizmosSelected()
        {
            for(int i=0; i<food.GetLength(0); i++)
            {
                for(int j=0; j < food.GetLength(1); j++)
                {
                    if (food[i, j] < 0.1f && danger[i, j] < 0.1f)
                    {
                        continue;
                    }
                    var pos = GetVectorPos(i, j);
                    Gizmos.color = new Color(danger[i,j]/10, food[i,j]/10, 0f);
                    Gizmos.DrawSphere(pos, 2);
                }
            }
        }
    }
}
