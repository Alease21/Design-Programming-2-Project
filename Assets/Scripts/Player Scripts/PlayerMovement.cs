using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Range(0, 20)]
    [SerializeField] private float _playerSpeed;
    private Vector2 _move;
    private Rigidbody _rb;

    //value read input
    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 currentVelo = _rb.linearVelocity;
        Vector3 targetVelo = new Vector3(_move.x, _move.y, 0f) * _playerSpeed;

        targetVelo = transform.TransformDirection(targetVelo);
        Vector3 veloChange = targetVelo - currentVelo;

        _rb.AddForce(new Vector3(veloChange.x, veloChange.y, 0), ForceMode.VelocityChange); //forcemode velocitychange is mass agnostic
                                                                                            //also can split this to ignore y if gravity is odd
    }
}

/* Other movement system I had before in class example
 * keeping here just in case 
 * 
private CharacterController _characterController;
private InputAction _moveAction;

[Range(0, 10)]
[SerializeField] private float _playerSpeed;
void Start()
{
    _characterController = GetComponent<CharacterController>();
    _moveAction = InputSystem.actions.FindAction("Move");
}

void Update()
{
    _characterController.Move(_moveAction.ReadValue<Vector2>().normalized * _playerSpeed * 0.01f); //float value is just from trial & error
}
*/
