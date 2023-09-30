using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public class ObjectFilterScript : MonoBehaviour
{
    GameObject _relevantGameObject;
    GameObject[] _gameObjects;
    
    TrackedObject[] _trackedObjects;
    
    float[,] _scores;

    class TrackedObject
    {
        public Vector3 LastLocation;
        public AnimationClip Animation;
        public int ObjectIndex;
        public float Score;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameObjects = new GameObject[4];
        _gameObjects[0] = GameObject.Find("Character1");
        _gameObjects[1] = GameObject.Find("Character2");
        _gameObjects[2] = GameObject.Find("Character3");
        _gameObjects[3] = GameObject.Find("Character4");

        _scores = new float[5,5];
        _trackedObjects = new TrackedObject[4];
        for(int i=0; i<4; ++i)
        {
            _trackedObjects[i] = new TrackedObject
            {
                Animation = _gameObjects[i].GetComponent<Animation>().clip,
                LastLocation = _gameObjects[i].transform.position,
                ObjectIndex = i,
                Score = float.MaxValue
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if(i == 4 || j == 4)
                {
                    _scores[i, j] = 10.0f; // Matching threshold
                }
                else if (_gameObjects[i].transform.localPosition.magnitude < 0.1)
                {
                    _scores[i,j] = float.MaxValue;
                }
                else
                {
                    _scores[i, j] = (_gameObjects[i].transform.position - _trackedObjects[j].LastLocation).magnitude;
                }
            }
        }

        // Bipartite graph matching
        // https://www.youtube.com/watch?v=8uTXar8AWOw&t=1359s

        for (int i = 0; i < 5; i++)
        {
            int minIndex = 0;
            for (int j = 0; j < 5; j++)
            {
                if (_scores[i,j] < _scores[i, minIndex])
                {
                    minIndex = j;
                }
            }

            for(int k=0; k<5; ++k)
            {
                if(k != minIndex)
                {
                    _scores[i, k] = float.MaxValue;
                }
            }

            for (int k = 0; k < 5; ++k)
            {
                if (k != i)
                {
                    _scores[k, minIndex] = float.MaxValue;
                }
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (_scores[i,j] != float.MaxValue)
                {
                    if(i < 4 && j < 4)
                    {
                        _trackedObjects[j].ObjectIndex = i;
                        _trackedObjects[j].LastLocation = _gameObjects[i].transform.position;
                        _trackedObjects[j].Score = _scores[i, j];
                        _gameObjects[i].GetComponent<Animation>().clip = _trackedObjects[j].Animation;
                    }
                    else if(j < 4) // Object disappeared
                    {
                        _trackedObjects[j].ObjectIndex = -1;
                        _trackedObjects[j].Score = float.MaxValue;
                    }
                }
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (_scores[i, 4] != float.MaxValue)
            {
                for(int k=0; k<4; ++k)
                {
                    if (_trackedObjects[k].ObjectIndex == -1)
                    {
                        // New object appeared
                        _trackedObjects[k].ObjectIndex = i;
                        _trackedObjects[k].LastLocation = _gameObjects[i].transform.position;
                        _trackedObjects[k].Animation = _gameObjects[i].GetComponent<Animation>().clip;
                        _trackedObjects[k].Score = float.MaxValue;
                        break;
                    }
                }
            }
        }

        int MinIndex = 0;
        for(int i=1; i<4; ++i)
        {
            if (_trackedObjects[i].Score < _trackedObjects[MinIndex].Score)
            {
                MinIndex = i;
            }
        }

        for (int i = 0; i < 4; ++i)
        {
             _gameObjects[i].transform.localScale = Vector3.zero;
        }
        _gameObjects[_trackedObjects[MinIndex].ObjectIndex].transform.localScale = Vector3.one;
    }
}
