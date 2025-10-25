using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using WFC;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Range(0, 20)]
    [SerializeField] private float _playerSpeed;
    //private Vector2 _move;
    private Rigidbody _rb;
    private Camera _camera;
    public Player photonPlayer;
    public Animator spriteAnimator;
    public bool _playerFrozen = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        //_move = context.ReadValue<Vector2>();
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;

        if (id == 1) //can do random selection here if wanted
        {
            //GameManager.instance.GiveFlag(id, true);
        }

        if (!photonView.IsMine)
            _rb.isKinematic = true;
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main;
        spriteAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine && !_playerFrozen)
        {
            OnMove();
            _camera.transform.position = new Vector3(transform.position.x, transform.position.y, _camera.transform.position.z);
        }
    }
    private void OnMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        SetAnimFloats(horizontal, vertical);
        _rb.linearVelocity = new Vector3(horizontal * _playerSpeed, vertical * _playerSpeed, _rb.linearVelocity.z);
    }

    public void SetAnimFloats(float hori, float verti)
    {
        spriteAnimator.SetFloat("Horizontal", hori);
        spriteAnimator.SetFloat("Vertical", verti);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) { }
            //stream.SendNext(curFlagTime);
        else if (stream.IsReading) { }
            //curFlagTime = (float)stream.ReceiveNext();
    }
}
