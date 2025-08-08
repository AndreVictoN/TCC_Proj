using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class DialogueBox : MonoBehaviour
{
    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public List<string> dialogue = new List<string>();
    public float wordSpeed;
    [SerializeField]protected float _secondsToReturn = 1.5f;
    protected bool _isClosed = true;
    protected bool _isAutomatic = false;

    [SerializeField] protected Sprite _npcSprite;
    protected int _i;
    protected bool _isTyping;
    protected int _currentDialogue = 1;
    protected bool _canSkip;
    protected bool _dialogueStarted = false;

    public virtual void SetDialoguePanel()
    {
        dialoguePanel.SetActive(true);
        _isClosed = false;

        GameObject npcImage = GameObject.FindGameObjectWithTag("NPC_Image");
        npcImage.GetComponent<Image>().sprite = this._npcSprite;
        npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);

        GameObject npcName = GameObject.FindGameObjectWithTag("NPC_Name");
        npcName.GetComponent<TextMeshProUGUI>().text = this.gameObject.name.ToString();

        GameObject playerImage = GameObject.FindGameObjectWithTag("Player_Image");
        playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 0.75f);
    }

    public virtual void ResetText()
    {
        dialogueText.text = "";
        dialogueText.alignment = TextAlignmentOptions.Left;
        _i = 0;

        if(dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
            _isClosed = true;
        }
    }

    protected IEnumerator Typing()
    {
        _isTyping = true;
        CheckCharacter(_i);

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
        //continueButton.SetActive(false);

        if(_i < dialogue.Count - 1)
        {
            _i++;
            CheckCharacter(_i);
            dialogueText.text = "";
            StartCoroutine(Typing());
        }else
        {
            ResetText();
        }
    }
    
    protected virtual void CheckCharacter(int i){}
    public void SetCanSkip(bool can){_canSkip = can;}
}
