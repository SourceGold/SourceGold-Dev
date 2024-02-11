using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    // Start is called before the first frame update

    public InputMap InputMap;

    private InputMap.PlayerActions _player;
    private PlayerManager _playerManager;
    private MovementHandler _movementHandler;
    private MeleeHandler _meleeHandler;
    private ShootingHandler _shootingHandler;
    private GameItemSensationHandler _gameItemSensationHandler;
    private InGamePauseController _inGamePauseController;

    private void Awake()
    {
        InputMap = new InputMap();
        _player = InputMap.Player;
        InputMap.Player.Enable();
        _playerManager = FindObjectOfType<PlayerManager>();
        _movementHandler = _playerManager.GetComponentInChildren<MovementHandler>();
        _meleeHandler = _playerManager.GetComponentInChildren<MeleeHandler>();
        _shootingHandler = _playerManager.GetComponentInChildren<ShootingHandler>();
        _gameItemSensationHandler = _playerManager.GetComponentInChildren<GameItemSensationHandler>();
        _inGamePauseController = FindObjectOfType<InGamePauseController>();
    }

    void Start()
    {
        _player.Move.started += GetMoveInput;
        _player.Move.performed += GetMoveInput;
        _player.Move.canceled += GetMoveInput;
        _player.Run.started += ToggleRunning;
        _player.Run.performed += ToggleRunning;
        _player.Run.canceled += ToggleRunning;
        _player.Jump.started += TriggerJump;
        _player.Jump.performed += TriggerJump;
        _player.Jump.canceled += TriggerJump;
        _player.LockOn.started += ToggleLockOn;
        _player.LockOn.performed += ToggleLockOn;
        _player.LockOn.canceled += ToggleLockOn;
        _player.Aim.performed += ToggleAim;


        _player.EquipWeapon.performed += EquipWeapon;
        _player.SwitchWeapon.performed += SwitchWeapon;
        _player.StandingMeleeLight.performed += StandingMeleeLight;
        _player.StandingMeleeHeavy.performed += StandingMeleeHeavy;
        //_player.MeleeAttack2Press.performed += StandingMeleeAttack2Press;
        //_player.MeleeAttack2Release.performed += StandingMeleeAttack2Release;

        _player.Shoot.started += HandleShoot;
        _player.Shoot.performed += HandleShoot;
        _player.Shoot.canceled += HandleShoot;

        _player.SceneInteraction.performed += PickupKeyPress;

        _player.EscClick.performed += EscOnClick;
    }

    #region Movement Bindings
    private void GetMoveInput(InputAction.CallbackContext context)
    {
        _movementHandler.GetMoveInput(context.ReadValue<Vector2>());
        
    }
    private void ToggleRunning(InputAction.CallbackContext context)
    {
        _movementHandler.ToggleRunning(context.performed);
    }
    private void TriggerJump(InputAction.CallbackContext context)
    {
        _movementHandler.TriggerJump(context.ReadValueAsButton());
    }
    private void ToggleLockOn(InputAction.CallbackContext context)
    {
        _movementHandler.ToggleLockOn(context.performed);
    }
    private void ToggleAim(InputAction.CallbackContext context)
    {
        _movementHandler.ToggleAim(context.performed);
    }
    #endregion

    #region Melee Bindings
    private void EquipWeapon(InputAction.CallbackContext context)
    {
        _meleeHandler.EquipWeapon(context.performed);
    }
    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        _meleeHandler.SwitchWeapon(context.performed);
    }
    private void StandingMeleeLight(InputAction.CallbackContext context)
    {
        _meleeHandler.StandingMeleeLight(context.performed);
    }
    private void StandingMeleeHeavy(InputAction.CallbackContext context)
    {
        _meleeHandler.StandingMeleeHeavy(context.performed);
    }
    //private void StandingMeleeAttack2Press(InputAction.CallbackContext context)
    //{
    //    _meleeHandler.StandingMeleeAttack2Press(context.performed);
    //}
    //private void StandingMeleeAttack2Release(InputAction.CallbackContext context)
    //{
    //    _meleeHandler.StandingMeleeAttack2Release(context.performed);
    //}
    #endregion

    #region Range Bindings
    private void HandleShoot(InputAction.CallbackContext context)
    {
        _shootingHandler.HandleShoot(context.performed, context.canceled);
    }
    #endregion

    #region Interaction Bindings
    private void PickupKeyPress(InputAction.CallbackContext context)
    {
        _gameItemSensationHandler.PickupKeyPress(context.performed);
    }
    #endregion

    #region UI Bindings
    private void EscOnClick(InputAction.CallbackContext context)
    {
        _inGamePauseController.EscOnClick(context.performed);
    }
    #endregion
}
