using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Controls")]
    [Range (0,1)]
    [SerializeField] public float musicVolume=1.0f;
    [SerializeField] public float soundVolume=1.0f;
    [Header("Music Assets")]
    [SerializeField] public AudioClip introClip;
    [SerializeField] public AudioClip intermissionClip;
    
    [Header("FX Assets")]
    [SerializeField] public AudioClip pacmanMunchClip;
    [SerializeField] public AudioClip pacmanDeathClip;
    [SerializeField] public AudioClip pacmanEatGhostClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
