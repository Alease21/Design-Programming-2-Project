using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Slider _playerHpBar; //send thru network
    [SerializeField] private Slider _playerDownedBar; //send thru network

    [SerializeField] private Sprite _playerAbility1; //send thru network?
    [SerializeField] private Sprite _playerAbility2; //send thru network?

    [SerializeField] private TextMeshProUGUI _playerLevel; //send thru network
    [SerializeField] private Slider _playerExpBar;

    //[SerializeField] private GameObject _playerAimIndicator;

    private void Awake()
    {
        
    }
}
