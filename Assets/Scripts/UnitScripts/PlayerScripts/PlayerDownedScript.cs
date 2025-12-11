using UnityEngine;
using System.Collections.Generic;

public class PlayerDownedScript : MonoBehaviour
{
    private UnitScript _unitScript;
    private PlayerMultiplayerIdScript _multiplayerIdScript;
    private PlayerMovement _playerMovement;

    [SerializeField] private bool _isDowned;
    [SerializeField] private float _percentSpeedReductionOnDown = 0.2f;
    public bool IsDowned => _isDowned;

    private void Awake()
    {
        _unitScript = GetComponent<UnitScript>();
        _multiplayerIdScript = GetComponent<PlayerMultiplayerIdScript>();
        _playerMovement = GetComponent<PlayerMovement>();

        _unitScript.PlayerDowned += OnPlayerDowned;
    }
    private void OnDisable()
    {
        _unitScript.PlayerDowned -= OnPlayerDowned;
    }
    private void OnPlayerDowned(int playerID)
    {
        if (_multiplayerIdScript.id != playerID) return;

        _playerMovement.playerSpeed *= _percentSpeedReductionOnDown;
    }
}
