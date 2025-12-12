using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;

public class PlayerDownedScript : MonoBehaviourPunCallbacks
{
    private UnitScript _unitScript;
    private PlayerMultiplayerIdScript _multiplayerIdScript;
    private PlayerMovement _playerMovement;

    [SerializeField] private bool _isDowned;
    [SerializeField] private float _percentSpeedReductionOnDown = 0.2f;
    [SerializeField] private float _bleedOutDuration = 30f;
    [SerializeField] private float _bleedOutTimer;
    public bool IsDowned => _isDowned;

    private void Awake()
    {
        _unitScript = GetComponent<UnitScript>();
        _multiplayerIdScript = GetComponent<PlayerMultiplayerIdScript>();
        _playerMovement = GetComponent<PlayerMovement>();

        _unitScript.PlayerDowned += OnPlayerDowned;
    }
    public override void OnDisable()
    {
        _unitScript.PlayerDowned -= OnPlayerDowned;
    }
    private void OnPlayerDowned(int playerID)
    {
        if (_multiplayerIdScript.id != playerID) return;

        _playerMovement.playerSpeed *= _percentSpeedReductionOnDown;
        _isDowned = true;
        StartCoroutine(PlayerDownedCoro());
    }
    [PunRPC]
    public void OnPlayerRevived(int id)
    {
        if (_multiplayerIdScript.id != id) return;

        StopAllCoroutines();
        _unitScript.OnRevive();
        _playerMovement.playerSpeed /= _percentSpeedReductionOnDown;
        _isDowned = false;
    }
    public void OnPlayerDeath()
    {
        if (photonView.IsMine)
            GameManager.instance.BackToMenu();
        Destroy(gameObject);
    }
    public IEnumerator PlayerDownedCoro()
    {
        for (_bleedOutTimer = _bleedOutDuration; _bleedOutTimer > 0f; _bleedOutTimer -= Time.deltaTime)
        {
            //update ui?
            yield return null;
        }
        OnPlayerDeath();
    }
}
