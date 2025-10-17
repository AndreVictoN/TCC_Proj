using System;
using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using DG.Tweening;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : Singleton<BattleManager>, IObserver
{
    public Enemy enemy;
    public Sprite mothSprite;
    public Sprite birdSprite;
    public Sprite lizardSprite;
    public AnimatorController mothAnimator;
    public AnimatorController birdAnimator;
    public AnimatorController lizardAnimator;
    public Ezequiel ezequiel;
    public Estella estella;
    public PlayerController player;
    public List<GameObject> cards;
    public Cards cardEzequiel;
    public Cards cardEstella;
    public TextMeshProUGUI battleText;
    public GameObject textBox;
    public GameObject estellaHealing;
    public float wordSpeed = 0.06f;
    public Image transitionImage;

    [SerializeField] private string _pastScene;
    [SerializeField] private string _stringToType;
    private TextMeshProUGUI _enemyName;
    private bool _prototypeSetupMade;
    private bool _setupMade;
    private bool _enemyIsAttacking;
    private bool _ezequielInScene = false;
    private bool _estellaInScene = false;
    private Coroutine _currentTextCoroutine;
    private Coroutine _ezequielCoroutine;
    private Coroutine _estellaCoroutine;

    void Start()
    {
        _enemyName = GameObject.FindGameObjectWithTag("EnemyName").GetComponent<TextMeshProUGUI>();
        _prototypeSetupMade = false;
        _setupMade = false;
        _enemyIsAttacking = false;

        //PlayerPrefs.SetString("pastScene", "PrototypeScene");
        _pastScene = PlayerPrefs.GetString("pastScene");

        if(enemy == null) enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        if(player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.SetMyTurn(true);

        if(_pastScene == "PrototypeScene"){PrototypeBattleSetup();}
        else {BattleSetup(_pastScene);}
    }

    void Update()
    {
        if(_pastScene == "PrototypeScene"){PrototypeBattle();}
        else {Battle();}

        BattleUpdate();
    }

    private void BattleUpdate()
    {
        if(!player.GetMyTurn() && !_enemyIsAttacking)
        {
            _enemyIsAttacking = true;
            enemy.Attack(player);
        }
    }

    private IEnumerator SomeoneShowsUp()
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
        if (player.GetCanAct()) return;

        if (_currentTextCoroutine != null && _currentTextCoroutine != _ezequielCoroutine)
        {
            StopCoroutine(_currentTextCoroutine);
            _currentTextCoroutine = StartCoroutine(ActionkWarningCoroutine("action"));
        }
        else if (_currentTextCoroutine == null)
        {
            _currentTextCoroutine = StartCoroutine(ActionkWarningCoroutine("action"));
        }
    }

    public void ShowAttackWarning()
    {
        if(player.GetAnxiety() < 110f){ player.SetCanAttack(true); }
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
        }else { battleText.text = "VocÊ estÁ muito mal para isso"; }
        textBox.SetActive(true);
        textBox.GetComponent<Animator>().Play("warningText");
        yield return new WaitForSeconds(3f);
        textBox.SetActive(false);
    }

    public void SetEnemyIsAttacking(bool isAttacking){_enemyIsAttacking = isAttacking;}

    private void BattleSetup(string pastScene)
    {
        if (pastScene.Equals("Floor2") && PlayerPrefs.GetString("currentState").Equals("Start"))
        {
            enemy.gameObject.GetComponent<Animator>().runtimeAnimatorController = birdAnimator;
            enemy.gameObject.GetComponent<SpriteRenderer>().sprite = birdSprite;
            enemy.gameObject.transform.localPosition = new Vector2(enemy.gameObject.transform.localPosition.x, 1f);
            enemy.gameObject.transform.localScale = new Vector2(1.6f, 1.6f);
            _enemyName.text = "Feabird";
            _setupMade = true;
        }else if(pastScene.Equals("Class") && PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            enemy.gameObject.GetComponent<Animator>().runtimeAnimatorController = lizardAnimator;
            enemy.gameObject.GetComponent<SpriteRenderer>().sprite = lizardSprite;
            enemy.gameObject.transform.localPosition = new Vector2(enemy.gameObject.transform.localPosition.x, 1.15f);
            enemy.gameObject.transform.localScale = new Vector2(1.6f, 1.6f);
            _enemyName.text = "Judgzy";
            _setupMade = true;
        }

        player.SetCanAct(false);
        player.SetCanAttack(true);
    }

    private void Battle()
    {
        if (!_setupMade && _pastScene != null) BattleSetup(_pastScene);

        if (_pastScene.Equals("Floor2") && PlayerPrefs.GetString("currentState").Equals("Start")){
            if (player.GetAnxiety() >= 95 && !_estellaInScene)
            {
                estella.gameObject.SetActive(true);
                cardEstella.gameObject.SetActive(true);

                _estellaInScene = true;

                _estellaCoroutine = StartCoroutine(SomeoneShowsUp());
                _currentTextCoroutine = _estellaCoroutine;
            }
        } else if(_pastScene.Equals("Class") && PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            if (player.GetAnxiety() >= 100 && !_estellaInScene && !_ezequielInScene)
            {
                estella.gameObject.SetActive(true);
                ezequiel.gameObject.SetActive(true);
                cardEstella.gameObject.SetActive(true);
                cardEzequiel.gameObject.SetActive(true);
                estellaHealing.SetActive(true);

                _estellaInScene = true;
                _ezequielInScene = true;

                _currentTextCoroutine = StartCoroutine(SomeoneShowsUp());
            }
        }
    }

#region Prototype
    private void PrototypeBattleSetup()
    {
        enemy.gameObject.GetComponent<Animator>().runtimeAnimatorController = mothAnimator;
        enemy.gameObject.GetComponent<SpriteRenderer>().sprite = mothSprite;
        enemy.gameObject.transform.localPosition = new Vector2(enemy.gameObject.transform.localPosition.x, 0.7f);
        enemy.gameObject.transform.localScale = new Vector2(1f, 1f);
        _enemyName.text = "Vergosulo";

        _prototypeSetupMade = true;

        player.SetCanAct(false);
        player.SetCanAttack(true);
    }

    private void PrototypeBattle()
    {
        if (!_prototypeSetupMade) PrototypeBattleSetup();

        if(player.GetAnxiety() >= 95 && !_ezequielInScene)
        {
            _ezequielInScene = true;

            ezequiel.gameObject.SetActive(true);
            cardEzequiel.gameObject.SetActive(true);

            _ezequielCoroutine = StartCoroutine(SomeoneShowsUp());
            _currentTextCoroutine = _ezequielCoroutine;
        }
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
        cards.AddRange(GameObject.FindGameObjectsWithTag("Card"));
        cards.ForEach(card => card.SetActive(false));
        _enemyName.gameObject.SetActive(false);

        if (_pastScene == "PrototypeScene") { StartCoroutine(PrototypeWin()); }
        else { StartCoroutine(Win()); }
    }

    private void LoseSettings()
    {
        cards.AddRange(GameObject.FindGameObjectsWithTag("Card"));
        cards.ForEach(card => card.SetActive(false));
        enemy.gameObject.SetActive(false);

        if (_pastScene == "Class") { StartCoroutine(Lose()); }
    }

    private IEnumerator Win()
    {
        if(_pastScene == "Floor2" && PlayerPrefs.GetString("currentState").Equals("Start"))
        {
            yield return new WaitForSeconds(0.3f);
            textBox.SetActive(true);
            battleText.text = "";

            _stringToType = "\"O-O que foi isso...?\"";
            if (_currentTextCoroutine != null) StopCoroutine(_currentTextCoroutine);
            StartCoroutine(Typing());
            yield return new WaitForSeconds(1f);

            transitionImage.gameObject.SetActive(true);
            StartCoroutine(FadeTransition(transitionImage.color, new Color(0, 0, 0, 1), 1));
            PlayerPrefs.SetString("pastScene", "BattleScene");
            yield return new WaitForSeconds(1f);

            //Destroy(player);
            player.GetComponent<Animator>().StopPlayback();
            //cards.ForEach(card => card.GetComponent<Animator>().StopPlayback());
            DOTween.KillAll();
            SceneManager.LoadScene("Floor2");
        }else if (_pastScene == "Class" && PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            yield return new WaitForSeconds(0.3f);
            textBox.SetActive(true);
            battleText.text = "";

            _stringToType = "\"Eu... tive uma ideia...\"";
            if (_currentTextCoroutine != null) StopCoroutine(_currentTextCoroutine);
            StartCoroutine(Typing());
            yield return new WaitForSeconds(1f);

            transitionImage.gameObject.SetActive(true);
            StartCoroutine(FadeTransition(transitionImage.color, new Color(0, 0, 0, 1), 1));
            PlayerPrefs.SetString("pastScene", "BattleScene");
            yield return new WaitForSeconds(1f);

            //Destroy(player);
            player.GetComponent<Animator>().StopPlayback();
            //cards.ForEach(card => card.GetComponent<Animator>().StopPlayback());
            DOTween.KillAll();
            SceneManager.LoadScene("Class");
        }
    }

    private IEnumerator Lose()
    {
        yield return new WaitForSeconds(0.3f);
        textBox.SetActive(true);
        battleText.text = "";

        transitionImage.gameObject.SetActive(true);
        StartCoroutine(FadeTransition(transitionImage.color, new Color(0, 0, 0, 1), 1));
        yield return new WaitForSeconds(1f);

        //Destroy(player);
        player.GetComponent<Animator>().StopPlayback();
        //cards.ForEach(card => card.GetComponent<Animator>().StopPlayback());
        DOTween.KillAll();
        SceneManager.LoadScene("DefeatScene");
    }

    private IEnumerator PrototypeWin()
    {
        yield return new WaitForSeconds(0.3f);
        textBox.SetActive(true);
        battleText.text = "";

        _stringToType = "\"Q-Quem É ele...\"";
        if (_currentTextCoroutine != null) StopCoroutine(_currentTextCoroutine);
        StartCoroutine(Typing());
        yield return new WaitForSeconds(1f);

        transitionImage.gameObject.SetActive(true);
        StartCoroutine(FadeTransition(transitionImage.color, new Color(0, 0, 0, 1), 1));
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
        }else if(evt == EventsEnum.Lose)
        {
            LoseSettings();
        }
    }

    public void DamageEnemy(float damage){if(enemy != null) enemy.TakeDamage(damage);}

    public void SetPastScene(string pastScene) {_pastScene = pastScene;}
}
