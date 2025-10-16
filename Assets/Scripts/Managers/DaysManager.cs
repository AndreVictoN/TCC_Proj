using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UI;

public class DaysManager : DialogueBox
{
    public List<string> secondDayDialogue = new();
    public List<string> thirdDayDialogue = new();
    public List<string> fourthDayDialogue = new();
    public List<Sprite> playerImages = new();
    public List<Sprite> npcImages = new();
    public List<GameObject> npcs = new();
    public GameManager gameManager;
    public GameObject baseGrid;
    public GameObject baseCollisions;
    public GameObject groupGrid;
    public GameObject groupCollisions;
    private GameObject instance;

    [SerializeField]private PlayerController _playercontroller;
    [SerializeField] private List<GameObject> _toOtherFloors = new();
    [SerializeField] private GameObject battleTrigger;
    [SerializeField] private GameObject allClassView;
    [SerializeField] private GameObject ezequielView;
    [SerializeField] private Door toOtherFloorDoor;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject npcName;
    [SerializeField] private Door classroomDoor;
    [SerializeField] private Image illustration;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image npcImage;
    private List<Vector2> _npcsInitialPositions = new();
    private HashSet<int> _secondDayDialogueNarrator;
    private HashSet<int> _secondDayPlayer;
    private HashSet<int> _secondDayNPC;
    private float defaultTimeToReturn;
    private string _currentDialogueState;

    private void baseAwakeSingleton()
    {
        instance = GameObject.FindGameObjectWithTag("DaysManager");

        if(instance == null)
        {
            instance = this.gameObject;
        }else if (instance != this.gameObject)
        {
            Destroy(instance);
        }
    }

    void Awake()
    {
        baseAwakeSingleton();
        
        InitializeDialogueHashes();

        if (battleTrigger != null) battleTrigger.SetActive(false);
    }

    void Start()
    {

    }

    public void SetGameManager(GameManager gm)
    {
        gameManager = gm;
        defaultTimeToReturn = _secondsToReturn;
        _playercontroller = gm.GetPlayerController();
    }

    public IEnumerator FirstLines()
    {
        GameManager.Instance.SetDayConfigured(true);
        BasicPlayerCutsceneConfig();
        if (_toOtherFloors.Count > 0) foreach (GameObject toOtherFloor in _toOtherFloors) toOtherFloor.SetActive(true);

        _playercontroller.SetCanMove(false);
        _canSkip = true;
        _currentDialogueState = "SecondDayDialogue";
        yield return new WaitForSeconds(3f);

        dialogue.Clear();
        for (int iterator = 0; iterator < 4; iterator++) { dialogue.Add(secondDayDialogue[iterator]); }

        StartDialogue(0);

        while (!_isClosed) { yield return null; }
        _playercontroller.SetCanMove(true);

        gameManager.instruction.text = "Pressione TAB para abrir o inventÁrio e conferir seu objetivo.";
        gameManager.instruction.gameObject.SetActive(true);
        gameManager.currentObjective.text = "OBJETIVO: VÁ para sua sala.";
        yield return new WaitForSeconds(5f);
        gameManager.instruction.gameObject.SetActive(false);
    }

    public void SecondDayFloor2Config()
    {
        foreach (GameObject npc in npcs)
        {
            if (npc != null && npc.activeSelf) { if (npc.name.Equals("Estella") || npc.name.Equals("Ezequiel")) npc.SetActive(false); }
        }

        BasicPlayerCutsceneConfig();
        _playercontroller?.SetCanMove(true);
        _isAutomatic = false;
        _canSkip = true;

        gameManager.currentObjective.text = "Objetivo: VÁ para sua sala.";
    }

    public IEnumerator GroupClass()
    {
        BasicPlayerCutsceneConfig();
        GroupClassSettings();
        _currentDialogueState = "SecondDayDialogue";

        if (!npcs[0].gameObject.activeSelf) npcs[0].SetActive(true);
        for (int i = 0; i <= 7; i++) dialogue.Add(secondDayDialogue[i]);
        npcs[0].GetComponent<Animator>().Play("IdleD");

        yield return null;

        _canSkip = true;
        _isAutomatic = false;
        StartDialogue(4);

        while (!_isClosed) { yield return null; }
        dialogue.Clear();
        for (int i = 0; i <= 57; i++) dialogue.Add(secondDayDialogue[i]);

        _canSkip = false;
        _isAutomatic = true;
        StartCoroutine(StartAutomaticTalk(8));
        gameManager.AnimateTransition(1f, true);

        npcs[8].GetComponent<Animator>().Play("H_WalkingR");
        StartCoroutine(npcs[8].GetComponent<Ezequiel>().GoTo(3f, new Vector2(-1.96f, npcs[8].transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(3f);
        ezequielView.SetActive(true);
        npcs[8].GetComponent<Animator>().Play("H_WalkingD");
        StartCoroutine(npcs[8].GetComponent<Ezequiel>().GoTo(6f, new Vector2(npcs[8].transform.localPosition.x, -5.6f), 'y'));
        yield return new WaitForSeconds(6f);
        npcs[8].GetComponent<Animator>().Play("H_WalkingR");
        StartCoroutine(npcs[8].GetComponent<Ezequiel>().GoTo(3f, new Vector2(4.08f, npcs[8].transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(3f);
        npcs[8].GetComponent<Animator>().Play("H_IdleD");

        while (_i != 17) yield return null;

        npcs[8].GetComponent<Animator>().Play("H_WalkingR");
        StartCoroutine(npcs[8].GetComponent<Ezequiel>().GoTo(1f, new Vector2(6f, npcs[8].transform.localPosition.y), 'x'));
        yield return new WaitForSeconds(0.5f);
        npcs[8].GetComponent<SpriteRenderer>().sortingOrder = 1;
        yield return new WaitForSeconds(0.5f);
        npcs[8].GetComponent<Animator>().Play("H_IdleD");

        while (_i != 22) yield return null;
        StartCoroutine(npcs[8].GetComponent<Ezequiel>().GoTo(6f, new Vector2(npcs[8].transform.localPosition.x, -5.8f), 'y'));

        while (_i != 29) yield return null;
        gameManager.AnimateTransition(13f, false);
        StartCoroutine(CameraRoll(13f));

        while (_i != 33) yield return null;
        ezequielView.GetComponent<CinemachineCamera>().Lens.Dutch = 0f;
        gameManager.AnimateTransition(5f, true);
    }
    
    private IEnumerator CameraRoll(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            ezequielView.GetComponent<CinemachineCamera>().Lens.Dutch += Mathf.Clamp01((elapsedTime / time)/100);
            yield return null;
        }

        yield return null;
    }

    private void GroupClassSettings()
    {
        baseGrid.SetActive(false);
        groupGrid.SetActive(true);
        baseCollisions.SetActive(false);
        groupCollisions.SetActive(true);

        foreach (GameObject go in npcs)
        {
            CircleCollider2D trigger = go.GetComponent<CircleCollider2D>();

            if (trigger) trigger.enabled = false;

            _npcsInitialPositions.Add(go.transform.localPosition);
        }

        npcs[1].SetActive(false);
        npcs[2].SetActive(false);

        npcs[0].transform.localPosition = new Vector2(0, 3.8f);
        npcs[3].transform.localPosition = new Vector2(-6.02f, -1.58f);
        npcs[4].transform.localPosition = new Vector2(-3.98f, -1.73f);
        SetNpcParts(4);
        npcs[5].transform.localPosition = new Vector2(-4.02f, -0.032f);
        SetNpcParts(5);
        npcs[6].transform.localPosition = new Vector2(-6.01f, 0.01f);
        SetNpcParts(6);

        npcs[7].SetActive(true);
        npcs[8].SetActive(true);

        npcs[7].GetComponent<SpriteRenderer>().sortingOrder = 1;
        npcs[7].transform.localPosition = new Vector2(3.96f, -7.64f);
        npcs[7].GetComponent<Animator>().Play("Idle_U");
        npcs[8].GetComponent<SpriteRenderer>().sortingOrder = -1;
        //npcs[8].transform.localPosition = new Vector2(6, -5.62f);
        npcs[8].transform.localPosition = new Vector2(-6.18f, 2.77f);
        npcs[8].GetComponent<Animator>().Play("H_IdleR");

        _playercontroller.gameObject.transform.localPosition = new Vector2(6f, -7.58f);
        _playercontroller.gameObject.GetComponent<Animator>().Play("H_IdleUp");

        allClassView.SetActive(true);
    }
    
    private void SetNpcParts(int npcIndex)
    {
        npcs[npcIndex].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
        npcs[npcIndex].transform.Find("part2").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;

        if (npcIndex == 5 || npcIndex == 6) { npcs[npcIndex].transform.Find("part3").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1; }
        else { npcs[npcIndex].transform.Find("part3").gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2; }
        
        if(npcIndex == 5)
        {
            npcs[npcIndex].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[3];
            npcs[npcIndex].transform.Find("part2").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[4];
            npcs[npcIndex].transform.Find("part3").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[5];
            npcs[npcIndex].transform.Find("part1").gameObject.transform.localPosition = new Vector2(npcs[npcIndex].transform.Find("part1").gameObject.transform.localPosition.x, 0.716f);
        }else if(npcIndex == 6)
        {
            npcs[npcIndex].transform.Find("part1").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[0];
            npcs[npcIndex].transform.Find("part2").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[1];
            npcs[npcIndex].transform.Find("part3").gameObject.GetComponent<SpriteRenderer>().sprite = npcImages[2];
            npcs[npcIndex].transform.Find("part1").gameObject.transform.localPosition = new Vector2(npcs[npcIndex].transform.Find("part1").gameObject.transform.localPosition.x, 0.745f);
        }
    }

    protected void StartDialogue(int indexToStart)
    {
        if (!dialoguePanel.activeSelf)
        {
            dialogueText.text = "";
            _i = indexToStart;

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
        npcImage = GameObject.FindGameObjectWithTag("NPC_Image").GetComponent<Image>();
        playerName = GameObject.FindGameObjectWithTag("PlayerName");
        playerImage = GameObject.FindGameObjectWithTag("Player_Image").GetComponent<Image>();

        if (_currentDialogueState.Equals("SecondDayDialogue"))
        {
            SecondDayDialogueCheck(i);
        }
    }

    private void SecondDayDialogueCheck(int i)
    {
        if(_secondDayDialogueNarrator.Contains(i))
        {
            if(i == 0) playerName.GetComponent<TextMeshProUGUI>().text = "Alex";

            if (i < 4) { DialoguePanelSettings(1, 0.5f, 0, 0, TextAlignmentOptions.Center, FontStyles.Normal); }
            else if (i >= 4 && i < 8) { DialoguePanelSettings(0, 0, 0, 0, TextAlignmentOptions.Center, FontStyles.Normal); }
            else {
                DialoguePanelSettings(1, 0.5f, 1, 0.5f, TextAlignmentOptions.Center, FontStyles.Normal);
                if (i == 24) npcImage.sprite = npcImages[7];
                if (i == 29) playerImage.sprite = playerImages[1];
                if (i == 32) playerImage.sprite = playerImages[2];
            }

            if(i == 8){ DialoguePanelSettings(1, 0.5f, 0, 0, TextAlignmentOptions.Center, FontStyles.Normal); _secondsToReturn = 11f; wordSpeed = 0.06f; }
        }
        else if(_secondDayNPC.Contains(i))
        {
            DialoguePanelSettings(1f, 0.5f, 1, 1, TextAlignmentOptions.Left, FontStyles.Normal);
            
            if(i == 9 || i == 13 || i == 15 || i == 23 || i == 28 || i == 31 || i == 38 || i == 40){
                if(i == 9) _secondsToReturn = 2f;
                npcImage.sprite = npcImages[6];
                npcName.GetComponent<TextMeshProUGUI>().text = "Ezequiel";
            }else if(i == 12 || i == 14 || i == 16 || i == 26 || i == 39 || i == 62)
            {
                npcImage.sprite = npcImages[7];
                npcName.GetComponent<TextMeshProUGUI>().text = "Estella";
            }
        }
        else if(_secondDayPlayer.Contains(i))
        {
            DialoguePanelSettings(1, 1f, 0, 0, TextAlignmentOptions.Right, FontStyles.Normal);

            if(i == 36) playerImage.sprite = playerImages[0];
        }
    }

    public IEnumerator StartAutomaticTalk(int indexStart)
    {
        GameObject skipText = null;
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

        /*if ()
        {*/
            if (skipText != null) skipText.SetActive(true);
            if (!_canSkip) _canSkip = true;
        //}
        if(_i != 57) _playercontroller.SetCanMove(true);
    }

    private void BasicPlayerCutsceneConfig()
    {
        if (_playercontroller == null) _playercontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playercontroller?.SetCanMove(false);
        _isAutomatic = true;
        _canSkip = false;
    }

#region Initialize Dialogue Hashes
    private void InitializeDialogueHashes()
    {
        if (PlayerPrefs.GetString("currentState").Equals("StartDayTwo") || PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            InitializeSecondDayHash();
        }
    }

    private void InitializeSecondDayHash()
    {
        _secondDayDialogueNarrator = new HashSet<int>
        { 0, 3, 4, 5, 6, 7, 8, 10, 11, 17, 18, 19, 20, 22, 24, 27, 29, 30, 32, 33, 34, 37, 41, 42, 43, 44, 45, 47, 48, 49, 50, 54, 55, 56, 57, 58, 59, 60, 61, 63, 64, 65, 66, 67, 68, 69 };

        _secondDayPlayer = new HashSet<int>
        { 21, 35, 36, 76 };

        _secondDayNPC = new HashSet<int>
        { 9, 12, 13, 14, 15, 16, 23, 26, 28, 31, 38, 39, 40, 62 };
    }
#endregion
}
