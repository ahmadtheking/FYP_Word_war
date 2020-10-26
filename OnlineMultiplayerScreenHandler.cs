using UnityEngine;
using UnityEngine.UI;

public class OnlineMultiplayerScreenHandler : MonoBehaviour
{
    [Header("Online Multiplayer Screen")]
    public GameObject createPrivateRoomPanel;
    public GameObject createPublicRoomPanel;
    public GameObject joinPrivateRoomPanel;
    public Transform publicRoomsContent;
    public GameObject roomsMessageText;

    [Header("Message")]
    public GameObject messagePanel;
    public Text messageText;

    private void OnEnable()
    {
        createPrivateRoomPanel.SetActive(false);
        createPublicRoomPanel.SetActive(false);
        joinPrivateRoomPanel.SetActive(false);
    }

    private void Update()
    {
        if (RoomListingManager.Instance.roomsList.Count == 0)
            roomsMessageText.SetActive(true);
        else
            roomsMessageText.SetActive(false);
    }

    public void OnMultiPlayerButtonClick()
    {
        gameObject.SetActive(true);
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    public void OnCreatePrivateRoomButtonClick()
    {
        createPrivateRoomPanel.SetActive(true);
    }

    public void OnJoinPrivateRoomButtonClick()
    {
        joinPrivateRoomPanel.SetActive(true);
    }

    public void OnCreatePublicRoomButtonClick()
    {
        createPublicRoomPanel.SetActive(true);
    }




    #region Message

    public void DisplayMessage(string message)
    {
        messagePanel.SetActive(true);
        messageText.text = message;
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 4f);
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }

    #endregion
}


