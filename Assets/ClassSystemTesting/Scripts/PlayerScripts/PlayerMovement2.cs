using MagicSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [Range(0, 20)]
    [SerializeField] private float _playerSpeed;

    //private Vector2 _move;
    private Rigidbody2D _rb;
    private Camera _camera;
    public bool _playerFrozen = false;
    public Vector3 GetMouseDir => FindMouseDir();

    public static PlayerMovement2 instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        _rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.back, FindMouseDir());

        OnMove();
        _camera.transform.position = new Vector3(transform.position.x, transform.position.y, _camera.transform.position.z);
    }
    private void OnMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _rb.linearVelocity = new Vector3(horizontal * _playerSpeed, vertical * _playerSpeed);
    }
    public Vector3 FindMouseDir()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPoint.z = 0f;
        return (mouseWorldPoint - transform.position).normalized;
    }
}
