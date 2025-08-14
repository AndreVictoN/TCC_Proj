using System;
using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager>, IObserver
{
    public Enemy enemy;
    public Ezequiel ezequiel;
    public PlayerController player;
    public List<GameObject> cards;
    public Cards cardEzequiel;
    public TextMeshProUGUI battleText;
    public GameObject textBox;
    public float wordSpeed = 0.06f;
    public Image transitionImage;

    [SerializeField] private string _pastScene;
    [SerializeField] private string _stringToType;
    private TextMeshProUGUI _enemyName;
    private bool _prototypeSetupMade;
    private bool _enemyIsAttacking;
    private bool _ezequielInScene = false;
    private Coroutine _currentTextCoroutine;
    private Coroutine _ezequielCoroutine;

    void Start()
    {
        _enemyName = GameObject.FindGameObjectWithTag("EnemyName").GetComponent<TextMeshProUGUI>();
        _prototypeSetupMade = false;
        _enemyIsAttacking = false;

        PlayerPrefs.SetString("pastScene", "PrototypeScene");
        _pastScene = PlayerPrefs.GetString("pastScene");

        if(enemy == null) enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.SetMyTurn(true);

        if(_pastScene == "PrototypeScene"){PrototypeBattleSetup();}
    }

    void Update()
    {
        if(_pastScene == "PrototypeScene"){PrototypeBattle();}

        BattleUpdate();
    }

    private void BattleUpdate()
    {
        if(!player.GetMyTurn() && !_enemyIsAttacking)
        {
            _enemyIsAttacking = true;
            enemy.Attack(player);
        }

        if(player.GetAnxiety() == 95 && !_ezequielInScene)
        {
            _ezequielInScene = true;

            ezequiel.gameObject.SetActive(true);
            cardEzequiel.gameObject.SetActive(true);

            _ezequielCoroutine = StartCoroutine(EzequielShowsUp());
            _currentTextCoroutine = _ezequielCoroutine;
        }
    }

    private IEnumerator EzequielShowsUp()
    {
        textBox.SetActive(true);
        textBox.GetComponent<Animator>().Play("battleText");
        _stringToType = "AlguÉm se juntou A vocÊ";
        StartCoroutine(Typing());
        
        player.SetCanAct(false);
        player.SetCanAttack(false);

        yield return new WaitForSeconds(6f);
        textBox.SetActive(false);

        _currentTextCoroutine = null;
    }

    public void ShowActionkWarning()
    {
        if(player.GetCanAct()) return;

        if(_currentTextCoroutine != null && _currentTextCoroutine != _ezequielCoroutine)
        {
            StopCoroutine(_currentTextCoroutine);
            _currentTextCoroutine = StartCoroutine(ActionkWarningCoroutine("action"));
        }else if(_currentTextCoroutine == null)
        {
            _currentTextCoroutine = StartCoroutine(ActionkWarningCoroutine("action"));
        }
    }

    public void ShowAttackkWarning()
    {
        if(player.GetCanAttack()) return;

        if(_currentTextCoroutine != null && _currentTextCoroutine != _ezequielCoroutine)
        {
            StopCoroutine(_currentTextCoroutine);
            _currentTextCoroutine = StartCoroutine(ActionkWarningCoroutine("attack"));
        }else if(_currentTextCoroutine == null)
        {
            _currentTextCoroutine = StartCoroutine(ActionkWarningCoroutine("attack"));
        }
    }

    private IEnumerator ActionkWarningCoroutine(string actionType)
    {
        if(actionType == "action")
        {
            battleText.text = "VocÊ nÃo pode fazer isso agora";
        }else { battleText.text = "Alex estÁ muito mal para isso"; }
        textBox.SetActive(true);
        textBox.GetComponent<Animator>().Play("warningText");
        yield return new WaitForSeconds(3f);
        textBox.SetActive(false);
    }

    public void SetEnemyIsAttacking(bool isAttacking){_enemyIsAttacking = isAttacking;}

#region Prototype
    private void PrototypeBattleSetup()
    {
        _enemyName.text = "Vergosulo";

        _prototypeSetupMade = true;

        player.SetCanAct(false);
        player.SetCanAttack(true);
    }

    private void PrototypeBattle()
    {
        if(!_prototypeSetupMade) PrototypeBattleSetup();
        
    }
#endregion

    public Tween GoToDefaultPosition(GameObject gameO, bool _isMoving, Tween _currentTween, Vector3 defaultPosition, float attackTime)
    {
        if(!_isMoving && gameO.activeSelf)
        {
            _currentTween?.Kill();
            if(gameO != null) return gameO.transform.DOLocalMove(defaultPosition, attackTime);
        }

        return null;
    }

    public Coroutine FadeToColor(Color color, Coroutine _currentCoroutine, SpriteRenderer spriteRenderer, float fadeTime)
    {
        if(_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        Color oldColor = spriteRenderer.color;

        return StartCoroutine(Fade(oldColor, color, fadeTime, spriteRenderer));
    }

    private IEnumerator Fade(Color old, Color color, float time, SpriteRenderer spriteRenderer)
    {
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Clamp01(elapsedTime/time);
            spriteRenderer.color = Color.Lerp(old, color, lerpAmount);

            yield return null;
        }
    }

    protected IEnumerator Typing()
    {
        //_isTyping = true;

        if(battleText.text != "")
        {
            battleText.text = "";
        }

        foreach(char letter in _stringToType)
        {
            if(battleText.text != _stringToType)
            {
                battleText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
        }

        //_isTyping = false;
    }

    private void WinSettings()
    {
        if(_pastScene == "PrototypeScene")
        {
            cards.AddRange(GameObject.FindGameObjectsWithTag("Card"));
            cards.ForEach(card => card.SetActive(false));
            _enemyName.gameObject.SetActive(false);

            StartCoroutine(PrototypeWin());
        }
    }

    private IEnumerator PrototypeWin()
    {
        yield return new WaitForSeconds(0.3f);
        textBox.SetActive(true);
        battleText.text = "";

        _stringToType = "\"Q-Quem É ele...\"";
        if(_currentTextCoroutine != null) StopCoroutine(_currentTextCoroutine);
        StartCoroutine(Typing());
        yield return new WaitForSeconds(1f);

        transitionImage.gameObject.SetActive(true);
        StartCoroutine(FadeTransition(transitionImage.color, new Color(0,0,0,1), 1));
        PlayerPrefs.SetString("pastScene", "BattleScene");
        yield return new WaitForSeconds(1f);

        //Destroy(player);
        player.GetComponent<Animator>().StopPlayback();
        //cards.ForEach(card => card.GetComponent<Animator>().StopPlayback());
        DOTween.KillAll();
        SceneManager.LoadScene("PrototypeScene");
    }

    private IEnumerator FadeTransition(Color old, Color color, float time)
    {
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Clamp01(elapsedTime/time);
            transitionImage.color = Color.Lerp(old, color, lerpAmount);

            yield return null;
        }
    }

    void IObserver.OnNotify(EventsEnum evt)
    {
        if(evt == EventsEnum.EnemyDead)
        {
            WinSettings();
        }
    }

    public void DamageEnemy(float damage){if(enemy != null) enemy.TakeDamage(damage);}

    public void SetPastScene(string pastScene) {_pastScene = pastScene;}
}
