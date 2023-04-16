using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class KD_Controller : MonoBehaviour
{
    #region Variables: Animation

    private Animator _animator;

    #endregion
    #region Variables: Movement

    private Vector2 _input;
    private bool _isRunning;
    private Vector3 _direction;
    private Vector3 _velocity;
    private float _speed;
    private float _targetSpeed;
    private Transform _transform;

    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _rotateSpeed = 1000.0f;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _runSpeed = 3.5f;

    private float _moveSpeedMultiplier;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        _isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Move();
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
        Debug.Log(_input);
    }

    public void ToggleRunning(InputAction.CallbackContext context)
    {
        _isRunning = context.performed ? !_isRunning : _isRunning;
        Debug.Log(_isRunning);
    }
    private void Rotate()
    {
        if (_input.Equals(Vector2.zero))
            return;

        _direction.x = _input.x;
        _direction.z = _input.y;

        Quaternion target = Quaternion.LookRotation(_direction, Vector3.up);
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, target, _rotateSpeed * Time.deltaTime);
    }
    private void Move()
    {
        _targetSpeed = _isRunning ? _runSpeed : _walkSpeed;
        _targetSpeed *= _input.magnitude;
        _speed = Mathf.Lerp(_speed, _targetSpeed, 0.5f);
        _animator.SetFloat("HorizontalSpeed", _speed);
    }
}
