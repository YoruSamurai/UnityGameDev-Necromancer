using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private GameObject cineCamera;

    [SerializeField] private float parallaxEffect;
    [SerializeField] private int index;

    private float xPosition;

    private void Start()
    {
        xPosition = transform.position.x;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float distanceToMove = cineCamera.transform.position.x * parallaxEffect;
        if(index ==1)
            transform.position = new Vector3(xPosition + distanceToMove, cineCamera.transform.position.y);
        if(index == 2)
            transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);
    }
}
