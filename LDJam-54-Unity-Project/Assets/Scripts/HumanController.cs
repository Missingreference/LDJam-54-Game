using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{

    public Human human { get; private set; }

    void Awake()
    {
        human = GetComponent<Human>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(human == null) return;

        Vector2 moveDirection = Vector2.zero;
        if(Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector2.up;
        }
        if(Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector2.down;
        }
        if(Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector2.left;
        }
        if(Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector2.right;
        }

        human.Move(moveDirection);

        if(Input.GetMouseButtonDown(0))
        {
            human.Attack();
        }
    }
}
