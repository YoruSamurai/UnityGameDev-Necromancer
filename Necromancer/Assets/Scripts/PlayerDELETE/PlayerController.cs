using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader input;

    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;


    private Vector2 _moveDirection;
    private bool _isJumping;

    private void Start()
    {
        input.MoveEvent += HandleMove;

        input.JumpEvent += HandleJump;
        input.JumpCancelEvent += HandleCancelledJump;
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void HandleMove(Vector2 dir)
    {
        _moveDirection = dir;
    }

    private void HandleJump()
    {
        _isJumping = true;
    }

    private void HandleCancelledJump()
    {
        _isJumping = false;
    }

    private void Move()
    {
        if(_moveDirection.x == 0)
        {
            return;
        }
        if(_moveDirection.x > 0)
        {
            transform.position += new Vector3(1, 0, 0) * (speed * Time.deltaTime);
        }
        else if (_moveDirection.x < 0)
        {
            transform.position += new Vector3(-1, 0, 0) * (speed * Time.deltaTime);
        }
        //transform.position += new Vector3(_moveDirection.x, 0, 0) * (speed * Time.deltaTime);
    }

    private void Jump()
    {
        if(_isJumping)
        {
            transform.position += new Vector3(0, 1, 0) * (jumpSpeed * Time.deltaTime);
        }
    }
}
