using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class titleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject objectAnimation;
    
    public Vector3 finalPosition = new Vector3(1.5f, 1.5f, 0);
    public Color startColor = Color.blue; 
    public Color endColor = Color.red; 
    public float colorChangeSpeed = 1f;
    public bool glowEnabled = true; 
    public float glowOffset = 0.66f;
    public float glowOuter = 0.731f;

    private TextMeshPro textMesh;
    private float lerpTime;


    private void Start()
    {
        LeanTween.scale(objectAnimation, finalPosition, 5f).setEaseInOutSine().setLoopPingPong();

        textMesh = objectAnimation.GetComponent<TextMeshPro>();
       
    }


    private void Update()
    {
        var glowOpacity = glowEnabled ? 1f : 0f; 

        lerpTime += Time.deltaTime * colorChangeSpeed;
        Color currentColor = Color.Lerp(startColor, endColor, Mathf.PingPong(lerpTime, 1));
        currentColor.a *= glowOpacity; 

        textMesh.fontMaterial.SetColor("_GlowColor", currentColor);
        textMesh.fontMaterial.SetFloat("_GlowOffset", glowOffset);
        textMesh.fontMaterial.SetFloat("_GlowOuter", glowOuter);
        textMesh.UpdateMeshPadding();
    }
}
