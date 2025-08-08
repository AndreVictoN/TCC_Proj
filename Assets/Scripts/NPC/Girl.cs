using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Girl : NPC
{
    public List<string> prototypeDialogue = new();

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
                if(wordSpeed != 0.06f) wordSpeed = 0.06f;

                if(i == 0 || i == 3 || i == 6){
                    dialogueText.alignment = TextAlignmentOptions.Right;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 1);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);

                    if(i == 3) wordSpeed = 0.3f;
                    if(i == 6) wordSpeed = 0.03f;
                }else if(i == 4)
                {
                    dialogueText.alignment = TextAlignmentOptions.Center;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);
                }else{
                    dialogueText.alignment = TextAlignmentOptions.Left;
                    playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 1);

                    if(i == 5) wordSpeed = 0.2f;
                }
            }
        }
    }

    public override void UpdateNPC()
    {
        base.UpdateNPC();
        if(Input.GetKeyDown(KeyCode.Return) && !_isTyping && _isAutomatic && dialoguePanel.activeSelf){ NextLine(); }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerIsClose = false;
        }
    }

    public void Move(float time, Vector2 position, char xy)
    {
        StartCoroutine(GoTo(time, position, xy));
    }

    public override void RecieveTrigger(GameObject player, string trigger)
    {
        if(player == null) return;

        if(trigger == "GirlTrigger")
        {
            StartDialogue();
            StartCoroutine(GoToPlayer(2f, player, trigger));
        }
    }
}
