using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Singleton;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerPFB;
    public GameObject animalPlayerPFB;
    public Image transitionImage;
    public List<GameObject> doors;
    public CinemachineCamera cinemachineCamera;
    public TextMeshProUGUI currentDay;
    public TextMeshProUGUI currentObjective;
    private PlayerController _playerController;
    private Ezequiel _ezequiel;

    protected override void Awake()
    {
        cinemachineCamera = GameObject.FindFirstObjectByType<CinemachineCamera>();
        PlayerManagement();
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name.Equals("PrototypeScene"))
        {
            PrototypeConfig();
        }
    }

    private void PrototypeConfig()
    {
        if(currentDay == null){currentDay = GameObject.FindGameObjectWithTag("CurrentDay").GetComponent<TextMeshProUGUI>();}
        if(currentObjective == null){currentObjective = GameObject.FindGameObjectWithTag("Objective").GetComponent<TextMeshProUGUI>();}
        transitionImage.color = new Vector4(transitionImage.color.r, transitionImage.color.g, transitionImage.color.b, 1f);
        currentObjective.color = new Vector4(currentObjective.color.r, currentObjective.color.g, currentObjective.color.b, 0f);
        AnimateTransition(3f, true);
        AnimateText(currentDay, 3f, true);
        AnimateText(currentObjective, 6f, false);
        StartCoroutine(SetPlayerCanMove());
    }

    IEnumerator SetPlayerCanMove()
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
        if(cinemachineCamera != null)
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

        cinemachineCamera = GameObject.FindFirstObjectByType<CinemachineCamera>();
        CinemachineFollow(player.GetComponent<Transform>());

        Door door = null;

        if (sceneName == "Classroom")
        {
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
    }

    void AnimateTransition(float time, bool toTransparent)
    {
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

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Clamp01(elapsedTime/time);
            textToFade.color = Color.Lerp(old, color, lerpAmount);

            yield return null;
        }
    }

    private IEnumerator FadeTransition(Color old, Color color, float time)
    {
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            float lerpAmount = Mathf.Clamp01(elapsedTime/time);
            transitionImage.color = Color.Lerp(old, color, lerpAmount);

            yield return null;
        }
    }

    public void CallEzequiel(string trigger)
    {
        if(trigger.Equals("PrototypeEzequielTrigger1"))
        {
            _ezequiel = GameObject.FindGameObjectWithTag("Ezequiel").GetComponent<Ezequiel>();
            _ezequiel.RecieveTrigger(_playerController.gameObject, "PrototypeEzequielTrigger1");
            Destroy(GameObject.FindGameObjectWithTag("PrototypeEzequielTrigger1"));
        }
    }
}
