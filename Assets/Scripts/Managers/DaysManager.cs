using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private GameObject instance;

    [SerializeField]private PlayerController _playercontroller;
    [SerializeField] private List<GameObject> _toOtherFloors = new();
    [SerializeField] private GameObject battleTrigger;
    [SerializeField] private Door toOtherFloorDoor;
    [SerializeField] private GameObject playerName;
    [SerializeField] private GameObject npcName;
    [SerializeField] private Door classroomDoor;
    [SerializeField] private Image illustration;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image npcImage;
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
        BasicPlayerCutsceneConfig();
        if (_toOtherFloors.Count > 0) foreach (GameObject toOtherFloor in _toOtherFloors) toOtherFloor.SetActive(true);

        _playercontroller.SetCanMove(false);
        _canSkip = true;
        _currentDialogueState = "SecondDayDialogue";
        yield return new WaitForSeconds(3f);

        dialogue.Clear();
        for (int iterator = 0; iterator < 3; iterator++) { dialogue.Add(secondDayDialogue[iterator]); }

        StartDialogue();

        while (!_isClosed) { yield return null; }
        _playercontroller.SetCanMove(true);

        gameManager.instruction.text = "Pressione TAB para abrir o inventÁrio e conferir seu objetivo.";
        gameManager.instruction.gameObject.SetActive(true);
        gameManager.currentObjective.text = "OBJETIVO: VÁ para sua sala.";
        yield return new WaitForSeconds(5f);
        gameManager.instruction.gameObject.SetActive(false);
    }

    public void SecondArrivalConfig()
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
        //BasicPlayerCutsceneConfig();

        if (!npcs[0].gameObject.activeSelf) npcs[0].SetActive(true);
        npcs[0].GetComponent<Animator>().Play("IdleD");
        yield return null;
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
        npcImage = GameObject.FindGameObjectWithTag("NPC_Image").GetComponent<Image>();
        playerName = GameObject.FindGameObjectWithTag("PlayerName");
        playerImage = GameObject.FindGameObjectWithTag("Player_Image").GetComponent<Image>();

        if (_currentDialogueState.Equals("SecondDayDialogue"))
        {
            switch(i)
            {
                case 0:
                    DialoguePanelSettings(1, 0.5f, 0, 0, TextAlignmentOptions.Center, FontStyles.Normal);
                    playerName.GetComponent<TextMeshProUGUI>().text = "Alex";
                    break;
                case 1: case 2:
                    DialoguePanelSettings(1, 1f, 0, 0, TextAlignmentOptions.Right, FontStyles.Normal);
                    break;
            }
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

    private void BasicPlayerCutsceneConfig()
    {
        if (_playercontroller == null) _playercontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _playercontroller?.SetCanMove(false);
        _isAutomatic = true;
        _canSkip = false;
    }
}
