using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Core.Singleton;

public class HumanPlayer : PlayerController
{
    void Update()
    {
        _isBattleScene = false;

        if (_canMove) _moveDirection = move.action.ReadValue<Vector2>();

        AnimateMovement();
    }
}
