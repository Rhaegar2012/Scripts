using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pellet : MonoBehaviour
{
    public static event Action OnPelletEaten;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="pacman")
        {
            OnPelletEaten?.Invoke();
            Destroy(gameObject);
        }
    }
}
