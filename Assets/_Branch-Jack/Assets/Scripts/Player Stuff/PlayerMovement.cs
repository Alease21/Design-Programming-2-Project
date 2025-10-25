using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementC : MonoBehaviour
{
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
}
