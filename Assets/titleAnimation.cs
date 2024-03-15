using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class titleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject objectAnimation;
    
    public Vector3 finalPosition = new Vector3(1.5f, 1.5f, 0);
    
    private void Start()
    {
        LeanTween.scale(objectAnimation, finalPosition, 5f).setEaseInOutSine().setLoopPingPong();
        
    }


    private void Update()
    {
        float start, end, val;
    }
}
