using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class NPC : DialogueBox, IHealthManager
{
    [Header("GeneralSettings")]
    public Color defaultColor;
    public Vector3 defaultPosition;
    public Animator animator;

    [Header("Battle")]
    public Animator battleAnimator;
    public BattleManager battleManager;
    public SpriteRenderer spriteRenderer;
    public GameObject enemy;
    public float attackTime = 0.3f;
    public float fadeTime = 0.5f;
    public TextMeshProUGUI sanity;
    public TextMeshProUGUI anxiety;
    public TextMeshProUGUI maxAnxiety;
    public TextMeshProUGUI maxSanity;
    [SerializeField] protected int _numSanity;
    [SerializeField] protected int _numAnxiety;
    [SerializeField] protected int _numMaxSanity;
    [SerializeField] protected int _numMaxAnxiety;

    #region Privates
    protected Coroutine _currentCoroutine;
    protected Tween _currentTween;
    protected bool _playerIsClose;
    protected bool _isBattling;
    protected String _npcName;
    protected bool _isMoving;
    protected PlayerController _player;
    protected bool _inPrototype;
    private bool _playerIsSet;
    #endregion

    void Start()
    {
        if(SceneManager.GetActiveScene().name != "BattleScene")
        {
            ResetText();
            SetPlayer();
        }else{
            BattleSettings();
        }
    }

    private void SetPlayer()
    {
        if(_player == null) _player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();

        if(_player != null) _playerIsSet = true;
    }

    protected virtual void BattleSettings()
    {
        defaultPosition = getPosition();
        this.gameObject.transform.position = defaultPosition;
        enemy = GameObject.FindGameObjectWithTag("Enemy");

        maxAnxiety.text = "/" + _numMaxAnxiety.ToString();
        maxSanity.text = "/" + _numMaxSanity.ToString();
        _numAnxiety = _numMaxAnxiety - _numMaxAnxiety + _numMaxAnxiety/2;
        _numSanity = _numMaxSanity;
        anxiety.text = _numAnxiety.ToString();
        sanity.text = _numSanity.ToString();
    }

    protected void BasicSettings()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
        _npcName = this.gameObject.name;
    }

    protected virtual Vector3 getPosition()
    {
        /*return _npcName switch
        {
            "Estella" => new Vector3(-5.3f, 0.01f, 0),
            "Rebecca" => new Vector3(-6.9f, 3.7f, 0),
            "Ezequiel" => new Vector3(-5.3f, 2.5f, 0),
            "Yuri" => new Vector3(-6.9f, -1.1f, 0),
            _ => this.gameObject.transform.position,
        };*/
        return this.gameObject.transform.position;
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name != "BattleScene")
        {
            UpdateNPC();
            if(_playerIsSet == false){SetPlayer();}
        }else
        {
            _isBattling = true;
            BattleManagement();
        }
    }

    public void BattleManagement()
    {
        if(!_isBattling) return;
        battleAnimator = this.gameObject.GetComponent<Animator>();

        _currentTween = battleManager.GoToDefaultPosition(this.gameObject, _isMoving, _currentTween, defaultPosition, attackTime);
    }

    void OnMouseOver()
    {
        if(SceneManager.GetActiveScene().name != "BattleScene") return;
        _currentCoroutine = battleManager.FadeToColor(CheckColorAspectByNPC(), _currentCoroutine, spriteRenderer, fadeTime);

        /*if(Input.GetMouseButtonDown(0))
        {
            Vector2 enemyPosition = enemy.transform.position;
            Movement(enemyPosition);
        }*/
    }

    protected virtual Color CheckColorAspectByNPC()
    {
        Color colorToFade = spriteRenderer.color;

        /*switch(_npcName)
        {
            case "Estella":
                colorToFade.r += 0.2f;
                break;
            case "Rebecca":
                colorToFade.g += 0.7f;
                break;
            case "Ezequiel":
                colorToFade.b -= 0.7f;
                break;
            case "Yuri":
                colorToFade.b -= 1f;
                break;
        }*/

        return colorToFade;
    }

    public void Movement()
    {
        Vector2 enemyPosition = enemy.transform.position;
        _isMoving = true;
        _currentTween?.Kill();
        _currentTween = transform.DOLocalMove(enemyPosition, attackTime).SetEase(Ease.InQuad).OnComplete(() => _isMoving = false);
    }

    void OnMouseExit()
    {
        if(SceneManager.GetActiveScene().name != "BattleScene") return;
        _currentCoroutine = battleManager.FadeToColor(defaultColor, _currentCoroutine, spriteRenderer, fadeTime);
    }

    public virtual void UpdateNPC()
    {
        if(!_isAutomatic)
        {
            if(Input.GetKeyDown(KeyCode.E) && _playerIsClose && !_isTyping && !_isAutomatic && !_isMoving)
            {
                if(!dialoguePanel.activeSelf)
                {
                    dialogueText.text = "";
                    _i = 0;

                    SetDialoguePanel();
                    StartCoroutine(Typing());
                }
                else
                {
                    dialoguePanel.SetActive(false);
                    _isClosed = true;
                    StopCoroutine(Typing());
                    ResetText();
                }
            }else if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && _playerIsClose && _isTyping && !_isAutomatic && !_isMoving)
            {
                if(this.gameObject.CompareTag("Ezequiel") && _isMoving)
                {
                    return;
                }

                StopCoroutine(Typing());
                dialogueText.text = dialogue[_i];
                //continueButton.SetActive(true);
            }else if(Input.GetKeyDown(KeyCode.Return) && !_isTyping && _playerIsClose && dialoguePanel.activeSelf)
            {
                if(this.gameObject.CompareTag("Ezequiel") && _isMoving)
                {
                    return;
                }

                NextLine();
            }

            if(dialogueText.text == dialogue[_i])
            {
                //continueButton.SetActive(true);
            }
        }

        if(!_isMoving)
        {
            if(_player?.gameObject.transform.localPosition.y >= this.gameObject.transform.localPosition.y)
            {
                this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
            }else{
                this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }

    public override void ResetText()
    {
        if(_isBattling) return;

        base.ResetText();
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerIsClose = true;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerIsClose = false;
            ResetText();
        }
    }

    public void AnimateAttack()
    {
        if (!_isBattling) return;

        StartCoroutine(AttackActions());
    }

    private IEnumerator AttackActions()
    {
        yield return new WaitForSeconds(0.3f);
        battleAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        battleManager.DamageEnemy(enemy.GetComponent<Enemy>().GetHealth());
    }

    public virtual void RecieveTrigger(GameObject player, string trigger) {}

    protected IEnumerator GoToPlayer(float time, GameObject player, string trigger)
    {
        _dialogueStarted = false;

        if(trigger.Equals("PrototypeEzequielTrigger1"))
        {
            this.transform.DOMoveY(player.transform.localPosition.y, time);
            yield return new WaitForSeconds(time);

            this.transform.DOMoveX(player.transform.localPosition.x - 0.8f, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }else if(trigger.Equals("GirlTrigger"))
        {
            animator.Play("IdleR");
            yield return new WaitForSeconds(0.5f);
            animator.Play("WalkingRight");

            this.transform.DOMoveX(-0.96f, time);
            yield return new WaitForSeconds(time);
            animator.Play("IdleR");
        }
    }

    protected void StartDialogue()
    {
        if(!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            _i = 0;

            SetDialoguePanel();
            StartCoroutine(Typing());
        }
    }

    protected IEnumerator GoTo(float time, Vector2 position, char xy)
    {
        if(xy == 'x') this.transform.DOMoveX(position.x, time);
        else if(xy == 'y') this.transform.DOMoveY(position.y, time);
        
        yield return new WaitForSeconds(time);

        if(_isAutomatic && this.gameObject.name.Equals("Ezequiel")) _isAutomatic = false;
    }

    public IEnumerator StartAutomaticTalk()
    {
        GameObject skipText = new();
        _isAutomatic = true;

        yield return new WaitForSeconds(0.5f);
        if(!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            _i = 0;

            SetDialoguePanel();
            skipText = GameObject.FindGameObjectWithTag("SkipText");
            skipText.SetActive(false);
            StartCoroutine(Typing());
        }

        while(_i != dialogue.Count - 1)
        {
            yield return null;

            if(!_isTyping)
            {
                yield return new WaitForSeconds(_secondsToReturn);
                NextLine();
            }
        }

        if(skipText != null) skipText.SetActive(true);
    }

    public void SetIsAutomatic(bool automatic)
    {
        _isAutomatic = automatic;
    }

    public bool GetIsClosed() {return _isClosed;}
    
    public void SetAnimation(string animation)
    {
        if(animator == null) animator = this.gameObject.GetComponent<Animator>();

        animator.Play(animation);
    }

#region HealthManagement
    public void TakeDamage(float damage)
    {
        _numSanity -= (int) Math.Round(damage);
        sanity.text = _numSanity.ToString();

        _numAnxiety += (int)(Math.Round(damage) * 1.5);
        anxiety.text = _numAnxiety.ToString();
    }

    public void Heal(float healingAmount)
    {
        _numSanity += (int) Math.Round(healingAmount);
        _numSanity = Mathf.Clamp(_numSanity, 0, _numMaxSanity);
        sanity.text = _numSanity.ToString();

        _numAnxiety -= (int)(Math.Round(healingAmount) * 1.5);
        _numAnxiety = Mathf.Clamp(_numAnxiety, 0, _numMaxAnxiety);
        anxiety.text = _numAnxiety.ToString();
    }

    public int GetSanity(){ return _numSanity; }
    public int GetAnxiety(){ return _numAnxiety; }
#endregion
}
