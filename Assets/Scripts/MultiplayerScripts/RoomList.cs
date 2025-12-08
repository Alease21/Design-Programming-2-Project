using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _roomPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            string roomName = roomList[i].Name;

            GameObject room = Instantiate(_roomPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform);
            room.GetComponentInChildren<TextMeshProUGUI>().text = roomName;
            room.GetComponentInChildren<Button>().onClick.AddListener(() => MenuUI.instance.OnJoinRoomButton(roomName));
        }
    }
}
