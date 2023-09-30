using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectFilterScript : MonoBehaviour
{
    GameObject[] _gameObjects;
    
    List<TrackedObject> _trackedObjects;

    float _lastCheckSeconds = 0.0f;

    float DistanceToleranceBetweenKeys = 3.0f; // meter
    float TrackedObjectLostTimeout = 10.0f; // seconds

    Vector3 previousVehiclePos = Vector3.zero;
    Vector3 previousTargetPos = Vector3.zero;

    class TrackedObject
    {
        public Vector3 LastKnownLocation;
        public float SecondsSinceLost;
        public float SecondsSinceTracked;
        public bool SeenNow;
        public GameObject GameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameObjects = new GameObject[4];
        _gameObjects[0] = GameObject.Find("Character1");
        _gameObjects[1] = GameObject.Find("Character2");
        _gameObjects[2] = GameObject.Find("Character3");
        _gameObjects[3] = GameObject.Find("Character4");

        _trackedObjects = new List<TrackedObject>();
        for(int i=0; i<4; ++i)
        {
            _trackedObjects.Add(new TrackedObject
            {
                LastKnownLocation = _gameObjects[i].transform.position,
                GameObject = _gameObjects[i],
                SecondsSinceLost = 0.0f
            });
        }
    }

    void HideGameObject(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.zero;
    }

    void ShowGameObject(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        // If a tracked object's last known position is recent, and there is a gameobject that is in a similar place, assume that this object is tracked
        // If a game object is not close to any of the tracked object's, assume it is new
        // If a tracked object was seen a long time ago, delete it
        // The relevant object is the one that is captured for the longest time

        float seconds = Time.realtimeSinceStartup;
        float delta = seconds - _lastCheckSeconds;

        if (delta > 0.1)
        {
            // Assume that there is no gameobject currently corresponding to this tracked object
            foreach (TrackedObject trackedObject in _trackedObjects)
            {
                trackedObject.SeenNow = false;
            }

            for (int i=0; i<4; ++i)
            {
                bool gameObjectIsTracked = false;

                foreach(TrackedObject trackedObject in _trackedObjects)
                {
                    if((trackedObject.LastKnownLocation - _gameObjects[i].transform.position).magnitude < DistanceToleranceBetweenKeys && _gameObjects[i].transform.localPosition.magnitude > 5.0)
                    {
                        gameObjectIsTracked = true;
                        trackedObject.SeenNow = true;
                        trackedObject.SecondsSinceLost = 0.0f;
                        trackedObject.SecondsSinceTracked += delta;
                        trackedObject.LastKnownLocation = _gameObjects[i].transform.position;
                        trackedObject.GameObject = _gameObjects[i];
                        break;
                    }
                }

                if(!gameObjectIsTracked && _gameObjects[i].transform.localPosition.magnitude > 5.0)
                {
                    // Add new tracked object
                    gameObjectIsTracked = true;
                    _trackedObjects.Add(new TrackedObject
                    {
                        LastKnownLocation = _gameObjects[i].transform.position,
                        SeenNow = true,
                        GameObject = _gameObjects[i],
                        SecondsSinceLost = 0.0f,
                        SecondsSinceTracked = 0.0f
                    });
                }
            }


            int longestTrackedIndex = -1;
            for (int i=0; i<_trackedObjects.Count; ++i)
            {
                if (!_trackedObjects[i].SeenNow)
                {
                    _trackedObjects[i].SecondsSinceLost += delta;
                }

                if (_trackedObjects[i].GameObject.transform.localPosition.magnitude < 5.0) continue;

                if(longestTrackedIndex == -1)
                {
                    longestTrackedIndex = i;
                }
                else if (_trackedObjects[longestTrackedIndex].SecondsSinceTracked < _trackedObjects[i].SecondsSinceTracked)
                {
                    longestTrackedIndex = i;
                }
            }

            bool found = false;
            for (int i = 0; i < _trackedObjects.Count; ++i)
            {
                if (i == longestTrackedIndex && _trackedObjects[i].SeenNow)
                {
                    found = true;
                    ShowGameObject(_trackedObjects[i].GameObject);
                    var targetVelocity = (GameObject.Find("TargetVelocity").transform.position - previousTargetPos).magnitude / Time.deltaTime;
                    GameObject.Find("TargetVelocity").GetComponent<TextMeshProUGUI>().text = String.Format("Target velocity: {0} m/s", targetVelocity);
                    previousTargetPos = GameObject.Find("TargetVelocity").transform.position;
                }
                else
                {
                    HideGameObject(_trackedObjects[i].GameObject);
                }
            }

            if(!found)
            {
                GameObject.Find("TargetVelocity").GetComponent<TextMeshProUGUI>().text = "Target unknown";
            }
            

            _trackedObjects.RemoveAll(trackedObject => trackedObject.SecondsSinceLost > TrackedObjectLostTimeout);

            _lastCheckSeconds = seconds;
        }

        var bmwVelocity = (GameObject.Find("BMWPos").transform.position - previousVehiclePos).magnitude / Time.deltaTime;
        var bmwLabel = GameObject.Find("VehicleVelocity").GetComponent<TextMeshProUGUI>().text = String.Format("Vehicle velocity: {0} m/s", bmwVelocity);

        previousVehiclePos = GameObject.Find("BMWPos").transform.position;
    }
}
