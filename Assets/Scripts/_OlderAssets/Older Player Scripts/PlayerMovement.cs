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
    private Rigidbody2D _rb;
    private Camera _camera;
    public Player photonPlayer;
    public Animator spriteAnimator;
    public bool _playerFrozen = false;
    public Vector3 GetMouseDir => FindMouseDir();
    public Vector3 GetMousePosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

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
            _rb.bodyType = RigidbodyType2D.Kinematic;
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        _rb.linearVelocity = new Vector2(horizontal * _playerSpeed, vertical * _playerSpeed);
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

    public Vector3 FindMouseDir()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPoint.z = 0f;
        return (mouseWorldPoint - transform.position).normalized;
    }
}
