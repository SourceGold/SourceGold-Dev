using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{
    // Start is called before the first frame update

    public InputMap InputMap;

    private InputMap.PlayerActions _player;
    private InputMap.SettingActions _setting;
    private bool _isSetting;

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
        _setting = InputMap.Setting;
        _isSetting = false;

        _playerManager = FindObjectOfType<PlayerManager>();
        _movementHandler = _playerManager.GetComponentInChildren<MovementHandler>();
        _meleeHandler = _playerManager.GetComponentInChildren<MeleeHandler>();
        _shootingHandler = _playerManager.GetComponentInChildren<ShootingHandler>();
        _gameItemSensationHandler = _playerManager.GetComponentInChildren<GameItemSensationHandler>();
        _inGamePauseController = FindObjectOfType<InGamePauseController>();
    }

    void Start()
    {
        _player.Enable();

        RegisterMovement();

        RegisterMelee();

        RegisterRanged();

        RegisterInteraction();

        RegisterUI();
    }

    public void ToggleInputActionMap()
    {
        if (_isSetting)
        {
            _isSetting = false;
            _setting.Disable();
            _player.Enable();
        }
        else
        {
            _isSetting = true;
            _player.Disable();
            _setting.Enable();
        }
    }

    #region Movement Bindings
    private void RegisterMovement()
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
        _player.Roll.performed += TriggerRoll;
        _player.SwitchBattlePose.performed += SwitchBattlePoseMovement;
    }
    private void GetMoveInput(InputAction.CallbackContext context)
    {
        _movementHandler.GetMoveInput(context.ReadValue<Vector2>());
        
    }
    private void ToggleRunning(InputAction.CallbackContext context)
    {
        _movementHandler.ToggleRunning(context);
    }
    private void TriggerJump(InputAction.CallbackContext context)
    {
        _movementHandler.TriggerJump(context.ReadValueAsButton());
    }
    private void ToggleLockOn(InputAction.CallbackContext context)
    {
        if (context.performed)
            _movementHandler.ToggleLockOn();
    }
    private void ToggleAim(InputAction.CallbackContext context)
    {
        if (context.performed)
            _movementHandler.ToggleAim();
    }
    private void TriggerRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
            _movementHandler.TriggerRoll();
    }
    private void SwitchBattlePoseMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
            _movementHandler.SwitchBattlePose();
    }
    #endregion

    #region Melee Bindings
    private void RegisterMelee()
    {
        _player.EquipWeapon.performed += EquipWeapon;
        _player.SwitchWeapon.performed += SwitchWeapon;
        _player.StandingMeleeLight.performed += StandingMeleeLight;
        _player.StandingMeleeHeavy.performed += StandingMeleeHeavy;
        _player.StandingMeleeHeavy.canceled += AttackRelease;
        _player.SwitchBattlePose.performed += SwitchBattlePoseMelee;


        //_player.MeleeAttack2Press.performed += StandingMeleeAttack2Press;
        //_player.MeleeAttack2Release.performed += StandingMeleeAttack2Release;
    }
    private void EquipWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
            _meleeHandler.EquipWeapon();
    }
    private void SwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
            _meleeHandler.SwitchWeapon();
    }
    private void StandingMeleeLight(InputAction.CallbackContext context)
    {
        if (context.performed)
            _meleeHandler.StandingMeleeLight();
    }
    private void StandingMeleeHeavy(InputAction.CallbackContext context)
    {
        if (context.performed)
            _meleeHandler.StandingMeleeHeavy();
    }
    private void SwitchBattlePoseMelee(InputAction.CallbackContext context)
    {
        if (context.performed)
            _meleeHandler.SwitchBattlePose();
    }
    private void AttackRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
            _meleeHandler.AttackRelease();
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
    private void RegisterRanged()
    {
        _player.Shoot.started += HandleShoot;
        _player.Shoot.performed += HandleShoot;
        _player.Shoot.canceled += HandleShoot;
    }
    private void HandleShoot(InputAction.CallbackContext context)
    {
        _shootingHandler.HandleShoot(context.performed, context.canceled);
    }
    #endregion

    #region Interaction Bindings
    private void RegisterInteraction()
    {
        _player.SceneInteraction.performed += PickupKeyPress;
    }

    private void PickupKeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
            _gameItemSensationHandler.PickupKeyPress();
    }
    #endregion

    #region UI Bindings
    private void RegisterUI()
    {
        _player.EscClick.performed += EscOnClick;

        _setting.EscClick.performed += EscOnClick;
    }
    private void EscOnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _inGamePauseController.EscOnClick();
            ToggleInputActionMap();
        } 
    }
    #endregion
}
