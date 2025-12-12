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
        var content = GameObject.Find("Content").transform;

        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;

            string roomName = roomList[i].Name;

            GameObject room = Instantiate(_roomPrefab, Vector3.zero, Quaternion.identity, content);
            room.GetComponentInChildren<TextMeshProUGUI>().text = roomName;
            var button = room.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => MenuUI.instance.OnJoinRoomButton(roomName));
            button.interactable = roomList[i].IsOpen;
        }
    }
}
