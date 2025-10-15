using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Core.Singleton;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;
using System;
using UnityEngine.InputSystem.Controls;

public abstract class PlayerController : Subject, IHealthManager
{
    [Header("Player Settings")]
    protected GameManager gameManager;

    [Header("Movement Settings")]
    public Rigidbody2D rb;
    public float moveSpeed = 3f;
    public InputActionReference move;
    public Animator animator;
    public Animator battleAnimator;
    public Sprite idleDown;
    public Sprite idleLeft;

    #region tags to compare
    [Header("ScenesToLoad")]
    public string testScene = "TestScene";
    public string firstFloor = "FirstFloorPrototype";
    public string battleScene = "BattleScene";
    public string classroom = "Class";

    protected string npcTag = "NPC";
    protected List<string> npcTags = new() { "Ezequiel", "Estella", "Yuri", "Rebecca" };
    protected string doorTag = "Door";
    protected string stairsTag = "Stairs";
    public string changeSceneTag = "ToOtherScene";
    #endregion

    [Header("HoverSettings")]
    public SpriteRenderer spriteRenderer;
    public Color defaultColor;
    public float fadeTime = 1f;

    [Header("BattleSettings")]
    public float myDamage;
    public GameObject enemy;
    public Image playerSplash;
    public float attackTime = 0.5f;
    public Vector3 defaultPosition;
    public BattleManager battleManager;
    [SerializeField] protected bool _myTurn;
    public TextMeshProUGUI sanity;
    public TextMeshProUGUI anxiety;
    public TextMeshProUGUI maxAnxiety;
    public TextMeshProUGUI maxSanity;
    [SerializeField] protected int _numSanity;
    [SerializeField] protected int _numAnxiety;
    [SerializeField] protected int _numMaxSanity;
    [SerializeField] protected int _numMaxAnxiety;

    #region Privates
    protected Vector2 _moveDirection;
    protected Vector3 _positionBeforeFloor;
    protected bool _canMove = true;
    protected bool _canAct = true;
    private bool _canAttack = true;
    protected bool _isBattleScene = false;
    protected bool _isMovingBattle = false;
    protected Coroutine _currentCoroutine;
    protected Tween _currentTween;
    protected string _walkingDown;
    protected Vector2 _lastMoveDirection = Vector2.down;
    protected string collisionTag = "collision";
    private float _defaultAnimatorSpeed;
    protected bool _inventorySet;
    protected bool _menuSet;
    #endregion

    protected override void Awake()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        Subscribe(gameManager);

        if (SceneManager.GetActiveScene().name == battleScene)
        {
            BattleSceneSettings();
        }
        else if (SceneManager.GetActiveScene().name == "Classroom")
        {
            defaultPosition = new Vector3(-10.53f, 6f, 0);
            this.gameObject.transform.position = defaultPosition;
        }
        else if (SceneManager.GetActiveScene().name == testScene)
        {
            this.gameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        else if (SceneManager.GetActiveScene().name.Equals("Class"))
        {
            this.gameObject.transform.localPosition = new Vector2(4.54f, 8.05f);
        }
        else if (SceneManager.GetActiveScene().name == "Floor2" && (PlayerPrefs.GetString("currentState").Equals("Start") || PlayerPrefs.GetString("currentState").Equals("FirstLeaving")))
        {
            if (!PlayerPrefs.GetString("pastScene").Equals("BattleScene")) this.gameObject.transform.localPosition = new Vector2(-17.47f, 4.59f);
            else this.gameObject.transform.localPosition = new Vector2(23.46f, 14.32541f);
        }else if(SceneManager.GetActiveScene().name.Equals("Floor2") && PlayerPrefs.GetString("currentState").Equals("StartDayTwo"))
        {
            if (PlayerPrefs.GetString("transitionType").Equals("frontTransition"))
            {
                this.gameObject.transform.localPosition = new Vector2(-41.82f, 10.24f);
            }else if (PlayerPrefs.GetString("transitionType").Equals("backTransition"))
            {
                this.gameObject.transform.localPosition = new Vector2(33.75f, 10.03f);
            }
            else
            {
                this.gameObject.transform.localPosition = new Vector2(-17.47f, 4.59f);
            }
        }
        else
        {
            _defaultAnimatorSpeed = animator.speed;
            if (PlayerPrefs.GetString("currentState").Equals("StartDayTwo"))
            {
                this.gameObject.transform.localPosition = new Vector2(-2.26f, 72.24f);
            }else if (SceneManager.GetActiveScene().name.Equals("Terreo") && PlayerPrefs.GetString("transitionType").Equals("frontTransition")) {
                this.gameObject.transform.localPosition = new Vector2(25.73f, 68.27f);
            }
            else if (SceneManager.GetActiveScene().name.Equals("Terreo") && PlayerPrefs.GetString("transitionType").Equals("backTransition")) {
                this.gameObject.transform.localPosition = new Vector2(17.88f, 111.86f);
            }
            else { this.gameObject.transform.position = new Vector3(0, 0, 0); }
        }

        if (rb == null && this.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            rb = this.gameObject.GetComponent<Rigidbody2D>();
        }

        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == battleScene)
        {
            _isBattleScene = true;
            playerSplash = GameObject.FindGameObjectWithTag("AlexOctopus").GetComponent<Image>();

            _currentTween = battleManager.GoToDefaultPosition(this.gameObject, _isMovingBattle, _currentTween, defaultPosition, attackTime);
        }
        else
        {
            _isBattleScene = false;
        }

        if (_isBattleScene) _canMove = false;

        if (_canMove) _moveDirection = move.action.ReadValue<Vector2>();

        AnimateMovement();
    }

    private void BattleSceneSettings()
    {
        _canAct = true;
        _canAttack = true;
        defaultPosition = new Vector3(-3.7f, 1.3f, 0);
        if (PlayerPrefs.GetString("pastScene").Equals("PrototypeScene")) { myDamage = 5f; }
        else if (PlayerPrefs.GetString("pastScene") == "Floor2" && PlayerPrefs.GetString("currentState").Equals("Start")) { myDamage = 15f; }
        this.gameObject.transform.position = defaultPosition;

        if (sanity == null) sanity = GameObject.FindGameObjectWithTag("AlexSanity").GetComponent<TextMeshProUGUI>();
        if (maxSanity == null) maxSanity = GameObject.FindGameObjectWithTag("AlexMaxSanity").GetComponent<TextMeshProUGUI>();
        if (anxiety == null) anxiety = GameObject.FindGameObjectWithTag("AlexAnxiety").GetComponent<TextMeshProUGUI>();
        if (maxAnxiety == null) maxAnxiety = GameObject.FindGameObjectWithTag("AlexMaxAnxiety").GetComponent<TextMeshProUGUI>();

        maxAnxiety.text = "/" + _numMaxAnxiety.ToString();
        maxSanity.text = "/" + _numMaxSanity.ToString();
        _numAnxiety = _numMaxAnxiety - _numMaxAnxiety + _numMaxAnxiety / 2;
        _numSanity = _numMaxSanity;
        anxiety.text = _numAnxiety.ToString();
        sanity.text = _numSanity.ToString();
    }

    public void AnimateMovement()
    {
        if (!_isBattleScene)
        {
            animator.SetFloat("Horizontal", _moveDirection.x);
            animator.SetFloat("Vertical", _moveDirection.y);
            animator.SetFloat("Speed", _moveDirection.sqrMagnitude);

            if (_moveDirection.sqrMagnitude > 0.01f)
            {
                _lastMoveDirection = _moveDirection;
                animator.SetFloat("LastHorizontal", _moveDirection.x);
                animator.SetFloat("LastVertical", _moveDirection.y);
            }
        }
    }

    public void SetIdle(char position)
    {
        if (position == 'L') { this.gameObject.GetComponent<SpriteRenderer>().sprite = idleLeft; }
        if (position == 'D') { this.gameObject.GetComponent<SpriteRenderer>().sprite = idleDown; }
    }

    void OnMouseOver()
    {
        if (!_isBattleScene) return;

        Color colorToFade = spriteRenderer.color;
        colorToFade.b += 1f;
        _currentCoroutine = battleManager.FadeToColor(colorToFade, _currentCoroutine, spriteRenderer, fadeTime);

        /*if(Input.GetMouseButtonDown(0))
        {
            Vector2 enemyPosition = enemy.transform.position;
            PlayerMovement(enemyPosition);
        }*/
    }

    public void PlayerMovement()
    {
        if (!_myTurn) return;
        if (!_canAct && !_canAttack) return;

        Vector2 enemyPosition = enemy.transform.position;
        _isMovingBattle = true;
        _currentTween?.Kill();
        _currentTween = transform.DOLocalMove(enemyPosition, attackTime).SetEase(Ease.InQuad).OnComplete(() => _isMovingBattle = false);
    }

    public void AnimateAttack()
    {
        if (!_isBattleScene || !_myTurn || (!_canAct && !_canAttack)) return;

        StartCoroutine(AttackAnimation());
    }

    IEnumerator AttackAnimation()
    {
        playerSplash.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        battleAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        battleManager.DamageEnemy(myDamage);
        _myTurn = false;
        yield return new WaitForSeconds(0.5f);
        playerSplash.gameObject.SetActive(false);
    }

    void OnMouseExit()
    {
        if (!_isBattleScene) return;
        _currentCoroutine = battleManager.FadeToColor(defaultColor, _currentCoroutine, spriteRenderer, fadeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collided with: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag(npcTag) || collision.gameObject.CompareTag(doorTag) || npcTags.Contains(collision.gameObject.tag) || collision.gameObject.CompareTag(collisionTag))
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag(stairsTag))
        {
            if (SceneManager.GetActiveScene().name == testScene) StartCoroutine(LoadFloor(firstFloor));
            else if (SceneManager.GetActiveScene().name == firstFloor) StartCoroutine(LoadFloor(testScene));

            _canMove = false;
        }else if(collision.gameObject.CompareTag("ToOtherFloor"))
        {
            if (SceneManager.GetActiveScene().name.Equals("Terreo"))
            {
                PlayerPrefs.SetString("transitionType", "backTransition");
                gameManager.StartLoadNewScene("Floor2", this.gameObject, collision.gameObject);
            }
            _canMove = false;
        }
        else if (collision.gameObject.CompareTag(changeSceneTag))
        {
            string sceneToLoad;

            if (SceneManager.GetActiveScene().name == testScene)
            {
                sceneToLoad = classroom;
                gameManager.StartLoadNewScene(sceneToLoad, this.gameObject, collision.gameObject);
            }
            else if (SceneManager.GetActiveScene().name == classroom)
            {
                sceneToLoad = testScene;
                gameManager.StartLoadNewScene(sceneToLoad, this.gameObject, collision.gameObject);
            }else if(SceneManager.GetActiveScene().name.Equals("Floor2"))
            {
                sceneToLoad = "Class";
                gameManager.StartLoadNewScene(sceneToLoad, this.gameObject, collision.gameObject);
            }else if(SceneManager.GetActiveScene().name.Equals("Class"))
            {
                sceneToLoad = "Floor2";
                PlayerPrefs.SetString("transitionType", "");
                gameManager.StartLoadNewScene(sceneToLoad, this.gameObject, collision.gameObject);
            }
            _canMove = false;
        }
        else if (collision.gameObject.CompareTag("StopTrigger"))
        {
            _canMove = false;
            Notify(EventsEnum.StopInteraction);
        }
        else if (collision.gameObject.CompareTag("ToSecretary"))
        {
            _canMove = false;
            Notify(EventsEnum.IntoSecretary);
        }
        else if (collision.gameObject.CompareTag("ExitGame"))
        {
            _canMove = false;
            Notify(EventsEnum.ExitGame);
        }
        else if (collision.gameObject.CompareTag("EnterSchool"))
        {
            _canMove = false;
            Notify(EventsEnum.EnterSchool);
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    public void SetCanAct(bool canAct) { _canAct = canAct; }
    public void SetCanAttack(bool canAttack) { _canAttack = canAttack; }

    IEnumerator LoadFloor(string sceneName)
    {
        transform.DOMoveY(-2, 1f).SetRelative();

        yield return new WaitForSeconds(1f);

        _positionBeforeFloor = this.gameObject.transform.position;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;

        transform.DOMoveY(2.5f, 1f).SetRelative();

        yield return new WaitForSeconds(1f);

        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        _canMove = true;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void MovePlayer()
    {
        if (_canMove) { rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed); }
        else { rb.linearVelocity = Vector2.zero; _moveDirection = Vector2.zero; }
    }

    public void SetSpriteDown()
    {
        animator.SetFloat("LastVertical", 0);
        this.gameObject.GetComponent<SpriteRenderer>().sprite = idleDown;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PrototypeEzequielTrigger1"))
        {
            _canMove = false;
            Notify(EventsEnum.CallPrototypeEzequiel);
        }
        else if (collision.gameObject.CompareTag("PrototypeBattleTrigger"))
        {
            Notify(EventsEnum.PrototypeBattle);
        }
        else if (collision.transform.parent != null && collision.transform.parent.gameObject.CompareTag("PrototypeFirstInteractionTrigger"))
        {
            _canMove = false;
            Notify(EventsEnum.PrototypeFirstInteraction);
        }
        else if (collision.gameObject.CompareTag("GirlTrigger"))
        {
            _canMove = false;
            Notify(EventsEnum.PrototypeGirl);
        }
        else if (collision.gameObject.CompareTag("ToOutside"))
        {
            if (this.gameObject.transform.localPosition.y > collision.gameObject.transform.localPosition.y)
            {
                _canMove = false;
                Notify(EventsEnum.ToOutside);
            }
        }
        else if (collision.gameObject.CompareTag("FirstInteractionTrigger"))
        {
            _canMove = false;
            Notify(EventsEnum.FirstInteraction);
        }
        else if (collision.gameObject.CompareTag("FirstConflictTrigger"))
        {
            _canMove = false;
            Notify(EventsEnum.FirstConflict);
        }
        else if (collision.gameObject.CompareTag("BattleTrigger"))
        {
            Notify(EventsEnum.Battle);
        }
    }

    public void SetAnimation(string animation, float animationSpeed)
    {
        animator.Play(animation);

        if (animationSpeed != 0) animator.speed = animationSpeed;
        else { animator.speed = _defaultAnimatorSpeed; }
    }

    public void SetAnimationTrigger(string trigger)
    {
        battleAnimator.SetTrigger(trigger);
    }

    public IEnumerator GoTo(float time, Vector2 position, char xy, bool canMoveAfter)
    {
        _canMove = false;

        if (xy == 'x') this.transform.DOMoveX(position.x, time);
        else if (xy == 'y') this.transform.DOMoveY(position.y, time);

        yield return new WaitForSeconds(time);

        if (canMoveAfter)
        {
            animator.Play("Moving");
            _canMove = true;
        }
    }

    public float ToX(float positionX)
    {
        float time = 0;
        float distance = Mathf.Abs(this.gameObject.transform.localPosition.x - positionX);

        if (distance <= 1)
        {
            time = 0.3f;
        }
        else if (distance > 1 && distance <= 3.5f)
        {
            time = 1f;
        }
        else if (distance > 3.5f && distance <= 10)
        {
            time = 3f;
        }
        else if (distance > 10)
        {
            time = 5f;
        }

        if (time != 0) StartCoroutine(GoTo(time, new Vector2(positionX, this.transform.position.y), 'x', false));

        return time;
    }

    public float ToY(float positionY)
    {
        float time = 0;
        float distance = Mathf.Abs(this.gameObject.transform.localPosition.y - positionY);

        if (distance <= 1)
        {
            time = 0.3f;
        }
        else if (distance > 1 && distance <= 3.5f)
        {
            time = 1f;
        }
        else if (distance > 3.5f && distance <= 10)
        {
            time = 3f;
        }
        else if (distance > 10)
        {
            time = 5f;
        }

        if (time != 0) StartCoroutine(GoTo(time, new Vector2(this.transform.position.x, positionY), 'y', false));

        return time;
    }

    public void InventorySet(bool isSet)
    {
        _inventorySet = isSet;
        if (isSet) _canMove = false;
        else _canMove = true;
    }

    public void MenuSet(bool isSet)
    {
        _menuSet = isSet;
        if (isSet) _canMove = false;
        else _canMove = true;
    }

    public void SetMyTurn(bool myTurn) { _myTurn = myTurn; }
    public bool GetMyTurn() { return _myTurn; }
    public bool GetCanAct() { return _canAct; }
    public bool GetCanAttack() { return _canAttack; }

    #region HealthManagement
    public void TakeDamage(float damage)
    {
        if (!_isBattleScene) return;

        float criticalHitRatio = UnityEngine.Random.value;

        if(criticalHitRatio <= 0.1f){ damage *= 2; }

        _numSanity -= (int)Math.Round(damage);
        if (_numSanity < 0) _numSanity = 0;
        sanity.text = _numSanity.ToString();

        _numAnxiety += (int)(Math.Round(damage) * 1.5);
        if (_numAnxiety > 100) _numAnxiety = 100;
        anxiety.text = _numAnxiety.ToString();
    }

    public void Heal(float healingAmount)
    {
        if (!_isBattleScene) return;

        _numSanity += (int)Math.Round(healingAmount);
        _numSanity = Mathf.Clamp(_numSanity, 0, _numMaxSanity);
        sanity.text = _numSanity.ToString();

        _numAnxiety -= (int)(Math.Round(healingAmount) * 1.5);
        _numAnxiety = Mathf.Clamp(_numAnxiety, 0, _numMaxAnxiety);
        anxiety.text = _numAnxiety.ToString();
    }

    public int GetSanity() { return _numSanity; }
    public int GetAnxiety() { return _numAnxiety; }
    #endregion

    #region Items Management
    public void RegisterToSubscribers(InventoryManager ivManager)
    {
        if(!_subscribers.Contains(ivManager)) Subscribe(ivManager);
    }

    public void NotifyFromItem(EventsEnum evt)
    {
        Notify(evt);
    }
    #endregion
}
