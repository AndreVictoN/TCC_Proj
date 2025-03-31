using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public InputActionReference move;
    private Vector2 _moveDirection;

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
        _moveDirection = move.action.ReadValue<Vector2>();
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
