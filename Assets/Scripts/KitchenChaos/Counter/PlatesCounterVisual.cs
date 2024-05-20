using System;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenChaos.Counter
{
    public class PlatesCounterVisual : MonoBehaviour
    {
        [SerializeField] private GameObject plateVisualPrefab;
        [SerializeField] private PlatesCounter platesCounter;
        
        private List<GameObject> _platesSpawnGameObjectList;

        private void Awake()
        {
            _platesSpawnGameObjectList = new List<GameObject>();
        }

        private void Start()
        {
            platesCounter.OnPlateSpawn += PlatesCounterOnOnPlateSpawn;
            platesCounter.OnPlateRemove += PlatesCounterOnOnPlateRemove;
        }

        private void PlatesCounterOnOnPlateRemove(object sender, EventArgs e)
        {
            var plateForDestroy = _platesSpawnGameObjectList[^1];
            _platesSpawnGameObjectList.Remove(plateForDestroy);
            Destroy(plateForDestroy);
        }

        private void PlatesCounterOnOnPlateSpawn(object sender, EventArgs e)
        {
            var plate = Instantiate(plateVisualPrefab, platesCounter.KitchenObjectFollowTransform);
            var plateOffsetY = _platesSpawnGameObjectList.Count * .1f;
            plate.transform.localPosition = new Vector3(0, plateOffsetY, 0);
            _platesSpawnGameObjectList.Add(plate);
        }
    }
}