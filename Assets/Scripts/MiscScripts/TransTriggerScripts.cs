using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WFC;

public class TransTriggerScripts : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [SerializeField] private Transform target1, target2, 
                                        room1, room2;
    [SerializeField] private Camera _cam;
    [SerializeField] private float transLerpDur;

    private bool _playerInTransit = false;
    public bool PlayerInTransit { get { return _playerInTransit; } private set { _playerInTransit = value; } }

    private void Awake()
    {
        _cam = Camera.main;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Determine which target the player is further from, set appropriate targets,
        // freeze player, then start coroutines with appropriate arguments
        if (other.TryGetComponent<PlayerMovement>(out playerMovement))
        {
            _playerInTransit = true;

            float target1DistanceX = Mathf.Abs(target1.transform.position.x - other.transform.position.x);
            float target1DistanceY = Mathf.Abs(target1.transform.position.y - other.transform.position.y);

            float target2DistanceX = Mathf.Abs(target2.transform.position.x - other.transform.position.x);
            float target2DistanceY = Mathf.Abs(target2.transform.position.y - other.transform.position.y);

            bool nearTarget1 = (target1DistanceX + target1DistanceY) < (target2DistanceX + target2DistanceY);

            Transform roomTarget = nearTarget1 ? room2 : room1;
            Transform playerTarget = nearTarget1 ? target2 : target1;

            playerMovement._playerFrozen = true;
            StartCoroutine(RoomCamLerp(roomTarget));
            StartCoroutine(PlayerRoomTransCoro(playerTarget));
        }
    }
    public void SetRoomTargets(RoomElement newRoom1, RoomElement newRoom2)
    {
        room1.position = new Vector3(newRoom1.GetPosition.x, newRoom1.GetPosition.y, 0);
        room2.position = new Vector3(newRoom2.GetPosition.x, newRoom2.GetPosition.y, 0);
    }

    // Coroutine to lerp camera from initial position to target gameobject position over
    // transLerpDur seconds, then unfreeze player
    // @param roomTarget: target position for camera lerp
    private IEnumerator RoomCamLerp(Transform roomTarget)
    {
        Vector3 initialCamPos = _cam.transform.position;
        float timer = 0;

        while (timer < transLerpDur)
        {
            float lerpRatio = timer / transLerpDur;

            _cam.transform.position = Vector3.Lerp(initialCamPos, roomTarget.position, lerpRatio);

            timer += Time.deltaTime;
            yield return null;
        }
        playerMovement._playerFrozen = false;
    }

    // Coroutine to lerp player from initial position to target position over
    // transLerpDur seconds
    // @param target: target position for player lerp
    public IEnumerator PlayerRoomTransCoro(Transform target)
    {
        Vector3 initialPos = playerMovement.transform.position;
        float timer = 0;

        while (timer < transLerpDur)
        {
            float lerpRatio = timer / transLerpDur;

            // Player Lerp to target x and y positions, keeping z constant
            playerMovement.transform.position = Vector3.Lerp(initialPos, target.position, lerpRatio);

            timer += Time.deltaTime;
            yield return null;
        }
        _playerInTransit = false;
    }
}
