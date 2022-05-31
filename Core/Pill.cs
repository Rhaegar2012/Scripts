using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

public class Pill : MonoBehaviour
{
    public static event Action OnPillEaten;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag=="pacman")
        {
            OnPillEaten?.Invoke();
            Destroy(gameObject);
        }
    }
}
