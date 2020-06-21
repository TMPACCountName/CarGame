using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace CarGame
{
    /// <summary>
    /// This script spawns objects around a central object such as the player. Spawned objects can be for example Items, or enemy cars, or obstacles.
    /// </summary>
    public class SpawnAroundObject : MonoBehaviour
    {
        static GameController gameController;

        [Tooltip("Тег того вокруг кого спавнить объекты")]
        public string spawnAroundTag = "Player";
        internal Transform spawnAroundObject;

        [Tooltip("Если True то спавним объекты")]
        public bool isSpawning = false;

        [System.Serializable]
        public class SpawnGroup
        {
            [Tooltip("Список объектов группы")]
            public Transform[] spawnObjects;

            [Tooltip("Скорость появления в секундах")]
            public float spawnRate = 5;
            internal float spawnRateCount = 0;
            internal int spawnIndex = 0;

            [Tooltip("Расстояние на котором этот объект порождается относительно spawnAroundObject")]
            public Vector2 spawnObjectDistance = new Vector2(10, 20);
        }

        [Tooltip("Спавн группы, машики ментов, камни деньги и т.д.")]
        public SpawnGroup[] spawnGroups;

        internal int index;

        private void Start()
        {
            // доступ к контроллеру
            if (gameController == null) gameController = GameObject.FindObjectOfType<GameController>();
        }

        void Update()
        {
            // Если не передан объект для спавна, то обозначит его спавн тегом
            if (spawnAroundObject == null && spawnAroundTag != string.Empty && GameObject.FindGameObjectWithTag(spawnAroundTag)) 
                spawnAroundObject = GameObject.FindGameObjectWithTag(spawnAroundTag).transform;

            // Выход если спавнить не надо
            if (!isSpawning) return;

            // Пройти по всем групам спавна и их отсчету 
            for (index = 0; index < spawnGroups.Length; index++)
            {
                // Если есть объекты для спанва
                if (spawnGroups[index].spawnObjects.Length > 0)
                {
                    // Обратный отсчет для появления следующего объекта
                    if (spawnGroups[index].spawnRateCount > 0) 
                        spawnGroups[index].spawnRateCount -= Time.deltaTime;
                    else
                    {
                        // Заспавнить следующий объект в выбранной группе
                        Spawn(spawnGroups[index].spawnObjects, spawnGroups[index].spawnIndex, spawnGroups[index].spawnObjectDistance);

                        // Переход к следующем элементу в группе
                        spawnGroups[index].spawnIndex++;

                        // Сброс если доходим до конца
                        if (spawnGroups[index].spawnIndex > spawnGroups[index].spawnObjects.Length - 1) spawnGroups[index].spawnIndex = 0;

                        // Ресет счетчика
                        spawnGroups[index].spawnRateCount = spawnGroups[index].spawnRate;
                    }

                }
            }
        }

        /// <summary>
        /// Spawns an object based on the index chosen from the array
        /// </summary>
        /// <param name="spawnArray"></param>
        /// <param name="spawnIndex"></param>
        /// <param name="spawnGap"></param>
        public void Spawn(Transform[] spawnArray, int spawnIndex, Vector2 spawnGap)
        {
            // If the array is empty, don't continue
            if (spawnArray[spawnIndex] == null) return;

            // Create a new Object spawn based on the index which loops in the list
            Transform newSpawn = Instantiate(spawnArray[spawnIndex]) as Transform;

            // Spawn an Object at the target position
            if (spawnAroundObject) newSpawn.position = spawnAroundObject.transform.position;// + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

            // Rotate the object randomly, and then move it forward to a random distance from the spawn point
            newSpawn.eulerAngles = Vector3.up * Random.Range(0, 360);
            newSpawn.Translate(Vector3.forward * Random.Range(spawnGap.x, spawnGap.y), Space.Self);

            // Then rotate it back to face the spawn point
            newSpawn.eulerAngles += Vector3.up * 180;

            // Position the object at the same height as the target spawn point
            //if (spawnAroundObject) newSpawn.position += Vector3.up * spawnAroundObject.position.y;

            RaycastHit hit;

            if (Physics.Raycast(newSpawn.position + Vector3.up * 5, -10 * Vector3.up, out hit, 100, gameController.groundLayer)) newSpawn.position = hit.point;


        }
    }
}