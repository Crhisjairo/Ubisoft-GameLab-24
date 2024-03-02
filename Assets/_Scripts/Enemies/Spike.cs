using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Controllers;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(1);
        }
    }
}
