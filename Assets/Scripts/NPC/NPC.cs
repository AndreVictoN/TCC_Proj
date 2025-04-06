using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public GameObject continueButton;
    public TextMeshProUGUI dialogueText;
    public List<String> dialogue = new List<String>();
    public float wordSpeed;
    public bool playerIsClose;
    public bool isTyping;

    //Privates
    private int _i;

    void Start()
    {
        ResetText();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIsClose && !isTyping)
        {
            if(!dialoguePanel.activeSelf)
            {
                dialogueText.text = "";
                _i = 0;

                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
            else
            {
                dialoguePanel.SetActive(false);
                StopCoroutine(Typing());
                ResetText();
            }
        }else if(Input.GetKeyDown(KeyCode.E) && playerIsClose && isTyping)
        {
            StopCoroutine(Typing());
            dialogueText.text = dialogue[_i];
            continueButton.SetActive(true);
        }

        if(dialogueText.text == dialogue[_i])
        {
            continueButton.SetActive(true);
        }
    }

    public void ResetText()
    {
        dialogueText.text = "";
        _i = 0;

        if(dialoguePanel.activeSelf && dialoguePanel.activeInHierarchy) dialoguePanel.SetActive(false);
    }

    IEnumerator Typing()
    {
        isTyping = true;

        if(dialogueText.text != "")
        {
            dialogueText.text = "";
        }

        foreach(char letter in dialogue[_i].ToCharArray())
        {
            if(dialogueText.text != dialogue[_i])
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
        }

        isTyping = false;
    }

    public void NextLine()
    {
        continueButton.SetActive(false);

        if(_i < dialogue.Count - 1)
        {
            _i++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }else
        {
            ResetText();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
         if(collision.CompareTag("Player"))
         {
             playerIsClose = true;
         }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
         if(collision.CompareTag("Player"))
         {
             playerIsClose = false;
             ResetText();
         }
    }
}
