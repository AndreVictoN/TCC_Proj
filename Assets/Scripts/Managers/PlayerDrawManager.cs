using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerDrawManager : MonoBehaviour
{
    //[SerializeField] private List<GameObject> rows = new List<GameObject>();
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
            if(playerSR != null) _playerIsSet = true;
        }
    }

    void Update()
    {
        DrawManagement();

        if(_playerIsSet == false) {SetPlayer();}
    }

    public void DrawManagement()
    {
        if (!_playerIsSet) return;
        if (SceneManager.GetActiveScene().name == "Class" && _playerIsSet)
        {
            ClassDraw();
        }
        else if (SceneManager.GetActiveScene().name == "Terreo")
        {
            TerreoDraw();
        }
        else if (SceneManager.GetActiveScene().name == "PrototypeScene")
        {
            PrototypeDraw();
        }
    }

    private void ClassDraw()
    {
        if (player.transform.localPosition.y > 5.08f)
        {
            playerSR.sortingOrder = 0;
        }
        else if (player.transform.localPosition.y > 1.35f && player.transform.localPosition.y < 5.08f)
        {
            playerSR.sortingOrder = 1;
        }
        else if (player.transform.localPosition.y > -1.56f && player.transform.localPosition.y < 1.35f)
        {
            playerSR.sortingOrder = 2;
        }
        else if (player.transform.localPosition.y > -4.58f && player.transform.localPosition.y < -1.56f)
        {
            playerSR.sortingOrder = 3;
        }
        else if (player.transform.localPosition.y > -7.6f && player.transform.localPosition.y < -4.58f)
        {
            playerSR.sortingOrder = 4;
        }
        else if (player.transform.localPosition.y < -7.6f) { playerSR.sortingOrder = 5; }
    }

    private void TerreoDraw()
    {
        if (player.transform.position.x >= 15.37f && player.transform.position.x <= 23.49f)
        {
            if (player.transform.position.y >= 81.22f && player.transform.position.y <= 82.07f)
            {
                playerSR.sortingOrder = -1;
            }
        }
        else if (player.transform.position.x >= 27.5f && player.transform.position.x <= 35.28f)
        {
            if (player.transform.position.y >= 83.3f && player.transform.position.y <= 84.1f)
            {
                playerSR.sortingOrder = -1;
            }
        }
        else if (player.transform.position.x >= 1 && player.transform.position.x <= 8.9f && player.transform.position.y >= 77.42691f && player.transform.position.y <= 86.1f)
        {
            if ((player.transform.position.y >= 85.3f && player.transform.position.y <= 86.1f) || (player.transform.position.y >= 80.36f && player.transform.position.y <= 81.17f))
            {
                playerSR.sortingOrder = -1;
            }else { playerSR.sortingOrder = 1; }
        }else if(player.transform.position.x >= -4 && player.transform.position.x <= 4.9)
        {
            if(player.transform.position.y >= 76.60f && player.transform.position.y <= 77.62f)
            {
                playerSR.sortingOrder = -1;
            }
        }
        else { playerSR.sortingOrder = 1; }
    }
    
    private void PrototypeDraw()
    {
        if (player.transform.position.x >= 0.68f && player.transform.position.x <= 9.11f)
        {
            if ((player.transform.position.y >= 80f && player.transform.position.y <= 82f) || (player.transform.position.y <= 86.25f && player.transform.position.y >= 85.25f))
            {
                playerSR.sortingOrder = -1;
            }
        }
        else if (player.transform.position.x >= 15f && player.transform.position.x <= 23f)
        {
            if (player.transform.position.y >= 81.3f && player.transform.position.y <= 82.3f)
            {
                playerSR.sortingOrder = -1;
            }
        }
        else if (player.transform.position.x >= 27.5f && player.transform.position.x <= 35.28f)
        {
            if (player.transform.position.y >= 83.3f && player.transform.position.y <= 84.3f)
            {
                playerSR.sortingOrder = -1;
            }
        }
        else { playerSR.sortingOrder = 1; }
    }
}
