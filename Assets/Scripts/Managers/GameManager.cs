using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Singleton;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;

public class GameManager : Singleton<GameManager>, IObserver
{
    public GameObject playerPFB;
    public GameObject animalPlayerPFB;
    public Image transitionImage;
    public List<GameObject> doors;
    public CinemachineCamera cinemachineCamera;
    public TextMeshProUGUI currentDay;
    public TextMeshProUGUI currentObjective;
    public TextMeshProUGUI instruction;
    public GameObject exitGame;
    public GameObject inventory;

    [Header("Arrival")]
    public ArrivalManager arrivalManager;

    [Header("Prototype")]
    [SerializeField] private GameObject _prototypeTeacher;
    [SerializeField] private GameObject _prototypeGirl;
    [SerializeField] private GameObject _girlTrigger;
    [SerializeField] private GameObject _battleTrigger;
    [SerializeField] private GameObject _ezequielTrigger;
    [SerializeField] private GameObject _firstInteractionTrigger;
    [SerializeField] private GameObject _stopTrigger;
    public string stopDialogue;

    [Header("Texts")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public List<string> dialogue = new List<string>();
    public float wordSpeed = 0.6f;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Ezequiel _ezequiel;
    private string _classroomScene = "Classroom";
    private bool _isTyping;
    private bool _skipped;
    private bool _canSkip;
    private int _i;

    protected override void Awake()
    {
        //PlayerPrefs.SetString("pastScene", "Menu");
        //PlayerPrefs.SetString("currentState", "Start");
        cinemachineCamera = GameObject.FindFirstObjectByType<CinemachineCamera>();
        PlayerManagement();
        _canSkip = false;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("PrototypeScene"))
        {
            PrototypeConfig();
        }
        else if (SceneManager.GetActiveScene().name.Equals("Terreo"))
        {
            if (PlayerPrefs.GetString("currentState").Equals("Start")) ArrivalConfig();
        }
        else if (SceneManager.GetActiveScene().name.Equals("Class"))
        {
            if (PlayerPrefs.GetString("currentState").Equals("Start")) FirstClassConfig();
        }
    }

    private void ArrivalConfig()
    {
        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();
        if (currentDay == null) { currentDay = GameObject.FindGameObjectWithTag("CurrentDay").GetComponent<TextMeshProUGUI>(); }
        transitionImage.color = new Vector4(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);
        AnimateTransition(3f, true);
        AnimateText(currentDay, 3f, true);
        arrivalManager.SetGameManager(this);
        if (currentObjective == null) { currentObjective = GameObject.FindGameObjectWithTag("Objective").GetComponent<TextMeshProUGUI>(); }
        if (instruction == null) instruction = GameObject.FindGameObjectWithTag("Instruction").GetComponent<TextMeshProUGUI>();
        StartCoroutine(arrivalManager.FirstLines());
    }

    private void FirstClassConfig()
    {
        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();
        transitionImage.color = new Vector4(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);
        AnimateTransition(3f, true);
        arrivalManager.SetGameManager(this);
        StartCoroutine(arrivalManager.FirstClass());
    }

    private void PrototypeConfig()
    {
        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();
        if (currentDay == null) { currentDay = GameObject.FindGameObjectWithTag("CurrentDay").GetComponent<TextMeshProUGUI>(); }
        if (currentObjective == null) { currentObjective = GameObject.FindGameObjectWithTag("Objective").GetComponent<TextMeshProUGUI>(); }
        transitionImage.color = new Vector4(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);

        if (PlayerPrefs.GetString("pastScene") == "BattleScene")
        {
            PostBattleConfig();
        }
        else
        {
            BasicConfig();
        }

        AnimateTransition(3f, true);
        AnimateText(currentObjective, 6f, false);
    }

    private void PostBattleConfig()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        currentObjective = GameObject.FindGameObjectWithTag("Objective").GetComponent<TextMeshProUGUI>();
        if (exitGame.activeSelf == false) exitGame.SetActive(true);
        if (_girlTrigger.activeSelf == true) _girlTrigger.SetActive(false);
        if (_prototypeGirl.activeSelf == true) _prototypeGirl.SetActive(false);
        if (_battleTrigger.activeSelf == true) _battleTrigger.SetActive(false);
        if (_stopTrigger.activeSelf == true) _stopTrigger.SetActive(false);
        if (_ezequielTrigger.activeSelf == false) _ezequielTrigger.SetActive(true);
        if (_firstInteractionTrigger.activeSelf == true) _firstInteractionTrigger.SetActive(false);
        if (_ezequiel.gameObject.activeSelf == false) _ezequiel.gameObject.SetActive(true);
        if (currentDay.gameObject.activeSelf == true) currentDay.gameObject.SetActive(false);
        _ezequiel.gameObject.transform.localPosition = new Vector2(9.44f, 102.03f);
        _playerController.gameObject.transform.localPosition = new Vector2(11.52f, 102.03f);
        _playerController.SetAnimation("H_IdleL", 0);
        _ezequiel.SetAnimation("H_IdleR");
    }

    private void BasicConfig()
    {
        if (exitGame.activeSelf == true) exitGame.SetActive(false);
        if (_girlTrigger.activeSelf == true) _girlTrigger.SetActive(false);
        if (_prototypeGirl.activeSelf == true) _prototypeGirl.SetActive(false);
        if (_battleTrigger.activeSelf == true) _battleTrigger.SetActive(false);
        if (_ezequielTrigger.activeSelf == true) _ezequielTrigger.SetActive(false);
        if (_ezequiel.gameObject.activeSelf == true) _ezequiel.gameObject.SetActive(false);
        AnimateText(currentDay, 3f, true);
        if (PlayerPrefs.GetString("pastScene") != "Cutscene") StartCoroutine(SetPlayerCanMove());
    }

    public IEnumerator SetPlayerCanMove()
    {
        _playerController.SetCanMove(false);
        yield return new WaitForSeconds(3f);
        _playerController.SetCanMove(true);
    }

    public void PlayerManagement()
    {
        if (GameObject.FindFirstObjectByType<PlayerController>() == null)
        {
            CinemachineFollow(GameObject.Instantiate(playerPFB).GetComponent<Transform>());
            _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
    }

    void CinemachineFollow(Transform transform)
    {
        if (cinemachineCamera != null)
        {
            cinemachineCamera.Follow = transform;
        }
    }

    public void StartLoadNewScene(string sceneName, GameObject player, GameObject toOtherScene)
    {
        StartCoroutine(LoadNewScene(sceneName, player, toOtherScene));
    }

    public IEnumerator LoadNewScene(string sceneName, GameObject player, GameObject toOtherScene)
    {
        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();
        AnimateTransition(0.5f, false);
        yield return new WaitForSeconds(0.5f);

        string identifier = toOtherScene.GetComponentInChildren<Door>().identifier;

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        yield return null;

        if (sceneName != _classroomScene && sceneName != "TestScene")
        {
            cinemachineCamera = GameObject.FindFirstObjectByType<CinemachineCamera>();
            CinemachineFollow(player.GetComponent<Transform>());
        }

        Door door = null;

        if (sceneName == _classroomScene)
        {
            player = GameObject.Instantiate(playerPFB);
            door = GameObject.FindFirstObjectByType<Door>();
            player.transform.localScale = new Vector3(1.7f, 1.7f, 1f);
            player.GetComponent<PlayerController>().SetSpeed(8f);
            door.SetIsClosed(true);
        }
        else if (sceneName == "TestScene")
        {
            doors.Clear();
            doors.AddRange(GameObject.FindGameObjectsWithTag("Door"));
            foreach (GameObject d in doors)
            {
                if (d.GetComponent<Door>().identifier == identifier)
                {
                    door = d.GetComponent<Door>();
                    door.SetIsClosed(true);
                    break;
                }
            }
            player = GameObject.FindGameObjectWithTag("Player");
            player.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            player.GetComponent<PlayerController>().SetSpeed(5f);
        }

        if (door == null)
        {
            Debug.LogError("Door not found in the new scene. " + identifier);
            yield break;
        }
        player.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - 1f, door.transform.position.z);
        player.gameObject.GetComponent<PlayerController>().SetSpriteDown();

        door.ChangePlayer(player);
        door.ChangeSprite("open");

        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();
        transitionImage.color = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);

        yield return new WaitForSeconds(0.1f);

        AnimateTransition(0.5f, true);
        yield return new WaitForSeconds(0.5f);

        door.ChangeSprite("close");

        door.IgnoreCollision(player, false);

        player.GetComponent<PlayerController>().SetCanMove(true);
        doors.Clear();

        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void AnimateTransition(float time, bool toTransparent)
    {
        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();

        if (!toTransparent)
        {
            Color newColor = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);
            StartCoroutine(FadeTransition(transitionImage.color, newColor, time));
        }
        else
        {
            Color newColor = new Color(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 0f);
            StartCoroutine(FadeTransition(transitionImage.color, newColor, time));
        }
    }

    void AnimateText(TextMeshProUGUI textToFade, float time, bool toTransparent)
    {
        if (!toTransparent)
        {
            Color newColor = new Color(1f, 1f, 1f, 1f);
            StartCoroutine(FadeText(textToFade, textToFade.color, newColor, time));
        }
        else
        {
            Color newColor = new Color(1f, 1f, 1f, 0f);
            StartCoroutine(FadeText(textToFade, textToFade.color, newColor, time));
        }
    }

    private IEnumerator FadeText(TextMeshProUGUI textToFade, Color old, Color color, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Clamp01(elapsedTime / time);
            textToFade.color = Color.Lerp(old, color, lerpAmount);

            yield return null;
        }
    }

    private IEnumerator FadeTransition(Color old, Color color, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Clamp01(elapsedTime / time);
            transitionImage.color = Color.Lerp(old, color, lerpAmount);

            yield return null;
        }
    }

    public void CallEzequiel(string trigger)
    {
        if (trigger.Equals("PrototypeEzequielTrigger1"))
        {
            _ezequiel = GameObject.FindGameObjectWithTag("Ezequiel").GetComponent<Ezequiel>();
            if (_playerController == null) _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            _playerController.SetCanMove(false);
            _ezequiel.RecieveTrigger(_playerController.gameObject, "PrototypeEzequielTrigger1");
            Destroy(GameObject.FindGameObjectWithTag("PrototypeEzequielTrigger1"));
        }
    }

    public void CallGirl(string trigger)
    {
        if (trigger.Equals("GirlTrigger"))
        {
            StartCoroutine(GirlActions());
            Destroy(GameObject.FindGameObjectWithTag("GirlTrigger"));
        }
    }

    IEnumerator GirlActions()
    {
        if (_playerController.transform.localPosition.y < _prototypeGirl.transform.localPosition.y) { _playerController.SetAnimation("H_WalkingUp", 0); }
        else if (_playerController.transform.localPosition.y > _prototypeGirl.transform.localPosition.y) { _playerController.SetAnimation("H_WalkingDown", 0); }

        float movementTime = _playerController.ToY(_prototypeGirl.transform.localPosition.y);
        yield return new WaitForSeconds(movementTime);
        _playerController.SetAnimation("H_IdleL", 0);

        _prototypeGirl.GetComponent<Girl>().RecieveTrigger(_playerController.gameObject, "GirlTrigger");

        while (!_prototypeGirl.GetComponent<Girl>().GetIsClosed()) { yield return null; }

        _playerController.SetAnimation("H_WalkingLeft", 2);
        StartCoroutine(_playerController.GoTo(1f, new Vector2(9.58f, _playerController.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(1);

        _playerController.SetAnimation("H_WalkingUp", 3);
        StartCoroutine(_playerController.GoTo(4f, new Vector2(_playerController.transform.localPosition.x, 102.03f), 'y', false));
        yield return new WaitForSeconds(4f);

        _playerController.SetAnimation("H_WalkingLeft", 0);
        StartCoroutine(_playerController.GoTo(3f, new Vector2(11.52f, _playerController.transform.localPosition.y), 'x', false));
        yield return new WaitForSeconds(3f);
        _playerController.SetAnimation("TransformL", 0);
        _battleTrigger.SetActive(true);
    }

    public void OnNotify(EventsEnum evt)
    {
        if (evt == EventsEnum.CallPrototypeEzequiel)
        {
            CallEzequiel("PrototypeEzequielTrigger1");
        }
        else if (evt == EventsEnum.PrototypeBattle)
        {
            StartCoroutine(LoadBattleScene("PrototypeScene"));
            _battleTrigger.SetActive(false);
        }
        else if (evt == EventsEnum.PrototypeFirstInteraction)
        {
            ManageTeacherPrototypeInteraction(evt);
        }
        else if (evt == EventsEnum.StopInteraction)
        {
            StopPlayer(evt);
        }
        else if (evt == EventsEnum.PrototypeGirl)
        {
            CallGirl("GirlTrigger");
        }
        else if (evt == EventsEnum.IntoSecretary)
        {
            StartCoroutine(InAndOutSecretary());
        }
        else if (evt == EventsEnum.ToOutside)
        {
            StartCoroutine(OutSchool());
        }
        else if (evt == EventsEnum.ExitGame)
        {
            StartCoroutine(LeaveSchool());
        }
        else if (evt == EventsEnum.EnterSchool)
        {
            StartCoroutine(InSchool());
        }
        else if (evt == EventsEnum.Inventory)
        {
            Inventory();
        }
        else if (evt == EventsEnum.FirstInteraction)
        {
            if (!arrivalManager.gameObject.activeSelf) arrivalManager.gameObject.SetActive(true);
            StartCoroutine(arrivalManager.FirstInteractionScene());
            Destroy(GameObject.FindGameObjectWithTag("FirstInteractionTrigger"));
            Destroy(GameObject.FindGameObjectWithTag("StopTrigger"));
        }
    }

    private void Inventory()
    {
        if (inventory == null) inventory = GameObject.FindGameObjectWithTag("Inventory");

        if (inventory != null)
        {
            GameObject inventoryHUD = inventory.transform.Find("Inventory").gameObject;

            if (inventoryHUD != null && inventoryHUD.activeSelf == false)
            {
                if(instruction.IsActive()){ instruction.gameObject.SetActive(false); }
                inventoryHUD.SetActive(true); _playerController.InventorySet(true);
            }
            else if (inventoryHUD != null && inventoryHUD.activeSelf == true) { inventoryHUD.SetActive(false); _playerController.InventorySet(false); }
        }
    }

    private IEnumerator InSchool()
    {
        AnimateTransition(1f, false);
        yield return new WaitForSeconds(1);

        _playerController.SetCanMove(true);
        _playerController.gameObject.transform.localPosition = new Vector2(8.8f, 24f);

        _playerController.SetAnimation("H_IdleUp", 0);

        yield return new WaitForSeconds(1f);

        AnimateTransition(1f, true);

        _playerController.SetAnimation("Moving", 0);
    }

    private IEnumerator LeaveSchool()
    {
        AnimateTransition(1f, false);
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("EndScene");
    }

    private IEnumerator OutSchool()
    {
        AnimateTransition(1f, false);
        yield return new WaitForSeconds(1);

        _playerController.SetCanMove(true);
        _playerController.gameObject.transform.localPosition = new Vector2(8.8f, 7.1f);
        currentObjective.text = "";

        _playerController.SetAnimation("H_Idle", 0);

        yield return new WaitForSeconds(1f);

        AnimateTransition(1f, true);

        _playerController.SetAnimation("Moving", 0);
    }

    private IEnumerator InAndOutSecretary()
    {
        AnimateTransition(1f, false);
        yield return new WaitForSeconds(1);

        _ezequiel.gameObject.SetActive(false);
        _playerController.SetCanMove(true);
        _playerController.gameObject.transform.localPosition = new Vector2(18f, 110.53f);
        currentObjective = GameObject.FindGameObjectWithTag("Objective").GetComponent<TextMeshProUGUI>();
        currentObjective.text = "Objetivo: Saia da Escola";

        GameObject.FindGameObjectWithTag("ToSecretary").GetComponentInChildren<Door>().ChangeSprite("close");
        _playerController.SetAnimation("H_Idle", 0);

        yield return new WaitForSeconds(1f);

        AnimateTransition(1f, true);


        _playerController.SetAnimation("Moving", 0);

        if (!exitGame.activeSelf) exitGame.SetActive(true);
    }

    private void StopPlayer(EventsEnum evt)
    {
        dialogue.Clear();
        if (stopDialogue != null) dialogue.Add(stopDialogue);
        StartCoroutine(StartAutomaticTalk(evt));
    }

    private void ManageTeacherPrototypeInteraction(EventsEnum evt)
    {
        dialogue.Clear();

        if (dialogue.Count < 2)
        {
            dialogue.Add("Que lugar enorme...");
            dialogue.Add("vou ter que pedir informaÇÃo para alguÉm se eu quiser encontrar a secretaria... de preferÊncia um professor.");
        }

        Destroy(GameObject.FindGameObjectWithTag("PrototypeFirstInteractionTrigger"));
        StartCoroutine(StartAutomaticTalk(evt));
    }

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

        if (evt == EventsEnum.PrototypeFirstInteraction) { StartCoroutine(TeacherTalk(skipText)); }
        else
        {
            while (!_skipped) yield return null;
            dialogueText.fontStyle = FontStyles.Normal;
            _playerController.SetCanMove(true);
        }
    }

    private IEnumerator TeacherTalk(GameObject skipText)
    {
        while (!_skipped) yield return null;
        dialogueText.fontStyle = FontStyles.Normal;

        if (_playerController.gameObject.transform.localPosition.x > 28.86f) _playerController.SetAnimation("H_WalkingLeft", 0);
        else if (_playerController.gameObject.transform.localPosition.x < 28.86f) _playerController.SetAnimation("H_WalkingRight", 0);

        float seconds = _playerController.ToX(28.86f);
        yield return new WaitForSeconds(seconds);

        if (_playerController.gameObject.transform.localPosition.y > 78.38f) _playerController.SetAnimation("H_WalkingDown", 0);
        else if (_playerController.gameObject.transform.localPosition.y < 78.38f) _playerController.SetAnimation("H_WalkingUp", 0);

        seconds = _playerController.ToY(78.38f);
        yield return new WaitForSeconds(seconds);

        _playerController.SetAnimation("H_IdleL", 0);

        _prototypeTeacher.SetActive(true);
        _prototypeTeacher.GetComponent<Teacher>().Move(4f, new Vector2(_prototypeTeacher.transform.localPosition.x, 78.38f), 'y');
        _prototypeTeacher.GetComponent<Teacher>().SetAnimation("WalkingDown");
        yield return new WaitForSeconds(1f);
        _prototypeTeacher.GetComponent<Teacher>().SetCanSkip(false);
        StartCoroutine(_prototypeTeacher.GetComponent<Teacher>().StartAutomaticTalk());
        yield return new WaitForSeconds(4f);
        _prototypeTeacher.GetComponent<Teacher>().Move(1f, new Vector2(_prototypeTeacher.transform.localPosition.x, 74.04f), 'y');
        yield return new WaitForSeconds(1f);
        _prototypeTeacher.GetComponent<Teacher>().Move(1f, new Vector2(24.17f, _prototypeTeacher.transform.localPosition.y), 'x');
        _prototypeTeacher.GetComponent<Teacher>().SetAnimation("WalkingLeft");
        yield return new WaitForSeconds(1f);
        _prototypeTeacher.GetComponent<Teacher>().Move(1f, new Vector2(_prototypeTeacher.transform.localPosition.x, 63.12f), 'y');
        _prototypeTeacher.GetComponent<Teacher>().SetAnimation("WalkingDown");
        if (skipText.activeSelf == false) skipText.SetActive(true);
        _prototypeTeacher.GetComponent<Teacher>().SetCanSkip(true);
        yield return new WaitForSeconds(4f);

        while (!_prototypeTeacher.GetComponent<Teacher>().GetIsClosed()) yield return null;

        _prototypeTeacher.GetComponent<Teacher>().SetIsAutomatic(false);
        _playerController.SetCanMove(true);
        _playerController.SetAnimation("Moving", 0);

        Destroy(_prototypeTeacher);

        _prototypeGirl.SetActive(true);
        _girlTrigger.SetActive(true);
    }

    public virtual void NextLine()
    {

        if (_i < dialogue.Count - 1)
        {
            _i++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            _i = 0;
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (arrivalManager.gameObject == null || !arrivalManager.gameObject.activeSelf)
        {
            if (_canSkip && Input.GetKeyDown(KeyCode.Return) && _isTyping)
            {
                StopCoroutine(Typing());
                dialogueText.text = dialogue[_i];
            }
            else if (_canSkip && Input.GetKeyDown(KeyCode.Return) && !_isTyping)
            {
                NextLine();
                _canSkip = false;
                _skipped = true;
                //_playerController.SetCanMove(true);
            }
        }
    }

    public IEnumerator LoadBattleScene(string pastScene)
    {
        transitionImage = GameObject.FindGameObjectWithTag("TransitionImage").GetComponent<Image>();
        _playerController.SetCanMove(false);
        AnimateTransition(1f, false);
        yield return new WaitForSeconds(1f);

        PlayerPrefs.SetString("pastScene", pastScene);
        Destroy(_stopTrigger);
        Destroy(_battleTrigger);
        exitGame.SetActive(true);
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Single);
    }
    
    public PlayerController GetPlayerController(){ return _playerController; }
}
