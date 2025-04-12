using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public InputActionReference move;

    #region tags to compare
    [Header("ScenesToLoad")]
    public string prototypeScene = "PrototypeScene";
    public string firstFloor = "FirstFloorPrototype";

    private string npcTag = "NPC";
    private string doorTag = "Door";
    private string stairsTag = "Stairs";
    #endregion

    //Privates
    private Vector2 _moveDirection;
    private Vector3 _positionBeforeFloor;
    private bool _canMove = true;

    void Awake()
    {
        this.gameObject.transform.position = new Vector3(0, 0, 0);
        
        if(rb == null && this.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            rb = this.gameObject.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if(_canMove) _moveDirection = move.action.ReadValue<Vector2>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(npcTag) || collision.gameObject.CompareTag(doorTag))
        {
            rb.linearVelocity = Vector2.zero;
        }else if(collision.gameObject.CompareTag(stairsTag))
        {
            if(SceneManager.GetActiveScene().name == prototypeScene) StartCoroutine(LoadScene(firstFloor));
            else if(SceneManager.GetActiveScene().name == firstFloor) StartCoroutine(LoadScene(prototypeScene));

            _canMove = false;
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        transform.DOMoveY(-2, 1f).SetRelative();

        yield return new WaitForSeconds(1f);

        _positionBeforeFloor = this.gameObject.transform.position;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;

        transform.DOMoveY(2.5f, 1f).SetRelative();

        yield return new WaitForSeconds(1f);

        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        _canMove = true;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void MovePlayer()
    {
        rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);
    }
}
