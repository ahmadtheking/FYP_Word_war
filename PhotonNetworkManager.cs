using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
    public string roomType = RoomType.None.ToString();

    public static PhotonNetworkManager _instance;
    public static PhotonNetworkManager Instance
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

    private void Update()
    {
        if (!GameManager.Instance.tryToConnentToServer)
        {
            PhotonNetwork.ConnectUsingSettings();
            GameManager.Instance.tryToConnentToServer = true;
            Invoke(nameof(CheckIsUserIsConnectedToMasterServer), 3f);
        }
    }

    private void CheckIsUserIsConnectedToMasterServer()
    {
        if (!PhotonNetwork.IsConnected)
            GameManager.Instance.isConnectedToServer = false;

        if (!GameManager.Instance.isConnectedToServer)
            GameManager.Instance.tryToConnentToServer = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player is connected to " + PhotonNetwork.CloudRegion + " server");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        GameManager.Instance.isConnectedToServer = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected! Photon Server");
        base.OnDisconnected(cause);
        GameManager.Instance.isConnectedToServer = false;
        GameManager.Instance.tryToConnentToServer = false;
        OnlineGamePlayManager.Instance.OnDisconnected();
    }






    #region Public Room

    //create a public room
    public void CreatePublicRoom(string tableKey, int roomSize)
    {
        PreferenceManager.RoomKey = tableKey;
        roomType = RoomType.Public.ToString();
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte) roomSize};
        PhotonNetwork.CreateRoom(tableKey, roomOptions);
    }

    //join a public room
    public void JoinPublicRoom(string roomKey)
    {
        roomType = RoomType.Public.ToString();
        PhotonNetwork.JoinRoom(roomKey);
    }

    //join a random room
    public void JoinRandomRoom()
    {
        roomType = RoomType.Public.ToString();
        PhotonNetwork.JoinRandomRoom();
    }

    //no random room is available
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random room but failed. There must be no open games available.");
    }

    #endregion





    #region Private Room

    //create a private room
    public void CreatePrivateRoom(string roomKey, int roomSize)
    {
        roomType = RoomType.Private.ToString();
        Debug.Log(roomKey);
        PreferenceManager.RoomKey = roomKey;
        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom(roomKey, roomOptions);
    }

    public void JoinPrivateRoom(string roomKey)
    {
        roomType = RoomType.Private.ToString();
        PhotonNetwork.JoinRoom(roomKey);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        if (roomType == RoomType.Private.ToString())
        {
            //UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().JoinPrivateRoomPanel.GetComponent<JoinPrivateRoomScreenHandler>().messageText.text = message.ToUpper();   
        }
        else if (roomType == RoomType.Public.ToString())
        {
            //UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().messageText.text = message.ToUpper();
        }
    }

    #endregion





    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        if (roomType == RoomType.Private.ToString())
        {
            //UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().CreateRoomPanel.GetComponent<CreateRoomScreenHandler>().messageText.text = "ERROR, TRY AGAIN";
        }
        else if (roomType == RoomType.Public.ToString())
        {
            //UIManager.Instance.UIScreensReferences[GameScreens.OnlineMultiplayerScreen].GetComponent<OnlineMultiplayerScreenHandler>().messageText.text = "ERROR, TRY AGAIN";
        }
    }


    //room is created and joined 
    public override void OnJoinedRoom()
    {
        Debug.Log("Room joined: Number of players in Room: " + PhotonNetwork.PlayerList.Length);
        GameManager.Instance.isJoinedRoom = true;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Room Left!");
        GameManager.Instance.isJoinedRoom = false;
        RoomListingManager.Instance.DestroyTableFromList(PreferenceManager.RoomKey);
    }
}

public enum RoomType
{
    None,
    Private,
    Public
}
