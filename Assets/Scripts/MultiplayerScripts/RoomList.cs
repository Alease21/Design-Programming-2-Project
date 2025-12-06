using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _roomPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            GameObject room = Instantiate(_roomPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Content").transform);
            room.GetComponent<TextMeshProUGUI>().text = roomList[i].Name;
            room.GetComponent<Button>().clicked += () => MenuUI.instance.OnJoinRoomButton(roomList[i].Name);
        }
    }
}
