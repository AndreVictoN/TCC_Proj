using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArrivalManager : DialogueBox
{
    public List<string> arrivalDialogue = new();
    public List<Sprite> npcImages = new();
    public List<GameObject> npcs = new();
    public GameManager gameManager;
    private PlayerController _playercontroller;

    [SerializeField] private Image playerImage;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject npcName;
    [SerializeField] private Image npcImage;
    private float defaultTimeToReturn;

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
        defaultTimeToReturn = _secondsToReturn;
        _playercontroller = gm.GetPlayerController();
    }

    public IEnumerator FirstLines()
    {
        _playercontroller.SetCanMove(false);
        _canSkip = true;
        yield return new WaitForSeconds(3f);

        dialogue.Clear();
        for (int iterator = 0; iterator < 3; iterator++) { dialogue.Add(arrivalDialogue[iterator]); }

        StartDialogue();

        while (!_isClosed) { yield return null; }
        _playercontroller.SetCanMove(true);

        gameManager.instruction.text = "Pressione TAB para abrir o inventÁrio e conferir seu objetivo.";
        gameManager.instruction.gameObject.SetActive(true);
        gameManager.currentObjective.text = "OBJETIVO: Encontre alguma forma de descobrir sua sala de aula.";
        yield return new WaitForSeconds(5f);
        gameManager.instruction.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    protected void StartDialogue()
    {
        if (!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            _i = 0;

            SetDialoguePanel();
            StartCoroutine(Typing());
        }
    }

    void Update()
    {
        if (_canSkip && Input.GetKeyDown(KeyCode.Return) && _isTyping && !_isClosed)
        {
            StopCoroutine(Typing());
            dialogueText.text = dialogue[_i];
        }
        else if (_canSkip && Input.GetKeyDown(KeyCode.Return) && !_isTyping && !_isClosed)
        {
            NextLine();
        }
    }

    protected override void CheckCharacter(int i)
    {
        npcName = GameObject.FindGameObjectWithTag("NPC_Name");
        playerName = GameObject.FindGameObjectWithTag("PlayerName");

        if (i < 3 || i == 6 || i == 7)
        {
            if (i < 3){
                npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
                npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0f);
                playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
            }else if (i == 6 || i == 7){
                if(i == 6) _secondsToReturn = 8f;
                npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
                npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
                playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
                playerName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
            }else{
                npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
                playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
            }
            dialogueText.alignment = TextAlignmentOptions.Center;
            dialogueText.fontStyle = FontStyles.Normal;
        }else if (i == 3 || i == 5){
            if (i == 3) npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
            else { npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f); }
            playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            dialogueText.alignment = TextAlignmentOptions.Right;
            dialogueText.fontStyle = FontStyles.Italic;
        }else if (i == 4){
            npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
            npcName.GetComponent<TextMeshProUGUI>().text = "RÔmilo";
            npcImage.GetComponent<Image>().sprite = npcImages[0];
            npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
            dialogueText.alignment = TextAlignmentOptions.Left;
            dialogueText.fontStyle = FontStyles.Normal;
        }
    }

    public IEnumerator StartAutomaticTalk(int indexStart)
    {
        GameObject skipText = new();
        _isAutomatic = true;

        yield return new WaitForSeconds(0.5f);
        if (!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            _i = indexStart;

            SetDialoguePanel();
            skipText = GameObject.FindGameObjectWithTag("SkipText");
            skipText.SetActive(false);
            StartCoroutine(Typing());
        }

        while (_i != dialogue.Count - 1)
        {
            yield return null;

            if (!_isTyping)
            {
                yield return new WaitForSeconds(_secondsToReturn);
                NextLine();
            }
        }

        if (_i != 7)
        {
            if (skipText != null) skipText.SetActive(true);
            if (!_canSkip) _canSkip = true;
        }
    }

    public IEnumerator FirstInteractionScene()
    {
        if (_playercontroller == null) _playercontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playercontroller?.SetCanMove(false);
        _isAutomatic = true;
        _canSkip = false;

        for (int i = 3; i <= 7; i++) dialogue.Add(arrivalDialogue[i]);

        StartCoroutine(StartAutomaticTalk(3));

        yield return new WaitForSeconds(3f);

        foreach (GameObject npc in npcs)
        {
            npc.SetActive(true);
            if (npc == npcs[0]) { StartCoroutine(npc.GetComponent<NPC>().GoTo(3f, new Vector2(npc.transform.localPosition.x, npc.transform.localPosition.y - 6f), 'y')); }
            else { StartCoroutine(npc.GetComponent<NPC>().GoTo(3f, new Vector2(npc.transform.localPosition.x, npc.transform.localPosition.y - 4f), 'y')); }
        }

        yield return new WaitForSeconds(3f);

        npcs[1].GetComponent<SpriteRenderer>().sortingOrder = -1;
        StartCoroutine(npcs[0].GetComponent<NPC>().GoTo(1f, new Vector2(npcs[1].transform.localPosition.x, npcs[0].transform.localPosition.y), 'x'));
        StartCoroutine(npcs[2].GetComponent<NPC>().GoTo(1f, new Vector2(npcs[2].transform.localPosition.x, npcs[2].transform.localPosition.y - 3), 'y'));
        yield return new WaitForSeconds(1f);

        npcs[2].GetComponent<SpriteRenderer>().sortingOrder = 1;
        StartCoroutine(npcs[2].GetComponent<NPC>().GoTo(0.5f, new Vector2(npcs[1].transform.localPosition.x, npcs[2].transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(0.5f);

        while (_i != 6) { yield return null; }
        yield return new WaitForSeconds(2f);

        if (_playercontroller.gameObject.transform.localPosition.x > 23.01f) { _playercontroller.SetAnimation("H_WalkingLeft", 2); }
        if (_playercontroller.gameObject.transform.localPosition.x < 23.01f) { _playercontroller.SetAnimation("H_WalkingRight", 2); }
        StartCoroutine(_playercontroller.GoTo(1f, new Vector2(23.01f, _playercontroller.gameObject.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(1f);

        _playercontroller.SetAnimation("H_WalkingUp", 3);
        StartCoroutine(_playercontroller.GoTo(3f, new Vector2(_playercontroller.gameObject.transform.localPosition.x, 108f), 'y', false));
        yield return new WaitForSeconds(3f);

        _playercontroller.SetAnimation("H_WalkingRight", 2);
        StartCoroutine(_playercontroller.GoTo(2f, new Vector2(35.87f, _playercontroller.gameObject.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(2f);

        _playercontroller.SetAnimation("H_WalkingUp", 0.5f);
        StartCoroutine(_playercontroller.GoTo(3f, new Vector2(_playercontroller.gameObject.transform.localPosition.x, 104.67f), 'y', false));
        yield return new WaitForSeconds(3f);
        
        _playercontroller.SetAnimation("H_IdleUp", 0);
    }
}
