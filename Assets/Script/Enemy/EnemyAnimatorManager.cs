using UnityEngine;

public class EnemyAnimatorManager : AnimatorManager
{
    EnemyLocomotionManager _enemyLocomotionManager;

    private Animator _animator;
    public override Animator Animator 
    {
        get { return _animator; }
        set { _animator = value; }
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyLocomotionManager = GetComponentInParent<EnemyLocomotionManager>();
    }
    
    private void OnAnimatorMove()
    {
        Vector3 playerDelterMovement = _animator.deltaPosition;
        playerDelterMovement.y = _enemyLocomotionManager.VerticalVelocity * Time.deltaTime;
        _enemyLocomotionManager.CharacterController.Move(playerDelterMovement);
    }
}
