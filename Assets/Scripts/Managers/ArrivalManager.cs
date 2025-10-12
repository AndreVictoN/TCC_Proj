using System;
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
    [SerializeField] private GameObject battleTrigger;
    [SerializeField] private Door toOtherFloorDoor;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject npcName;
    [SerializeField] private Door classroomDoor;
    [SerializeField] private Image illustration;
    [SerializeField] private Image playerImage;
    [SerializeField] private Sprite paperClass;
    [SerializeField] private NPC estella;
    [SerializeField] private Image npcImage;
    private float defaultTimeToReturn;

    void Awake()
    {
        if (battleTrigger != null) battleTrigger.SetActive(false);
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Floor2") && PlayerPrefs.GetString("currentState").Equals("Start") && PlayerPrefs.GetString("pastScene").Equals("BattleScene"))
        {
            StartCoroutine(PostBattleArrival());
        }
    }

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

        if (i < 3 || i == 6 || i == 7 || (i >= 12 && i <= 14) || (i >= 17 && i <= 19) || i == 23 || i == 24 || i == 27 || i == 33 || i == 35 || i == 40 || i == 43 || i == 51 || i == 52 || i == 56 || i == 58 || i == 59)
        {
            if (i < 3)
            {
                DialoguePanelSettings(0f, 0.5f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else if (i == 6 || i == 7 || i == 52 || i == 56)
            {
                if (i == 6) _secondsToReturn = 8f;
                if (i == 56) _secondsToReturn = 5f;
                DialoguePanelSettings(0f, 0f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else if (i == 12) { _secondsToReturn = 11f; DialoguePanelSettings(0f, 0f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal); }
            else if (i == 13) { _secondsToReturn = 1.5f; }
            else if ((i >= 14 && i <= 19) || i == 23 || i == 24)
            {
                if (i == 17)
                {
                    npcs[2].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[2];
                    npcs[2].transform.Find("part2").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[5];
                    npcs[3].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[4];
                    npcs[4].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[3];
                    conversationView.gameObject.SetActive(true);
                    estellaView.gameObject.SetActive(false);
                }
                else if (i == 18) { _secondsToReturn = 2.5f; }
                else if (i == 23) { _secondsToReturn = 2f; }
                else if (i == 24) { wordSpeed = 0.07f; _secondsToReturn = 3f; }
                DialoguePanelSettings(0f, 0f, 0f, 0f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else if (i == 33)
            {
                _secondsToReturn = 2.5f;
                playerImage.GetComponent<Image>().sprite = playerImages[4];
                DialoguePanelSettings(1f, 0.5f, 1f, 0.5f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else if (i == 40)
            {
                playerImage.GetComponent<Image>().sprite = playerImages[2];
                DialoguePanelSettings(1f, 0.5f, 1f, 0.5f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else if(i == 59)
            {
                playerImage.GetComponent<Image>().sprite = playerImages[1];
                DialoguePanelSettings(1f, 0.5f, 1f, 0.5f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
            else
            {
                DialoguePanelSettings(1f, 0.5f, 1f, 0.5f, TextAlignmentOptions.Center, FontStyles.Normal);
            }
        }
        else if (i == 3 || i == 5 || i == 10 || (i >= 20 && i <= 22) || i == 25 || i == 26 || i == 34 || i == 36 || i == 38 || i == 41 || i == 44 || i == 47 || i == 50 || i == 60 || i == 63)
        {
            if (i == 3 || (i >= 20 && i <= 22) || i == 25)
            {
                if(i == 3){ _secondsToReturn = 10f; }
                if (i == 21)
                {
                    _secondsToReturn = 2.5f;
                    playerImage.sprite = playerImages[2];
                }
                else if (i == 22) { playerImage.sprite = playerImages[1]; _secondsToReturn = 5f; }
                else if (i == 25) { playerImage.sprite = playerImages[3]; wordSpeed = 0.05f; _secondsToReturn = 1.5f; }
                DialoguePanelSettings(1f, 1f, 0f, 0f, TextAlignmentOptions.Right, FontStyles.Italic);
            }
            else if (i == 10) { DialoguePanelSettings(1f, 1f, 1f, 0.5f, TextAlignmentOptions.Right, FontStyles.Normal); }
            else if (i == 34) { DialoguePanelSettings(1f, 1f, 1f, 0.5f, TextAlignmentOptions.Right, FontStyles.Normal); wordSpeed = 0.05f; }
            else
            {
                if (i == 26) { npcImage.GetComponent<Image>().sprite = npcImages[0]; npcName.GetComponent<TextMeshProUGUI>().text = "?"; }
                DialoguePanelSettings(1f, 1f, 1f, 0.5f, TextAlignmentOptions.Right, FontStyles.Italic);
            }
        }
        else if (i == 4 || i == 8 || i == 9 || i == 15 || i == 16 || i == 11 || (i >= 28 && i <= 32) || i == 37 || i == 39 || i == 42 || i == 45 || i == 46 || i == 48 || i == 49 || (i >= 53 && i <= 55) || i == 57 || i == 61 || i == 62)
        {
            if (i == 4)
            {
                _secondsToReturn = 1.5f;
                npcName.GetComponent<TextMeshProUGUI>().text = "RÔmilo";
                npcImage.GetComponent<Image>().sprite = npcImages[0];
                DialoguePanelSettings(1f, 0.5f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Normal);
            }
            else if (i == 8 || (i >= 53 && i <= 55))
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
            else if (i >= 29 && i <= 32)
            {
                DialoguePanelSettings(1f, 0f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Normal);
                npcImage.GetComponent<Image>().sprite = npcImages[0]; npcName.GetComponent<TextMeshProUGUI>().text = "?";
                _secondsToReturn = 2.5f;
            }
            else if (i == 46 || i == 57) {
                if (i == 57)
                {
                    _secondsToReturn = 1.5f;
                    npcImage.GetComponent<Image>().sprite = npcImages[1];
                    DialoguePanelSettings(1f, 0.5f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Normal);
                }
                
                npcName.GetComponent<TextMeshProUGUI>().text = "Estella";
            }
            else { DialoguePanelSettings(1f, 0.5f, 1f, 1f, TextAlignmentOptions.Left, FontStyles.Normal); }
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

        if (_i != 7 && _i != 28 && _i != 51 && _i != 55)
        {
            if (skipText != null) skipText.SetActive(true);
            if (!_canSkip) _canSkip = true;
        }
        if(_i != 28 && _i != 51 && _i != 55 && _i != 63) _playercontroller.SetCanMove(true);
    }

    public IEnumerator FirstInteractionScene()
    {
        BasicPlayerCutsceneConfig();

        for (int i = 3; i <= 7; i++) dialogue.Add(arrivalDialogue[i]);

        StartCoroutine(StartAutomaticTalk(3));

        while (_i != 3) { yield return null; }
        yield return new WaitForSeconds(2f);

        illustration.gameObject.SetActive(true);
        illustration.gameObject.GetComponent<Image>().sprite = paperClass;

        yield return new WaitForSeconds(6f);

        illustration.gameObject.GetComponent<Animator>().Play("BgDeactivate");
        yield return new WaitForSeconds(2f);
        illustration.gameObject.SetActive(false);

        foreach (GameObject npc in npcs)
        {
            npc.SetActive(true);
            if (npc == npcs[0]) { StartCoroutine(npc.GetComponent<NPC>().GoTo(3f, new Vector2(npc.transform.localPosition.x, npc.transform.localPosition.y - 6f), 'y')); }
            else { StartCoroutine(npc.GetComponent<NPC>().GoTo(3f, new Vector2(npc.transform.localPosition.x, npc.transform.localPosition.y - 4f), 'y')); }
            npc.GetComponent<Animator>().Play("Walking");
        }

        yield return new WaitForSeconds(3f);

        npcs[1].GetComponent<Animator>().Play("IdleR");
        npcs[1].GetComponent<SpriteRenderer>().sortingOrder = -1;
        npcs[0].GetComponent<Animator>().Play("WalkingR");
        StartCoroutine(npcs[0].GetComponent<NPC>().GoTo(1f, new Vector2(npcs[1].transform.localPosition.x, npcs[0].transform.localPosition.y), 'x'));
        StartCoroutine(npcs[2].GetComponent<NPC>().GoTo(1f, new Vector2(npcs[2].transform.localPosition.x, npcs[2].transform.localPosition.y - 3), 'y'));
        yield return new WaitForSeconds(1f);
        npcs[0].GetComponent<Animator>().Play("IdleR");


        npcs[2].GetComponent<SpriteRenderer>().sortingOrder = 1;
        npcs[2].GetComponent<Animator>().Play("WalkingR");
        StartCoroutine(npcs[2].GetComponent<NPC>().GoTo(0.5f, new Vector2(npcs[1].transform.localPosition.x, npcs[2].transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(0.5f);
        npcs[2].GetComponent<Animator>().Play("IdleR");

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
        PlayerPrefs.SetString("pastScene", "Terreo");
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
        for (int i = 0; i <= 28; i++) dialogue.Add(arrivalDialogue[i]);

        StartCoroutine(StartAutomaticTalk(20));
        yield return new WaitForSeconds(2f);
        ezequielView.gameObject.SetActive(true);

        while (_i != 22) { yield return null; }
        estella.gameObject.transform.localPosition = new Vector2(21.79f, _playercontroller.gameObject.transform.localPosition.y + 0.78f);
        estella.gameObject.SetActive(true);
        estella.gameObject.GetComponent<Animator>().Play("Idle_L");
        estellaView.gameObject.transform.localPosition = new Vector3(estellaView.gameObject.transform.localPosition.x, estella.gameObject.transform.localPosition.y, -10);
        estellaView.gameObject.SetActive(true);
        ezequielView.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);
        estellaView.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingRight");
        _playercontroller.SetSpeed(2f);
        StartCoroutine(_playercontroller.GoTo(3f, new Vector2(estella.gameObject.transform.localPosition.x, _playercontroller.gameObject.transform.localPosition.y), 'x', false));

        yield return new WaitForSeconds(2f);
        estella.gameObject.GetComponent<Animator>().Play("Idle_D");
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleUp");
        StartCoroutine(estella.GoTo(0.3f, new Vector2(estella.gameObject.transform.localPosition.x, estella.gameObject.transform.localPosition.y + 0.5f), 'y'));
        StartCoroutine(_playercontroller.GoTo(0.3f, new Vector2(_playercontroller.gameObject.transform.localPosition.x, _playercontroller.gameObject.transform.localPosition.y - 0.5f), 'y', false));

        while (_i != 27) { yield return null; }
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingRight");
        _playercontroller.SetSpeed(2f);
        StartCoroutine(_playercontroller.GoTo(0.5f, new Vector2(24.73f, _playercontroller.gameObject.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(0.5f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingUp");
        _playercontroller.SetSpeed(2.5f);
        StartCoroutine(_playercontroller.GoTo(2.5f, new Vector2(_playercontroller.gameObject.transform.localPosition.x, 14.32541f), 'y', false));
        yield return new WaitForSeconds(2f);
        estellaView.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        _playercontroller.gameObject.transform.localPosition = new Vector2(23.46f, _playercontroller.gameObject.transform.localPosition.y);
        _playercontroller.gameObject.GetComponent<Animator>().Play("TransformR");
        estellaView.gameObject.SetActive(false);

        while (_i != 28) { yield return null; }
        if (!battleTrigger) battleTrigger = GameObject.FindGameObjectWithTag("BattleTrigger");
        yield return new WaitForSeconds(2f);
        battleTrigger?.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        Destroy(battleTrigger);
    }

    private IEnumerator PostBattleArrival()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        BasicPlayerCutsceneConfig();
        for (int i = 0; i <= 51; i++) dialogue.Add(arrivalDialogue[i]);

        _playercontroller.gameObject.GetComponent<Animator>().Play("H_SittingOnFloor");
        estella.gameObject.SetActive(true);
        estella.gameObject.GetComponent<Animator>().Play("Idle_L");
        estella.gameObject.transform.localPosition = new Vector2(_playercontroller.gameObject.transform.localPosition.x + 1.5f, 14.79f);
        StartCoroutine(StartAutomaticTalk(29));
        yield return null;

        while (_i != 40) yield return null;
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_Idle");
        yield return new WaitForSeconds(1f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleR");

        while (_i != 51) yield return null;
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_Idle");
        estella.gameObject.GetComponent<Animator>().Play("Idle_D");
        yield return new WaitForSeconds(0.5f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_WalkingDown");
        StartCoroutine(_playercontroller.GoTo(5f, new Vector2(_playercontroller.gameObject.transform.localPosition.x, 2.6f), 'y', false));
        estella.gameObject.GetComponent<Animator>().Play("WalkingDown");
        StartCoroutine(estella.GoTo(5f, new Vector2(estella.gameObject.transform.localPosition.x, 3.6f), 'y'));
        yield return new WaitForSeconds(3f);

        gameManager = GameManager.Instance;
        gameManager.AnimateTransition(1.5f, false);
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetString("pastScene", "Floor2");
        SceneManager.LoadScene("Class");
    }

    public IEnumerator SecondClass()
    {
        BasicPlayerCutsceneConfig();

        for (int i = 0; i <= 55; i++) dialogue.Add(arrivalDialogue[i]);
        _playercontroller.gameObject.SetActive(false);
        if (!npcs[0].gameObject.activeSelf) npcs[0].SetActive(true);
        npcs[0].GetComponent<Animator>().Play("IdleD");

        StartCoroutine(StartAutomaticTalk(52));
        while (_i != 55) yield return null;
        yield return new WaitForSeconds(3f);

        gameManager.AnimateTransition(1f, false);
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetString("currentState", "FirstLeaving");
        SceneManager.LoadScene("Floor2");
    }

    public IEnumerator FirstLeavingConfig()
    {
        foreach (GameObject npc in npcs)
        {
            if (npc.name.Equals("Ezequiel") || npc.name.Equals("Estella")) npc.SetActive(false);
        }

        if (_playercontroller == null) _playercontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playercontroller?.SetCanMove(true);
        _canSkip = true;

        if (gameManager.instruction == null) Debug.Log("Bongers");
        gameManager.instruction.text = "Confira seu novo objetivo (TAB)";
        gameManager.instruction.gameObject.SetActive(true);
        if (gameManager.currentObjective == null) Debug.Log("Chongers");
        gameManager.currentObjective.text = "Objetivo: Saia da escola";
        yield return new WaitForSeconds(5f);
        gameManager.instruction.gameObject.SetActive(false);
    }

    public IEnumerator ExitFirstDay()
    {
        BasicPlayerCutsceneConfig();
        gameManager.AnimateTransition(1f, false);
        yield return new WaitForSeconds(1f);
        foreach (String line in arrivalDialogue) dialogue.Add(line);
        StartCoroutine(StartAutomaticTalk(56));
        estella.gameObject.SetActive(true);
        estella.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        estella.gameObject.transform.localPosition = new Vector2(7.2f, 2f);
        _playercontroller.gameObject.transform.localPosition = new Vector2(7.2f, 0.7f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_Idle");
        yield return new WaitForSeconds(5f);
        gameManager.AnimateTransition(2f, true);

        while (_i != 58) { yield return null; }
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleR");
        yield return new WaitForSeconds(1);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleUp");

        while (dialoguePanel.activeSelf) { yield return null; }
        estella.gameObject.GetComponent<Animator>().Play("WalkingRight");
        StartCoroutine(estella.GoTo(2f, new Vector2(11.3f, estella.gameObject.transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(1f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleR");
        yield return new WaitForSeconds(1f);
        estella.gameObject.GetComponent<Animator>().Play("WalkingDown");
        StartCoroutine(estella.GoTo(3f, new Vector2(estella.gameObject.transform.localPosition.x, -6.3f), 'y'));
        yield return new WaitForSeconds(1f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_Idle");
        yield return new WaitForSeconds(2f);
        estella.gameObject.GetComponent<Animator>().Play("WalkingRight");
        StartCoroutine(estella.GoTo(3f, new Vector2(22.5f, estella.gameObject.transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(3f);

        gameManager.AnimateTransition(1f, false);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("GoToNextDay");
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
