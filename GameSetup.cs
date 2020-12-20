using Photon.Pun;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    private PhotonView PV;

    public GameObject Player1;
    public GameObject Player2;

    [Space(20)]
    public bool Slot1Avaialble = true;
    public bool Slot2Avaialble = true;

    [Space(20)]
    public string Player1Name;
    public string Player2Name;

    public static GameSetup _instance;
    public static GameSetup Instance
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
    }

    private void OnEnable()
    {
        PV = GetComponent<PhotonView>();
    }

    public void SetPlayerIcon(string nickName, int viewID)
    {
        PV.RPC(nameof(RPC_SetUpPlayerIcon), RpcTarget.AllBuffered, nickName, viewID, PreferenceManager.Username, PreferenceManager.Gender, PreferenceManager.Color, PreferenceManager.Cap, PreferenceManager.Glasses);
    }

    [PunRPC]
    private void RPC_SetUpPlayerIcon(string nickName, int viewID, string userName, string gender, int color, int cap, int glasses)
    {
        if (nickName == Nickname.PLAYER1.ToString())
        {
            Player1Name = userName;
            PhotonView tempPhotonView = PhotonView.Find(viewID);
            Player1 = tempPhotonView.gameObject;
            Slot1Avaialble = false;

            OnlineGamePlayManager.Instance.player1Icon.SetAvatar(userName, gender, color, cap, glasses);

            OnlineGamePlayManager.Instance.resultScreenPlayer1Icon.SetAvatar(userName, gender, color, cap, glasses);

            OnlineGamePlayManager.Instance.chatScreenHandler.player1DummyIcon.SetActive(false);
            OnlineGamePlayManager.Instance.chatScreenHandler.player1Icon.gameObject.SetActive(true);
            OnlineGamePlayManager.Instance.chatScreenHandler.player1Icon.SetAvatar(userName, gender, color, cap, glasses);
            
            if(!PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.IsMasterClient)
                OnlineGamePlayManager.Instance.chatScreenHandler.shareKeyButton.SetActive(true);
        }
        else if (nickName == Nickname.PLAYER2.ToString())
        {
            Player2Name = userName;
            PhotonView tempPhotonView = PhotonView.Find(viewID);
            Player2 = tempPhotonView.gameObject;
            Slot2Avaialble = false;

            OnlineGamePlayManager.Instance.player2Icon.SetAvatar(userName, gender, color, cap, glasses);

            OnlineGamePlayManager.Instance.resultScreenPlayer2Icon.SetAvatar(userName, gender, color, cap, glasses);

            OnlineGamePlayManager.Instance.chatScreenHandler.player2DummyIcon.SetActive(false);
            OnlineGamePlayManager.Instance.chatScreenHandler.player2Icon.gameObject.SetActive(true);
            OnlineGamePlayManager.Instance.chatScreenHandler.player2Icon.SetAvatar(userName, gender, color, cap, glasses);
        }
    }

}

public enum Nickname
{
    PLAYER1,
    PLAYER2,
}
