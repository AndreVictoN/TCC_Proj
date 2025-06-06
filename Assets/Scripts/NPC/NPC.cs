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
    public TextMeshProUGUI continueButtonText;
    public TextMeshProUGUI dialogueText;
    public List<String> dialogue = new List<String>();
    public float wordSpeed;

    //Privates
    private bool _playerIsClose;
    private bool _isTyping;
    private int _i;

    void Start()
    {
        ResetText();
    }

    void Update()
    {
        UpdateNPC();
    }

    public virtual void UpdateNPC()
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
            continueButton.SetActive(true);
        }else if(Input.GetKeyDown(KeyCode.Return) && !_isTyping && _playerIsClose && dialoguePanel.activeSelf)
        {
            NextLine();
        }

        if(dialogueText.text == dialogue[_i])
        {
            continueButton.SetActive(true);
        }
    }

    public void SetDialoguePanel()
    {
        dialoguePanel.SetActive(true);

        GameObject npcImage = GameObject.FindGameObjectWithTag("NPC_Image");
        npcImage.GetComponent<Image>().color = this.gameObject.GetComponent<SpriteRenderer>().color;

        GameObject npcName = GameObject.FindGameObjectWithTag("NPC_Name");
        npcName.GetComponent<TextMeshProUGUI>().text = this.gameObject.name.ToString();
    }

    public virtual void ResetText()
    {
        dialogueText.text = "";
        _i = 0;

        if(dialoguePanel.activeSelf && dialoguePanel.activeInHierarchy) dialoguePanel.SetActive(false);
    }

    IEnumerator Typing()
    {
        _isTyping = true;

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

        _isTyping = false;
    }

    public virtual void NextLine()
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
}
