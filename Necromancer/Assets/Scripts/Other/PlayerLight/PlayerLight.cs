using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLight : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    void Start()
    {
        //playerTransform = PlayerStats.Instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position + new Vector3 (xOffset, yOffset);
    }
}
