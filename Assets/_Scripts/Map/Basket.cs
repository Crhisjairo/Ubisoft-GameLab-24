using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private float maxRotation = 35f;

    [SerializeField] private Transform maxPositionRight, maxPositionLeft;
    
    private Rigidbody2D _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.rotation = Mathf.Clamp(_rb.velocity.x, -maxRotation, maxRotation);
    }
}
