using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArrivalManager : DialogueBox
{
    public List<string> arrivalDialogue = new();
    public List<Sprite> playerImages = new();
    public List<Sprite> npcImages = new();
    public List<GameObject> npcs = new();
    public GameManager gameManager;
    private PlayerController _playercontroller;

    [SerializeField] private CinemachineCamera conversationView;
    [SerializeField] private CinemachineCamera estellaView;
    [SerializeField] private CinemachineCamera ezequielView;
    [SerializeField] private Door toOtherFloorDoor;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject npcName;
    [SerializeField] private Door classroomDoor;
    [SerializeField] private Image playerImage;
    [SerializeField] private NPC estella;
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

    private void DialoguePanelSettings(float playerNameAlpha, float playerImageAlpha, float npcNameAlpha, float npcImageAlpha, TextAlignmentOptions textAlignment, FontStyles fontStyle)
    {
        if (npcName == null || npcImage == null || playerName == null || playerImage == null || dialogueText == null) return;

        npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, npcNameAlpha);
        npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, npcImageAlpha);
        playerName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, playerNameAlpha);
        playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, playerImageAlpha);
        dialogueText.alignment = textAlignment;
        dialogueText.fontStyle = fontStyle;
    }

    protected override void CheckCharacter(int i)
    {
        npcName = GameObject.FindGameObjectWithTag("NPC_Name");
        playerName = GameObject.FindGameObjectWithTag("PlayerName");

        if (i < 3 || i == 6 || i == 7 || (i >= 12 && i <= 14) || (i >= 17 && i <= 19))
        {
            if (i < 3)
            {
                DialoguePanelSettings(0f, 0.5f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal);
                /*npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
                npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0f);
                playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);*/
            }
            else if (i == 6 || i == 7)
            {
                if (i == 6) _secondsToReturn = 8f;
                DialoguePanelSettings(0f, 0f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal);
                /*npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
                npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
                playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0);
                playerName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);*/
            }
            else if (i == 12) { _secondsToReturn = 11f; DialoguePanelSettings(0f, 0f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal); }
            else if (i == 13) { _secondsToReturn = 1.5f; }
            else if (i >= 14 && i <= 19)
            {
                if (i == 17)
                {
                    npcs[2].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[2];
                    npcs[2].transform.Find("part2").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[5];
                    npcs[3].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[4];
                    npcs[4].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[3];
                    conversationView.gameObject.SetActive(true);
                    estellaView.gameObject.SetActive(false);
                }else if(i == 18) { _secondsToReturn = 2.5f; }
                DialoguePanelSettings(0f, 0f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else
            {
                DialoguePanelSettings(1f, 0.5f, 1f, 0.5f, TextAlignmentOptions.Center, FontStyles.Normal);
                /*npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
                playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);*/
            }
        }
        else if (i == 3 || i == 5 || i == 10 || (i >= 20 && i <= 22))
        {
            if (i == 3 || (i >= 20 && i <= 22))
            {
                if (i == 21)
                {
                    _secondsToReturn = 2.5f;
                    playerImage.sprite = playerImages[2];
                }else if(i == 22){ playerImage.sprite = playerImages[1]; }
                DialoguePanelSettings(1f, 1f, 0f, 0f, TextAlignmentOptions.Right, FontStyles.Italic);
            }
            else if (i == 10) { DialoguePanelSettings(1f, 1f, 1f, 0.5f, TextAlignmentOptions.Right, FontStyles.Normal); }
            else { DialoguePanelSettings(1f, 1f, 1f, 0.5f, TextAlignmentOptions.Right, FontStyles.Italic); }
            /*playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            dialogueText.alignment = TextAlignmentOptions.Right;
            dialogueText.fontStyle = FontStyles.Italic;*/
        }
        else if (i == 4 || i == 8 || i == 9 || i == 15 || i == 16 || i == 11)
        {
            if (i == 4)
            {
                npcName.GetComponent<TextMeshProUGUI>().text = "RÔmilo";
                npcImage.GetComponent<Image>().sprite = npcImages[0];
            }
            else if (i == 8)
            {
                npcName.GetComponent<TextMeshProUGUI>().text = "Assilon";
                npcImage.GetComponent<Image>().sprite = npcs[0].GetComponent<NPC>().GetNPCSprite();
                DialoguePanelSettings(0f, 0f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Normal);
            }
            else if (i == 9) { npcs[0].GetComponent<Animator>().Play("IdleR"); }
            else if (i == 15 || i == 16)
            {
                npcName.GetComponent<TextMeshProUGUI>().text = "?";
                npcImage.GetComponent<Image>().sprite = npcImages[0];
                DialoguePanelSettings(0f, 0f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Italic);
            }
            else { DialoguePanelSettings(1f, 0.5f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Normal); }

            /*npcName.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 1);
            npcImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            playerImage.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
            dialogueText.alignment = TextAlignmentOptions.Left;
            dialogueText.fontStyle = FontStyles.Normal;*/
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
        _playercontroller.SetCanMove(true);
    }

    public IEnumerator FirstInteractionScene()
    {
        BasicPlayerCutsceneConfig();

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
        EstellaFirstSetup();
        estellaView.gameObject.SetActive(true);
        estellaView.Follow = estella.gameObject.transform;
        StartCoroutine(estella.GoTo(3f, new Vector2(estella.gameObject.transform.localPosition.x, 110.3f), 'y'));
        yield return new WaitForSeconds(3f);

        estella.gameObject.GetComponent<Animator>().Play("WalkingRight");
        StartCoroutine(estella.GoTo(3f, new Vector2(17.91f, estella.gameObject.transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(3f);

        estella.gameObject.GetComponent<Animator>().Play("WalkingUp");
        estella.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(estella.GoTo(1f, new Vector2(estella.gameObject.transform.localPosition.x, 112.24f), 'y'));
        yield return new WaitForSeconds(1f);

        estella.gameObject.GetComponent<Animator>().Play("Idle_U");
        toOtherFloorDoor.ChangeState(true);
        yield return new WaitForSeconds(0.5f);

        estella.gameObject.GetComponent<Animator>().Play("WalkingUp");
        StartCoroutine(estella.GoTo(1f, new Vector2(estella.gameObject.transform.localPosition.x, 112.9f), 'y'));
        yield return new WaitForSeconds(1f);

        estella.gameObject.GetComponent<Animator>().Play("Idle_U");
        yield return new WaitForSeconds(0.5f);

        estella.gameObject.SetActive(false);
        toOtherFloorDoor.GetComponent<BoxCollider2D>().enabled = false;
        toOtherFloorDoor.ChangeState(false);

        gameManager.AnimateTransition(1f, false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Class");
    }

    public IEnumerator FirstClass()
    {
        BasicPlayerCutsceneConfig();

        for (int i = 0; i <= 19/*11*/; i++) dialogue.Add(arrivalDialogue[i]);
        _playercontroller.gameObject.SetActive(false);
        if (!npcs[0].gameObject.activeSelf) npcs[0].SetActive(true);
        npcs[0].GetComponent<Animator>().Play("IdleD");

        StartCoroutine(StartAutomaticTalk(8));

        yield return new WaitForSeconds(5f);

        classroomDoor.ChangeState(true);
        _playercontroller.gameObject.SetActive(true);
        yield return new WaitForSeconds(8f);

        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingLeft");
        _playercontroller.SetSpeed(1f);
        StartCoroutine(_playercontroller.GoTo(1f, new Vector2(3.91f, _playercontroller.gameObject.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(1f);

        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingDown");
        StartCoroutine(_playercontroller.GoTo(10f, new Vector2(_playercontroller.gameObject.transform.localPosition.x, -8.550989f), 'y', false));
        npcs[0].GetComponent<Animator>().Play("IdleD");
        yield return new WaitForSeconds(10f);

        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingRight");
        _playercontroller.SetSpeed(0.5f);
        StartCoroutine(_playercontroller.GoTo(2f, new Vector2(5.9964f, _playercontroller.gameObject.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(2f);

        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleUp");
        yield return new WaitForSeconds(1f);

        estellaView.gameObject.SetActive(true);
        npcs[1].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[1];

        while (_i != 19) { yield return null; }
        yield return new WaitForSeconds(3f);

        gameManager.AnimateTransition(1f, false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Floor2");
    }

    public IEnumerator FirstConflictSequence()
    {
        BasicPlayerCutsceneConfig();
        for (int i = 0; i <= 50; i++) dialogue.Add(arrivalDialogue[i]);

        StartCoroutine(StartAutomaticTalk(20));
        yield return new WaitForSeconds(2f);
        ezequielView.gameObject.SetActive(true);

        while (_i != 22) { yield return null; }
        estella.gameObject.transform.localPosition = new Vector2(21.79f, _playercontroller.gameObject.transform.localPosition.y);
        estella.gameObject.SetActive(true);
        estella.gameObject.GetComponent<Animator>().Play("Idle_L");
        estellaView.gameObject.transform.localPosition = new Vector3(estellaView.gameObject.transform.localPosition.x, estella.gameObject.transform.localPosition.y, -10);
        estellaView.gameObject.SetActive(true);
        ezequielView.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);
        estellaView.gameObject.SetActive(false);
    }

    private void BasicPlayerCutsceneConfig()
    {
        if (_playercontroller == null) _playercontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playercontroller?.SetCanMove(false);
        _isAutomatic = true;
        _canSkip = false;
    }

    private void EstellaFirstSetup()
    {
        estella.gameObject.SetActive(true);
        estella.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
        estella.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        estella.gameObject.GetComponent<Animator>().Play("WalkingUp");
    }
}
