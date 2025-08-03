using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Core.Singleton;

public class PlayerController : Singleton<PlayerController>
{
    [Header("Player Settings")]
    GameManager gameManager;

    [Header("Movement Settings")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public InputActionReference move;
    public Animator animator;
    public Animator battleAnimator;
    public Sprite idleDown;

    #region tags to compare
    [Header("ScenesToLoad")]
    public string prototypeScene = "PrototypeScene";
    public string firstFloor = "FirstFloorPrototype";
    public string battleScene = "BattleScene";
    public string classroom = "Classroom";

    private string npcTag = "NPC";
    private string doorTag = "Door";
    private string stairsTag = "Stairs";
    public string changeSceneTag = "ToOtherScene";
    #endregion

    [Header("HoverSettings")]
    public SpriteRenderer spriteRenderer;
    public Color defaultColor;
    public float fadeTime = 1f;

    [Header("BattleSettings")]
    public BattleManager battleManager;
    public GameObject enemy;
    public float attackTime = 0.5f;
    public Vector3 defaultPosition;

    #region Privates
    private Vector2 _moveDirection;
    private Vector3 _positionBeforeFloor;
    private bool _canMove = true;
    private bool _isBattleScene = false;
    private bool _isMovingBattle = false;
    private Coroutine _currentCoroutine;
    private Tween _currentTween;
    private string _walkingDown;
    private Vector2 _lastMoveDirection = Vector2.down;
    #endregion

    protected override void Awake()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();

        if (SceneManager.GetActiveScene().name == battleScene)
        {
            defaultPosition = new Vector3(-3.7f, 1.3f, 0);
            this.gameObject.transform.position = defaultPosition;
        }
        else if (SceneManager.GetActiveScene().name == classroom)
        {
            defaultPosition = new Vector3(-10.53f, 6f, 0);
            this.gameObject.transform.position = defaultPosition;
        }
        else
        {
            this.gameObject.transform.position = new Vector3(0, 0, 0);
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
        Vector2 enemyPosition = enemy.transform.position;
        _isMovingBattle = true;
        _currentTween?.Kill();
        _currentTween = transform.DOLocalMove(enemyPosition, attackTime).SetEase(Ease.InQuad).OnComplete(() => _isMovingBattle = false);
    }

    public void AnimateAttack()
    {
        if (!_isBattleScene) return;

        battleAnimator.SetTrigger("Attack");
    }

    void OnMouseExit()
    {
        if (!_isBattleScene) return;
        _currentCoroutine = battleManager.FadeToColor(defaultColor, _currentCoroutine, spriteRenderer, fadeTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(npcTag) || collision.gameObject.CompareTag(doorTag))
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.gameObject.CompareTag(stairsTag))
        {
            if (SceneManager.GetActiveScene().name == prototypeScene) StartCoroutine(LoadFloor(firstFloor));
            else if (SceneManager.GetActiveScene().name == firstFloor) StartCoroutine(LoadFloor(prototypeScene));

            _canMove = false;
        }
        else if (collision.gameObject.CompareTag(changeSceneTag))
        {
            string sceneToLoad;

            if (SceneManager.GetActiveScene().name == prototypeScene)
            {
                sceneToLoad = classroom;
                gameManager.StartLoadNewScene(sceneToLoad, this.gameObject, collision.gameObject);
            }
            else if (SceneManager.GetActiveScene().name == classroom)
            {
                sceneToLoad = prototypeScene;
                gameManager.StartLoadNewScene(sceneToLoad, this.gameObject, collision.gameObject);
            }
            _canMove = false;
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

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
        rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);
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
}
