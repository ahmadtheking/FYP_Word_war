using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingManager : MonoBehaviourPunCallbacks
{
    private Transform content;
    public RoomElement roomElement;
    [HideInInspector] public List<RoomElement> roomsList = new List<RoomElement>();

    public static RoomListingManager _instance;
    public static RoomListingManager Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //content = UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().publicRoomsContent;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int indexNumber = roomsList.FindIndex(o => o.roomInfo.Name == info.Name);
                if (indexNumber != -1)
                {
                    Destroy(roomsList[indexNumber].gameObject);
                    roomsList.RemoveAt(indexNumber);
                }
            }
            else
            {
                int indexNumber = roomsList.FindIndex(o => o.roomInfo.Name == info.Name);
                if (indexNumber != -1)
                {
                    Destroy(roomsList[indexNumber].gameObject);
                    roomsList.RemoveAt(indexNumber);
                }

                RoomElement element = Instantiate(roomElement, content);
                if (info != null)
                {
                    element.SetUpRoomInfo(info);
                    roomsList.Add(element);
                }
            }
        }

        RectTransform rt = content.gameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, roomsList.Count * 90);
    }

    public void DestroyTableFromList(string tableKey)
    {
        int indexNumber = roomsList.FindIndex(o => o.roomInfo.Name == tableKey);
        if (indexNumber != -1)
        {
            Destroy(roomsList[indexNumber].gameObject);
            roomsList.RemoveAt(indexNumber);
        }
    }
}
