using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Core.Singleton;

public class HumanPlayer : PlayerController
{
    [SerializeField] private RuntimeAnimatorController _noMask;
    [SerializeField] private RuntimeAnimatorController _withMask;

    private bool _isMasked;

    protected override void Awake()
    {
        base.Awake();
    }
    
    void Start()
    {
        if (PlayerPrefs.GetString("isMasked").Equals("true")) { this._isMasked = true; animator.runtimeAnimatorController = _withMask; }
        else { this._isMasked = false; animator.runtimeAnimatorController = _noMask; }
    }

    void Update()
    {
        _isBattleScene = false;

        if (_canMove) _moveDirection = move.action.ReadValue<Vector2>();

        if (!_isBattleScene)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!_canMove)
                {
                    if (_inventorySet) { Notify(EventsEnum.Inventory); }
                }else{ Notify(EventsEnum.Inventory); }
            } else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_canMove)
                {
                    if (_menuSet) { Notify(EventsEnum.Menu); }
                }else{ Notify(EventsEnum.Menu); }
            }
        }

        AnimateMovement();
    }

    public void SetIsMasked(bool isMasked)
    {
        _isMasked = isMasked;

        if (_isMasked) { animator.runtimeAnimatorController = _withMask; }
        else{ animator.runtimeAnimatorController = _noMask; }
    }
}
