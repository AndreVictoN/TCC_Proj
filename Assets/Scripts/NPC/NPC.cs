using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class NPC : DialogueBox
{
    [Header("GeneralSettings")]
    public Color defaultColor;
    public Vector3 defaultPosition;

    [Header("Battle")]
    public Animator battleAnimator;
    public BattleManager battleManager;
    public SpriteRenderer spriteRenderer;
    public GameObject enemy;
    public float attackTime = 0.3f;
    public float fadeTime = 0.5f;

    #region Privates
    protected Coroutine _currentCoroutine;
    protected Tween _currentTween;
    protected bool _playerIsClose;
    protected bool _isBattling;
    protected String _npcName;
    protected bool _isMoving;
    protected PlayerController _player;
    protected bool _isAutomatic = false;
    #endregion

    void Start()
    {
        if(SceneManager.GetActiveScene().name != "BattleScene")
        {
            ResetText();
        }else{
            defaultPosition = getPosition();
            this.gameObject.transform.position = defaultPosition;
        }
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
            if(Input.GetKeyDown(KeyCode.E) && _playerIsClose && !_isTyping)
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
                    StopCoroutine(Typing());
                    ResetText();
                }
            }else if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return)) && _playerIsClose && _isTyping)
            {
                StopCoroutine(Typing());
                dialogueText.text = dialogue[_i];
                //continueButton.SetActive(true);
            }else if(Input.GetKeyDown(KeyCode.Return) && !_isTyping && _playerIsClose && dialoguePanel.activeSelf)
            {
                NextLine();
            }

            if(dialogueText.text == dialogue[_i])
            {
                //continueButton.SetActive(true);
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

        battleAnimator.SetTrigger("Attack");
    }

    public virtual void RecieveTrigger(GameObject player, string trigger) {}

    protected IEnumerator GoToPlayer(float time, GameObject player)
    {
        this.transform.DOMoveY(player.transform.localPosition.y, time);
        yield return new WaitForSeconds(time);

        this.transform.DOMoveX(player.transform.localPosition.x - 0.8f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        if(!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            _i = 0;

            SetDialoguePanel();
            StartCoroutine(Typing());
        }
    }

    protected IEnumerator GoTo(float time, Vector2 position)
    {
        this.transform.DOMoveX(position.x, time);
        yield return new WaitForSeconds(time);

        if(_isAutomatic) _isAutomatic = false;
    }

    protected IEnumerator StartAutomaticTalk()
    {
        GameObject skipText = new();

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
                yield return new WaitForSeconds(1.5f);
                NextLine();
            }
        }

        if(skipText != null) skipText.SetActive(true);
    }
}
