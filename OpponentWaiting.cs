using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class OpponentWaiting : MonoBehaviour
{
    
    private byte dots = 1;
    public Text waitingText;

    private string username;
    private string op_username = null;
    private string op_imagID = null;
    private string boardData = null;

    private void Start()
    {
        InvokeRepeating("UpdateUIForWaiting", 0, 1);
        username = PlayerPrefs.GetString("username");

        NetworkManager.Instance.StopConnection();
        NetworkManager.Instance.connect("_RAND_");

        NetworkManager.OnGameServerCredentialsRecieved += OnGameServerCredentialsRecieved;
        NetworkManager.OnServerConnected += OnServerConnected;
        NetworkManager.OnServerDisconnect += OnServerDisconnect;
        NetworkManager.OnListenDataEnded += OnListenDataEnded;
        NetworkManager.OnBoardData += OnBoardData;

    }

    public void UpdateUIForWaiting()
    {
        if(dots == 1)
            waitingText.text = "." + "הרחתמ שופיח";
        else if(dots == 2)
            waitingText.text = ".." + "הרחתמ שופיח";
        else
            waitingText.text = "..." + "הרחתמ שופיח";
        dots++;
        if (dots > 3)
            dots = 1;
    }
    

    public void GotoMainMenu()
    {
        try
        {
            SceneManager.LoadScene(1);
        }
        catch(Exception e)
        {
            SceneManager.LoadScene(1);
        }
        
    }


    private void OnBoardData(JSONObject js)
    {
        UnityThread.Instance.RunOnMainThread.Enqueue(() =>
        {
            try
            {
                Debug.Log(js);
                boardData = js.GetField("_BOARD_").str;
                string gameStatus = js.GetField("_STATUS_").str;

                if (gameStatus == "1")
                {
                    js.RemoveField(username);
                    js.RemoveField("_BOARD_");
                    js.RemoveField("_STATUS_");

                    JSONObject avatars = js.GetField("AVATARS");
                    avatars.RemoveField("AVATAR_ID_" + username);


                    Dictionary<string, string> dic = avatars.ToDictionary();
                    Dictionary<string, string>.KeyCollection keys = dic.Keys;

                    foreach (string key in keys)
                    {
                        op_username = key;
                        op_imagID = js.GetField("AVATARS").GetField(op_username).str;
                    }

                    if (op_username != null && boardData != null && op_imagID != null)
                    {
                        PlayerPrefs.SetString("OpponentName", op_username.Substring(10));
                        PlayerPrefs.SetString("Board", boardData);
                        PlayerPrefs.SetInt("op_picID", Convert.ToInt32(op_imagID));
                        Debug.Log("-------------Start GAME---------");
                        SceneManager.LoadScene(3);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Main Menu OnBoardData : " + e);
            }
        });
    }


    private void OnListenDataEnded(string s)
    {
        Debug.Log("OnListenDataEnded : " + s);
    }

    private void OnServerDisconnect(string s)
    {
        Debug.Log("Opponent Waiting Area : " + s);
    }

    private void OnServerConnected(string s)
    {
        Debug.Log("Opponent Waiting Area : " + s);
    }

    private void OnGameServerCredentialsRecieved(JSONObject js)
    {
        UnityThread.Instance.RunOnMainThread.Enqueue(() =>
        {
            try
            {
                NetworkManager.Instance.socketConnection.Close();
                string ip = js.GetField("IP").str;
                int port = (int)js.GetField("PORT").n;
                NetworkManager.Instance.ConnectToServer(ip, port, "", "");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

        });
    }

    private void OnDisable()
    {
        NetworkManager.OnGameServerCredentialsRecieved -= OnGameServerCredentialsRecieved;
        NetworkManager.OnServerConnected -= OnServerConnected;
        NetworkManager.OnServerDisconnect -= OnServerDisconnect;
        NetworkManager.OnListenDataEnded -= OnListenDataEnded;
        NetworkManager.OnBoardData -= OnBoardData;
    }
}
