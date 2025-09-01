using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerDrawManager : MonoBehaviour
{
    public List<GameObject> rows = new List<GameObject>();
    public GameObject player;
    public SpriteRenderer playerSR;

    private bool _playerIsSet = false;

    void Start()
    {
        _playerIsSet = false;
        SetPlayer();
    }

    private void SetPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            playerSR = player.GetComponent<SpriteRenderer>();
            _playerIsSet = true;
        }
    }

    void Update()
    {
        DrawManagement();

        if(_playerIsSet == false) {SetPlayer();}
    }

    public void DrawManagement()
    {
        if(SceneManager.GetActiveScene().name == "Classroom" && _playerIsSet)
        {
            if (player.transform.position.y > rows[0].transform.position.y)
            {
                playerSR.sortingOrder = 0;
            }
            else if (player.transform.position.y > rows[1].transform.position.y && player.transform.position.y < rows[0].transform.position.y)
            {
                playerSR.sortingOrder = 1;
            }
            else if (player.transform.position.y > rows[2].transform.position.y && player.transform.position.y < rows[1].transform.position.y)
            {
                playerSR.sortingOrder = 2;
            }
            else if (player.transform.position.y > rows[4].transform.position.y && player.transform.position.y < rows[2].transform.position.y)
            {
                playerSR.sortingOrder = 3;
            }
            else if (player.transform.position.y > rows[3].transform.position.y && player.transform.position.y < rows[4].transform.position.y)
            {
                playerSR.sortingOrder = 4;
            }
        }else if(SceneManager.GetActiveScene().name == "PrototypeScene")
        {
            if((player.transform.position.y >= 59.24f && player.transform.position.y <= 60.18f) || (player.transform.position.y >= 52.219f && player.transform.position.y <= 53.22f) || (player.transform.position.x >= 52.5f && player.transform.position.x <= 67.54f && ((player.transform.position.y >= 83.75f && player.transform.position.y <= 100.78f) || (player.transform.position.y >= 103.22f && player.transform.position.y <= 108.17f))))
            {
                playerSR.sortingOrder = -1;
            }else{
                playerSR.sortingOrder = 1;
            }
        }
    }
}
