using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject player;
    public SpriteRenderer spriteRenderer;

    //Privates
    private Color _initialColor;
    private bool _playerIsClose;
    private bool _isClosed;

    void Start()
    {
        _playerIsClose = false;
        _isClosed = true;
        if(spriteRenderer == null) spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        _initialColor = this.gameObject.GetComponent<SpriteRenderer>().color;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && _playerIsClose)
        {
            if(_isClosed)
            {
                Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<BoxCollider2D>(), true);
            
                spriteRenderer.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, 0.5f);

                _isClosed = false;
            }else
            {
                Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), this.gameObject.GetComponent<BoxCollider2D>(), false);
            
                ResetColor();
                _isClosed = true;
            }
        }
    }

    private void ResetColor()
    {
        spriteRenderer.color = _initialColor;
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
