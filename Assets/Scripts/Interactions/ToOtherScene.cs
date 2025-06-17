using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ToOtherScene : MonoBehaviour
{
    public Door door;

    void Start()
    {
        door = GetComponentInChildren<Door>();
    }

    public void StartLoadNewScene(string sceneName, GameObject player)
    {
        StartCoroutine(LoadNewScene(sceneName, player));
    }

    public IEnumerator LoadNewScene(string sceneName, GameObject player)
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        yield return null;

        door = GameObject.FindFirstObjectByType<Door>();

        if (sceneName == "Classroom")
        {
            this.gameObject.transform.position = new Vector3(-10.53f, 7f, 1f);
            this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            player.transform.localScale = new Vector3(1.7f, 1.7f, 1f);
            player.GetComponent<PlayerController>().SetSpeed(8f);
            door.SetIsClosed(true);
        }
        else if (sceneName == "PrototypeScene")
        {
            this.gameObject.transform.position = new Vector3(0.05f, 3.93f, 0f);
            this.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            player.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            player.GetComponent<PlayerController>().SetSpeed(5f);
            door.SetIsClosed(true);
        }
        player.transform.position = new Vector3(door.transform.position.x, door.transform.position.y - 1f, door.transform.position.z);
        player.gameObject.GetComponent<PlayerController>().SetSpriteDown();

        door.ChangePlayer(player);
        door.ChangeSprite("open");

        yield return new WaitForSeconds(0.1f);

        door.ChangeSprite("close");

        door.IgnoreCollision(player, false);

        player.GetComponent<PlayerController>().SetCanMove(true);
    }
}
