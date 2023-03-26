using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BasicMoving : MonoBehaviour
{
    #region Variables: Movement
    public Animator _anim;

    private Vector2 _input;
    private CharacterController _characterController;
    private Vector3 _direction, _direction_temp;

    [SerializeField] private float speed;

    #endregion
    #region Variables: Rotation

    [SerializeField] private float smoothTime = 0.05f;
    private float _currentVelocity;

    #endregion
    #region Variables: Gravity

    private float _gravity = -9.81f;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _velocity;

    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
        ApplyMovement();
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded && _velocity < 0.0f)
        {
            _velocity = -1.0f;
        }
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
        }

        _direction.y = _velocity;
    }

    private void ApplyRotation()
    {
        if (_input.sqrMagnitude == 0) return;

        var targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
    }

    private void ApplyMovement()
    {
        _characterController.Move(_direction * speed * Time.deltaTime);
        
            //anim.SetTrigger("stop");
            
         
    }

    public void Move(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        //print(_input);
        _direction = new Vector3(_input.x, 0.0f, _input.y);
        _direction_temp = _direction;
        //anim.SetTrigger("run");
        //if (anim.GetCurrentAnimatorStateInfo(0))
        //if (context.started)
        if (_input.x == 0 && _input.y == 0)
            _anim.SetBool("run", false);
        else
            _anim.SetBool("run", true);
    }

    public void Combat(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("stop movement");
            _direction.x = 0.0f;
            _direction.z = 0.0f;
            _input.x = 0;
            _input.y = 0;
        }
        
    }
}
