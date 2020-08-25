using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public Text usernmae;
    public Text coins;
    public Text XP;
    public Slider xp_bar;

    public GameObject RequestPanel;
    public AudioClip req_audio;
    public Image myPic;
    public GameObject pictureContainer;
    public Animator anim;
    public Animator anim_search;
    public Animator anim_leader;
    public Text search_user;
    public Sprite LessCoins;
    public GameObject personContainer;
    public GameObject person_leaderBoard;
    public GameObject adManager;
    public GameObject searchforUserButton;

    private AudioSource audiosource;
    private Button[] btns;

    private string username;
    private string op_username = null;
    private string op_imagID = null;
    private string boardData = null;

    private bool lessCoins = false;

    void Start()
    {

        username = PlayerPrefs.GetString("username");

        if (PlayerPrefs.HasKey("picID"))
        {
            myPic.sprite = NetworkManager.Instance.sprites[PlayerPrefs.GetInt("picID")];
        }
        else
        {
            myPic.sprite = NetworkManager.Instance.sprites[0];
        }
        
        btns = new Button[42];
        for (int i = 0; i < btns.Length; i++)
        {
            pictureContainer.GetComponent<Transform>().GetChild(i).gameObject.GetComponent<Image>().sprite = NetworkManager.Instance.sprites[i];
            btns[i] = pictureContainer.GetComponent<Transform>().GetChild(i).gameObject.GetComponent<Button>();

            int a = i;
            //next, any of these will work:
            btns[a].onClick.AddListener(() => { ChangeProfilePic(a); });
        }

        audiosource = gameObject.GetComponent<AudioSource>();


        //Invoke("RequestIncoming", 1);
        //Invoke("RequestHandeled", 6);

        try
        {
            if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password") && PlayerPrefs.HasKey("coins") && PlayerPrefs.HasKey("xp"))
            {
                usernmae.text = PlayerPrefs.GetString("username");

                if (PlayerPrefs.GetInt("coins") > 9999)
                {
                    string sc = PlayerPrefs.GetInt("coins").ToString();
                    sc = sc.Substring(0, sc.Length - 3) + "K";
                    coins.text = sc;
                }
                else
                {
                    coins.text = PlayerPrefs.GetInt("coins").ToString();
                }

                // xp = 50;
                // (1550, 1500 , 1600, 0, 1) => 0.5 Slider Value


                decimal level = Math.Floor((decimal)(PlayerPrefs.GetInt("xp") + 1) / (decimal)100);
                decimal next_level = Math.Ceiling((decimal)(PlayerPrefs.GetInt("xp") + 1) / (decimal)100);


                decimal norm = map(PlayerPrefs.GetInt("xp"), ((level < 1) ? 1 : level * 100), (next_level * 100), 0, 1);


                xp_bar.GetComponent<Slider>().value = (float)norm;

                XP.text = level.ToString();

                /*
                Debug.Log("XP : " + PlayerPrefs.GetInt("xp"));
                Debug.Log("This Level : " + level);
                Debug.Log("Next Level : " + next_level);
                Debug.Log("Slider : " + (float)norm);
                */

            }
            else
            {
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene(0);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(0);
        }

        NetworkManager.Instance.username = username;


        NetworkManager.Instance.StopConnection();
        NetworkManager.Instance.connect("_CONEC_");
        Debug.Log("___SENT____CONEC___FLAG___BY___MENU-MANAGER___START");

        NetworkManager.OnGameServerCredentialsRecieved += OnGameServerCredentialsRecieved;

        NetworkManager.OnBoardData += OnBoardData;
        NetworkManager.OnServerConnected += OnServerConnected;
        NetworkManager.OnServerDisconnect += OnServerDisconnect;
        NetworkManager.OnListenDataEnded += OnListenDataEnded;
        NetworkManager.OnPeerRequest += OnPeerRequest;
        NetworkManager.OnPeerDeclined += OnPeerDeclined;

        StartCoroutine(getLeaderBoard());
    }

    IEnumerator getLeaderBoard()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("http://server.wordsfight.com/leader-board"))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                //Debug.Log(webRequest.downloadHandler.text);
                try
                {
                    JSONObject js = new JSONObject(webRequest.downloadHandler.text);

                    foreach(string s in js.PrintAsync())
                    {
                        string ss = s.Replace("{", "");
                        ss = ss.Replace("}", "");
                        ss = ss.Replace("\"", "");
                        string[] sss = ss.Split(',');

                        foreach(string ssx in sss)
                        {
                           string[] locals = ssx.Split(':');
                            //Debug.Log(locals[0] + " : " + locals[1]);

                            GameObject g = Instantiate(person_leaderBoard, personContainer.transform);
                            g.transform.GetChild(0).GetComponent<Text>().text = locals[0]; //name
                            g.transform.GetChild(1).GetComponent<Text>().text = locals[1]; //score
                        }
                    }

                }
                catch (Exception e)
                {

                    Debug.Log("NO USER");
                }

            }
        }
    }




    IEnumerator ChangeButtonUI(Button b)
    {
        Sprite s = b.GetComponent<Image>().sprite;
        b.GetComponent<Image>().sprite = LessCoins;
        yield return new WaitForSeconds(5);
        b.GetComponent<Image>().sprite = s;
    }
    


    public void PlayRandom(Button b)
    {
        if(PlayerPrefs.GetInt("coins") >= 100)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            if (b.GetComponent<Image>().sprite != LessCoins)
            {
                Debug.LogError("Less Coins");
                StartCoroutine(ChangeButtonUI(b));
            }
            else
            {
                Debug.LogError("OPEN ADD");
                adManager.GetComponent<AdManager>().Display_VideoRewardAd();
            }
        }
    }

    public void ShowLeaderBoard()
    {
        anim_leader.SetBool("show", true);
    }

    public void CloseLeaderBoard()
    {
        anim_leader.SetBool("show", false);
    }

    public void PlayWithUser(Button b)
    {
        if (PlayerPrefs.GetInt("coins") >= 100)
        {
            anim_search.SetBool("showpanel", true);
        }
        else
        {
            if (b.GetComponent<Image>().sprite != LessCoins)
            {
                Debug.LogError("Less Coins");
                StartCoroutine(ChangeButtonUI(b));
            }
            else
            {
                Debug.LogError("OPEN ADD");
                adManager.GetComponent<AdManager>().Display_VideoRewardAd();
            }
        }
    }
    public void Close_UserPlay_Panel()
    {
        anim_search.SetBool("showpanel", false);
    }

    public void SearchForUsers()
    {
        Debug.LogWarning(search_user.text);
        NetworkManager.Instance.StopConnection();
        NetworkManager.Instance.ConnectToServer("server.wordsfight.com", 4444, "_REQ_", search_user.text);
        StartCoroutine(UpdateUIForWaiting());
    }

    public string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    IEnumerator UpdateUIForWaiting()
    {
        searchforUserButton.GetComponent<Button>().interactable = false;
        searchforUserButton.GetComponentInChildren<Text>().text = "." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = ".." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "..." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = ".." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "..." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = ".." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "..." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = ".." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "..." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = "." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponentInChildren<Text>().text = ".." + Reverse("מחכה");
        yield return new WaitForSeconds(1);

        searchforUserButton.GetComponent<Button>().interactable = true;
        searchforUserButton.GetComponentInChildren<Text>().text = "שפחל";


    }

    private void ChangeProfilePic(int a)
    {
        PlayerPrefs.SetInt("picID", a);
        myPic.sprite = NetworkManager.Instance.sprites[a];
    }

    public void OpenPicPanle()
    {
        anim.SetBool("picturebox", true);
    }
    public void ClosePicPanel()
    {
        anim.SetBool("picturebox", false);
    }

    public void PlaySolo()
    {
        SceneManager.LoadScene(6);
    }

    public void SelectGameType()
    {
        SceneManager.LoadScene(8);
    }

    public void SelectGameType(String type)
    {
        PlayerPrefs.SetString("gameType", type);
        SceneManager.LoadScene(6);
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


    private void OnPeerRequest(string s)
    {
        Debug.Log("REQUEST BY----------------------> " + s);
        Action rot = RequestIncoming;
        
        UnityThread.Instance.RunOnMainThread.Enqueue(() => {

            audiosource.clip = req_audio;
            audiosource.Play();
            RequestPanel.GetComponent<Transform>().GetChild(0).GetComponent<Text>().text = s + " רוצה לשחק נגדך";
            RequestPanel.GetComponent<Animator>().SetBool("request", true);
        });
    }

    public void OnPeerDeclined(string s)
    {
        UnityThread.Instance.RunOnMainThread.Enqueue(() => {
            NetworkManager.Instance.StopConnection();
            NetworkManager.Instance.connect("_CONEC_");
            RequestPanel.GetComponent<Animator>().SetBool("request", false);
        });
    }

    public void PeerReqYES()
    {
        if (PlayerPrefs.GetInt("coins") >= 100)
        {
            NetworkManager.Instance.SendMessageToServer("_YES_");
        }
        else
        {
            NetworkManager.Instance.SendMessageToServer("_NO_");
        }

        RequestPanel.GetComponent<Animator>().SetBool("request", false);
    }

    public void PeerReqNO()
    {
        NetworkManager.Instance.SendMessageToServer("_NO_");
        RequestPanel.GetComponent<Animator>().SetBool("request", false);
    }

    void RequestIncoming()
    {
        audiosource.clip = req_audio;
        audiosource.Play();
        RequestPanel.GetComponent<Animator>().SetBool("request", true);
    }

    public void RequestHandeled()
    {
        RequestPanel.GetComponent<Animator>().SetBool("request", false);
    }
    private void OnListenDataEnded(string s)
    {
        Debug.Log("Listen DataEnded");
        //NetworkManager.Instance.StopConnection();
    }

    private void OnServerDisconnect(string s)
    {
        Debug.Log("Menu Manager : " + s);
    }

    private void OnServerConnected(string s)
    {
        Debug.Log("Menu Manager : " + s);
    }

    decimal map(decimal x, decimal in_min, decimal in_max, decimal out_min, decimal out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }


    private void OnDisable()
    {
        NetworkManager.OnGameServerCredentialsRecieved -= OnGameServerCredentialsRecieved;
        NetworkManager.OnBoardData -= OnBoardData;

        NetworkManager.OnServerConnected -= OnServerConnected;
        NetworkManager.OnServerDisconnect -= OnServerDisconnect;
        NetworkManager.OnListenDataEnded -= OnListenDataEnded;
        NetworkManager.OnPeerRequest -= OnPeerRequest;
        NetworkManager.OnPeerDeclined -= OnPeerDeclined;
    }

}
