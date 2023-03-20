using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class animationStateController : MonoBehaviour
{
    #region Variables: CharacterController
    private CharacterController _characterController;
    #endregion

    #region Variables: Animation
    private Animator _animator;
    #endregion

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
    
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!IsGrounded()) return;

        _animator.SetTrigger("isJumping");
    }

    private bool IsGrounded() => _characterController.isGrounded;
}
