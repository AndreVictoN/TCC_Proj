using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public GameObject player;
    public Sprite doorOpened;
    public Sprite doorClosed;
    public string identifier;

    //Privates
    private bool _playerIsClose;
    private bool _isClosed;

    void Start()
    {
        _playerIsClose = false;
        _isClosed = true;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _playerIsClose)
        {
            if (_isClosed)
            {
                ChangeSprite("open");
                IgnoreCollision(player, true);

                _isClosed = false;
            }
            else
            {
                ChangeSprite("close");
                IgnoreCollision(player, false);

                _isClosed = true;
            }
        }
    }

    public void IgnoreCollision(GameObject newPlayer, bool ignore)
    {
        Physics2D.IgnoreCollision(newPlayer.GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<BoxCollider2D>(), ignore);
    }

    public void SetIsClosed(bool isClosed)
    {
        _isClosed = isClosed;
    }
    
    public void ChangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }

    public void ChangeSprite(string status)
    {
        if (status == "open")
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = doorOpened;
        }
        else if (status == "close")
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = doorClosed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerIsClose = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerIsClose = false;
        }
    }
}
