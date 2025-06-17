using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerDrawManager : MonoBehaviour
{
    public List<GameObject> rows = new List<GameObject>();
    public GameObject player;
    public SpriteRenderer playerSR;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerSR = player.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        DrawManagement();
    }

    public void DrawManagement()
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
    }
}
