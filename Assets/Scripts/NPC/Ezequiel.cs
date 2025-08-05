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
    private bool _inPrototype;

    void Awake()
    {
        BasicSettings();

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
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetCanMove(true);
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
        _player.SetAnimation("H_WalkingLeft");
        _isAutomatic = true;
        StartCoroutine(GoTo(77.5f, new Vector2(-40, this.gameObject.transform.position.y)));
        StartCoroutine(_player.GoTo(77.5f, new Vector2(-40 + 0.8f, this.gameObject.transform.position.y)));
        StartCoroutine(StartAutomaticTalk());
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

        if(trigger == "PrototypeEzequielTrigger1")
        {
            StartCoroutine(GoToPlayer(2f, player));
        }
    }
}
