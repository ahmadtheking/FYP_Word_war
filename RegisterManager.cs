using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

public class RegisterManager : MonoBehaviour
{
    public Text username;
    public Text password;
    public Text confirm_password;

    public Animator anim_search;
    public Text infoText;

    public void Register()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        PlayerInfo player = new PlayerInfo();
        player.user_name = username.text;
        player.coins = 500;
        player.xp = 0;
        player.weekly_rounds = 0;
        if(password.text == confirm_password.text)
        {
            player.password = password.text;
        }
        else
        {
            Debug.LogError("Password must match");
            infoText.text = "Password must match";
            anim_search.SetBool("showpanel", true);
            yield break;
        }

        string player_JSON = player.SaveToString();

        var uwr = new UnityWebRequest("http://127.0.0.1:5000/user", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(player_JSON);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();


        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            infoText.text = uwr.error;
            anim_search.SetBool("showpanel", true);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            JSONObject response = new JSONObject(uwr.downloadHandler.text);

            if(response.GetField("message").n == 0)
            {
                infoText.text = "Username already exists."; //Username already exists.
                anim_search.SetBool("showpanel", true);
            }
            else if (response.GetField("message").n == 1)
            {
                StartCoroutine(Login(username.text, password.text));
                infoText.text = "";
                anim_search.SetBool("showpanel", true);
            }
            else
            {
                infoText.text = "Error : " + uwr.downloadHandler.text;
                anim_search.SetBool("showpanel", true);
            }
        }
    }

    public string Reverse(string s)
    {
        return s;
        /*
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
        */

    }
    IEnumerator Login(string usrn, string pswd)
    {
        JSONObject js = new JSONObject();
        js.AddField("user_name", usrn);
        js.AddField("password", pswd);

        using (UnityWebRequest webRequest = new UnityWebRequest("http://127.0.0.1:5000/user-verify", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(js.ToString());
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
                infoText.text = webRequest.error;
                anim_search.SetBool("showpanel", true);
            }
            else
            {
                JSONObject response = new JSONObject(webRequest.downloadHandler.text);
                Debug.Log("Received: " + response.ToString());

                if (response.HasField("uID") && response.HasField("password"))
                {
                    Debug.Log("Login");

                    PlayerPrefs.SetString("username", response.GetField("uID").str);
                    PlayerPrefs.SetString("password", response.GetField("password").str);
                    PlayerPrefs.SetInt("coins", (int)response.GetField("coins").n);
                    PlayerPrefs.SetInt("weekly_rounds", (int)response.GetField("weekly_rounds").n);
                    PlayerPrefs.SetInt("xp", (int)response.GetField("xp").n);

                    SceneManager.LoadScene(1);
                }
                else
                {
                    infoText.text = Reverse("Please write down your correct credentials"); //Please write down your correct credentials
                    anim_search.SetBool("showpanel", true);
                }
            }
        }
    }



    public void goto_LoginScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void Close_UserPlay_Panel()
    {
        anim_search.SetBool("showpanel", false);
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string user_name;
    public int coins;
    public int xp;
    public int weekly_rounds;
    public string password;

    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}