using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Singleton;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("Texts")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public List<string> dialogue = new List<string>();
    public float wordSpeed = 0.6f;

    private bool _isTyping;
    private bool _skipped;
    private bool _canSkip;
    private int _i;

    protected IEnumerator Typing()
    {
        _isTyping = true;

        if (dialogueText.text != "")
        {
            dialogueText.text = "";
        }

        foreach (char letter in dialogue[_i].ToCharArray())
        {
            if (dialogueText.text != dialogue[_i])
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
        }

        _isTyping = false;
    }

    public virtual void NextLine()
    {

        if(_i < dialogue.Count - 1)
        {
            _i++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }else
        {
            _i = 0;
            if(dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }

    protected IEnumerator StartAutomaticTalk(EventsEnum evt)
    {
        GameObject skipText = null;

        if (!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            dialogueText.alignment = TextAlignmentOptions.Center;
            dialogueText.fontStyle = FontStyles.Italic;
            dialoguePanel.SetActive(true);
            _i = 0;

            GameObject npcImage = GameObject.FindGameObjectWithTag("NPC_Image");
            npcImage.GetComponent<Image>().color = new Vector4(0, 0, 0, 0);

            GameObject npcName = GameObject.FindGameObjectWithTag("NPC_Name");
            npcName.GetComponent<TextMeshProUGUI>().text = "";

            GameObject playerImage = GameObject.FindGameObjectWithTag("Player_Image");
            playerImage.GetComponent<Image>().color = new Vector4(playerImage.GetComponent<Image>().color.r, playerImage.GetComponent<Image>().color.g, playerImage.GetComponent<Image>().color.b, 1f);

            skipText = GameObject.FindGameObjectWithTag("SkipText");
            skipText.SetActive(false);
            StartCoroutine(Typing());
        }

        while (_i != dialogue.Count - 1)
        {
            yield return null;

            if (!_isTyping)
            {
                yield return new WaitForSeconds(1f);
                NextLine();
            }
        }

        if (skipText != null) skipText.SetActive(true);

        _canSkip = true;
        _skipped = false;
    }
}
