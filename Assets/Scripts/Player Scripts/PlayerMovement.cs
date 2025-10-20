using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    public int id;

    [Range(0, 20)]
    [SerializeField] private float _playerSpeed;
    //private Vector2 _move;
    private Rigidbody _rb;
    public Player photonPlayer;
    public Animator spriteAnimator;

    public void OnMove(InputAction.CallbackContext context)
    {
        //_move = context.ReadValue<Vector2>();
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        spriteAnimator = GetComponent<Animator>();
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

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            /*
            Vector3 currentVelo = _rb.linearVelocity;
            Vector3 targetVelo = new Vector3(_move.x, _move.y, 0f) * _playerSpeed;

            targetVelo = transform.TransformDirection(targetVelo);
            Vector3 veloChange = targetVelo - currentVelo;

            _rb.AddForce(new Vector3(veloChange.x, veloChange.y, 0), ForceMode.VelocityChange); //forcemode velocitychange is mass agnostic
                                                                                                //also can split this to ignore y if gravity is odd
            */
            OnMove();
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
