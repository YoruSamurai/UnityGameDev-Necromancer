using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private GameObject cineCamera;

    [SerializeField] private float parallaxEffect;
    [SerializeField] private int index;

    [SerializeField] private float parallaxOffsetX;
    [SerializeField] private float parallaxOffsetY;

    [SerializeField] private float parallaxScale;


    private void Start()
    {
        transform.localScale = new Vector3(parallaxScale, parallaxScale, 1);
        transform.position = new Vector3(cineCamera.transform.position.x + parallaxOffsetX, cineCamera.transform.position.y + parallaxOffsetY);
    }

    private void Update()
    {
        float distanceToMove = cineCamera.transform.position.x * (1-parallaxEffect);
        if (index == 1)
            transform.position = new Vector3(cineCamera.transform.position.x + distanceToMove + parallaxOffsetX, cineCamera.transform.position.y + parallaxOffsetY);

    }

    private void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
    }
}
