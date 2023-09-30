using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCameraController : MonoBehaviour
{
    public Human target;

    void Awake()
    {
        if(target == null)
            target = FindObjectOfType<Human>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(target == null) return;

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10.0f);
    }
}
