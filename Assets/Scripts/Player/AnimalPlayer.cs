using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Core.Singleton;

public class AnimalPlayer : PlayerController
{
    void Update()
    {
        if (SceneManager.GetActiveScene().name == battleScene)
        {
            _isBattleScene = true;

            _currentTween = battleManager.GoToDefaultPosition(this.gameObject, _isMovingBattle, _currentTween, defaultPosition, attackTime);
        }
        else
        {
            _isBattleScene = false;
        }

        _canMove = !_isBattleScene;

        if (_canMove) _moveDirection = move.action.ReadValue<Vector2>();

        AnimateMovement();
    }
}
