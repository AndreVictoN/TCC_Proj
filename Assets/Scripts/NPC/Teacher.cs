using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Teacher : NPC
{
    public List<string> prototypeDialogue = new();

    void Awake()
    {
        if(SceneManager.GetActiveScene().name == "PrototypeScene")
        {
            dialogue = prototypeDialogue;
            _inPrototype = true;
        }else
        {
            _inPrototype = false;
        }
    }

    protected override void CheckCharacter(int i)
    {
        GameObject playerImage = GameObject.FindGameObjectWithTag("Player_Image");
        GameObject npcImage = GameObject.FindGameObjectWithTag("NPC_Image");
        
        if(_inPrototype)
        {
            if(_currentDialogue == 1)
            {
                dialogueText.fontStyle = FontStyles.Normal;
                if(i == 0 || i == 2){
                    dialogueText.alignment = TextAlignmentOptions.Right;
                    if(playerImage != null) playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 1);
                    if(npcImage != null) npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 0.75f);
                }else{
                    dialogueText.alignment = TextAlignmentOptions.Left;
                    if(playerImage != null) playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
                    if(npcImage != null) npcImage.GetComponent<Image>().color = new Vector4(npcImage.GetComponent<Image>().color.r, npcImage.GetComponent<Image>().color.g, npcImage.GetComponent<Image>().color.b, 1);
                }
            }
        }
    }

    public override void UpdateNPC()
    {
        base.UpdateNPC();
        if(Input.GetKeyDown(KeyCode.Return) && !_isTyping && _isAutomatic && dialoguePanel.activeSelf && _canSkip){ NextLine(); }
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
}
