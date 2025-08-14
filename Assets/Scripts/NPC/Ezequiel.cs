using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ezequiel : NPC
{
    public List<String> prototypeDialogue = new();
    public List<String> prototypeDialogue2 = new();

    void Awake()
    {
        BasicSettings();
        
        this.gameObject.GetComponent<CircleCollider2D>().enabled = true;
        if(SceneManager.GetActiveScene().name == "PrototypeScene"){dialogue = prototypeDialogue; _inPrototype = true;}
    }

    protected override void CheckCharacter(int i)
    {
        GameObject playerImage = GameObject.FindGameObjectWithTag("Player_Image");
        GameObject npcImage = GameObject.FindGameObjectWithTag("NPC_Image");
        
        if(_inPrototype)
        {
            if(_currentDialogue == 1)
            {
                if(i == 2 || i == 5 || i == 11 || i == 15 || i == 17){
                    dialogueText.alignment = TextAlignmentOptions.Right;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 1);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);

                    if(i == 17){_currentDialogue = 2;}
                }else if(i == 6)
                {
                    dialogueText.alignment = TextAlignmentOptions.Center;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);
                }else{
                    dialogueText.alignment = TextAlignmentOptions.Left;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 1);
                }
            }else if(_currentDialogue == 2)
            {
                if(i == 7 || i == 15){
                    dialogueText.alignment = TextAlignmentOptions.Right;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 1);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);
                }else if(i == 2 || i == 8 || i == 14)
                {
                    dialogueText.alignment = TextAlignmentOptions.Center;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);
                }else{
                    dialogueText.alignment = TextAlignmentOptions.Left;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 1);
                }
            }
        }
    }

    public override void UpdateNPC()
    {
        if(_currentDialogue == 2 && !dialoguePanel.activeSelf && !_isTyping && dialogue != prototypeDialogue2){
            dialogue = prototypeDialogue2;
            SetupSecondDialogue();
        }

        base.UpdateNPC();
    }

    protected void SetupSecondDialogue()
    {
        if(_player == null){ _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); }
        _isAutomatic = true;
        StartCoroutine(Moving());
        StartCoroutine(StartAutomaticTalk());
    }

    private IEnumerator Moving()
    {
        _isMoving = true;

        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        this.gameObject.GetComponent<CircleCollider2D>().radius = 2;

        _player.SetAnimation("H_WalkingLeft", 0);
        animator.Play("H_WalkingL");
        //77.5f
        StartCoroutine(GoTo(3f, new Vector2(1.88f, this.gameObject.transform.position.y), 'x'));
        StartCoroutine(_player.GoTo(3.3f, new Vector2(3.64f, this.gameObject.transform.position.y), 'x', false));
        yield return new WaitForSeconds(3f);

        animator.Play("H_WalkingD");
        _player.SetAnimation("H_WalkingDown", 0);
        StartCoroutine(GoTo(16f, new Vector2(this.gameObject.transform.position.x, 63.3f), 'y'));
        StartCoroutine(_player.GoTo(16f, new Vector2(_player.gameObject.transform.position.x, 65.5f), 'y', false));
        yield return new WaitForSeconds(16f);
        
        _player.SetAnimation("H_Idle", 0);
        animator.Play("H_WalkingR");
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        StartCoroutine(GoTo(20f, new Vector2(38f, this.gameObject.transform.position.y), 'x'));
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        _player.SetAnimation("H_WalkingDown", 0);
        StartCoroutine(_player.GoTo(0.5f, new Vector2(_player.gameObject.transform.position.x, this.gameObject.transform.position.y), 'y', false));
        yield return new WaitForSeconds(0.5f);

        _player.SetAnimation("H_WalkingRight", 0);
        StartCoroutine(_player.GoTo(20f, new Vector2(36f, this.gameObject.transform.position.y), 'x', false));
        yield return new WaitForSeconds(20);

        _player.SetAnimation("H_WalkingUp", 0);
        animator.Play("H_WalkingU");
        StartCoroutine(GoTo(27f, new Vector2(this.gameObject.transform.position.x, 92f), 'y'));
        StartCoroutine(_player.GoTo(27f, new Vector2(_player.gameObject.transform.position.x, 90f), 'y', false));
        yield return new WaitForSeconds(27);

        _player.SetAnimation("H_IdleUp", 0);
        animator.Play("H_WalkingL");
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        StartCoroutine(GoTo(10, new Vector2(18.79f, this.gameObject.transform.position.y), 'x'));
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;

        _player.SetAnimation("H_WalkingUp", 0);
        StartCoroutine(_player.GoTo(0.5f, new Vector2(_player.gameObject.transform.position.x, this.gameObject.transform.position.y), 'y', false));
        yield return new WaitForSeconds(0.5f);

        _player.SetAnimation("H_WalkingLeft", 0);
        StartCoroutine(_player.GoTo(10f, new Vector2(21f, this.gameObject.transform.position.y), 'x', true));
        yield return new WaitForSeconds(10f);
        animator.Play("H_IdleR");

        this.gameObject.GetComponent<CircleCollider2D>().radius = 0.9f;
        _isMoving = false;

        while(dialoguePanel.activeSelf) yield return null;
        
        this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }

    protected override Vector3 getPosition()
    {
        return new Vector3(-5.3f, 2.5f, 0);
    }

    protected override Color CheckColorAspectByNPC()
    {
        Color colorToFade = spriteRenderer.color;

        colorToFade.b -= 0.7f;

        return colorToFade;
    }

    public override void RecieveTrigger(GameObject player, string trigger)
    {
        if(player == null) return;
        player.GetComponent<PlayerController>().SetCanMove(false);

        if(trigger == "PrototypeEzequielTrigger1")
        {
            //StartCoroutine(GoToPlayer(2f, player, trigger));
            StartCoroutine(StartEzequielBehaviour());
        }
    }

    private IEnumerator StartEzequielBehaviour()
    {
        yield return new WaitForSeconds(3);
        if(!_dialogueStarted) StartDialogue();
    }

    protected override void BattleSettings()
    {
        if(sanity == null) sanity = GameObject.FindGameObjectWithTag("EzAnxiety").GetComponent<TextMeshProUGUI>();
        if(maxSanity == null) maxSanity = GameObject.FindGameObjectWithTag("EzMaxSanity").GetComponent<TextMeshProUGUI>();
        if(anxiety == null) anxiety = GameObject.FindGameObjectWithTag("EzAnxiety").GetComponent<TextMeshProUGUI>();
        if(maxAnxiety == null) maxAnxiety = GameObject.FindGameObjectWithTag("EzMaxAnxiety").GetComponent<TextMeshProUGUI>();

        base.BattleSettings();
    }
}
