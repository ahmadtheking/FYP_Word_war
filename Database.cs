using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Database : MonoBehaviour
{
    /*
     * SINGLTON CLASS THAT IS ACCESSABLE TO ALL THE SCENES IN THE GAME, 
     * MANAGES ALL THE NETWORKING ASPECTS OF THE GAME.
    */
    private static Database _instance;
    public static Database Instance { get { return _instance; } }

    public List<string> words;
    public List<string> medical;
    public List<string> computer;
    public List<string> physics;

    private Thread clientReceiveThread;
    TextAsset tt;
    string ssss;
    string medical_ssss;
    string computer_ssss;
    string physics_ssss;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Database Created");
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void Start()
    {
        words = new List<string>();
        medical = new List<string>();
        computer = new List<string>();
        physics = new List<string>();
        //StartCoroutine(loadFromResourcesFolder());

        tt = Resources.Load<TextAsset>("eng_words");
        ssss = tt.text;
        clientReceiveThread = new Thread(() => CollectData());
        clientReceiveThread.IsBackground = true;
        clientReceiveThread.Start();



        tt = Resources.Load<TextAsset>("comp_words");
        computer_ssss = tt.text;
        clientReceiveThread = new Thread(() => CollectData_COMP());
        clientReceiveThread.IsBackground = true;
        clientReceiveThread.Start();


        tt = Resources.Load<TextAsset>("medi_words");
        medical_ssss = tt.text;
        clientReceiveThread = new Thread(() => CollectData_MED());
        clientReceiveThread.IsBackground = true;
        clientReceiveThread.Start();


        tt = Resources.Load<TextAsset>("phy_words");
        physics_ssss = tt.text;
        clientReceiveThread = new Thread(() => CollectData_PHY());
        clientReceiveThread.IsBackground = true;
        clientReceiveThread.Start();
    }




    void CollectData()
    {
        Debug.Log(ssss);
        JSONObject js = new JSONObject(ssss);

        IEnumerable<string> ss = js.PrintAsync();
        using (var sequenceEnum = ss.GetEnumerator())
        {
            while (sequenceEnum.MoveNext())
            {
                // Do something with sequenceEnum.Current.
                if (sequenceEnum.Current != null)
                {
                    string currentWord = sequenceEnum.Current;
                    currentWord = currentWord.Replace("[", "");
                    currentWord = currentWord.Replace("]", "");
                    currentWord = currentWord.Replace("\"", "");

                    foreach (string temp in currentWord.Split(','))
                    {
                        words.Add(temp);
                    }
                }
            }
        }
    }


    void CollectData_COMP()
    {
        Debug.Log(computer_ssss);
        JSONObject js = new JSONObject(computer_ssss);

        IEnumerable<string> ss = js.PrintAsync();
        using (var sequenceEnum = ss.GetEnumerator())
        {
            while (sequenceEnum.MoveNext())
            {
                // Do something with sequenceEnum.Current.
                if (sequenceEnum.Current != null)
                {
                    string currentWord = sequenceEnum.Current;
                    currentWord = currentWord.Replace("[", "");
                    currentWord = currentWord.Replace("]", "");
                    currentWord = currentWord.Replace("\"", "");

                    foreach (string temp in currentWord.Split(','))
                    {
                        computer.Add(temp);
                    }
                }
            }
        }
    }

    void CollectData_MED()
    {
        Debug.Log(medical_ssss);
        JSONObject js = new JSONObject(medical_ssss);

        IEnumerable<string> ss = js.PrintAsync();
        using (var sequenceEnum = ss.GetEnumerator())
        {
            while (sequenceEnum.MoveNext())
            {
                // Do something with sequenceEnum.Current.
                if (sequenceEnum.Current != null)
                {
                    string currentWord = sequenceEnum.Current;
                    currentWord = currentWord.Replace("[", "");
                    currentWord = currentWord.Replace("]", "");
                    currentWord = currentWord.Replace("\"", "");

                    foreach (string temp in currentWord.Split(','))
                    {
                        medical.Add(temp);
                    }
                }
            }
        }
    }

    void CollectData_PHY()
    {
        Debug.Log(physics_ssss);
        JSONObject js = new JSONObject(physics_ssss);

        IEnumerable<string> ss = js.PrintAsync();
        using (var sequenceEnum = ss.GetEnumerator())
        {
            while (sequenceEnum.MoveNext())
            {
                // Do something with sequenceEnum.Current.
                if (sequenceEnum.Current != null)
                {
                    string currentWord = sequenceEnum.Current;
                    currentWord = currentWord.Replace("[", "");
                    currentWord = currentWord.Replace("]", "");
                    currentWord = currentWord.Replace("\"", "");

                    foreach (string temp in currentWord.Split(','))
                    {
                        physics.Add(temp);
                    }
                }
            }
        }
    }



    IEnumerator loadFromResourcesFolder()
    {
        //Request data to be loaded
        ResourceRequest loadAsync = Resources.LoadAsync("eng_words", typeof(TextAsset));

        //Wait till we are done loading
        while (!loadAsync.isDone)
        {
            Debug.Log("Load Progress: " + loadAsync.progress);
            yield return null;
        }

        //Get the loaded data
        TextAsset tt = loadAsync.asset as TextAsset;

        Debug.LogError(tt.text);
        JSONObject js = new JSONObject(tt.text);

        IEnumerable<string> ss = js.PrintAsync();
        using (var sequenceEnum = ss.GetEnumerator())
        {
            while (sequenceEnum.MoveNext())
            {
                // Do something with sequenceEnum.Current.
                if (sequenceEnum.Current != null)
                {
                    string currentWord = sequenceEnum.Current;
                    currentWord = currentWord.Replace("[", "");
                    currentWord = currentWord.Replace("]", "");
                    currentWord = currentWord.Replace("\"", "");

                    foreach (string temp in currentWord.Split(','))
                    {
                        words.Add(temp);
                        Debug.Log(temp);
                    }
                }
            }
        }

    }


    public bool IsWord(string s)
    {
        if (PlayerPrefs.GetString("gameType") == "practice")
        {
            if (words.Contains(s))
            {
                Debug.Log("----------IN-------------");
                return true;
            }
        }
        else if (PlayerPrefs.GetString("gameType") == "computer")
        {
            if (computer.Contains(s))
            {
                Debug.Log("----------IN-------------");
                return true;
            }
        }
        else if (PlayerPrefs.GetString("gameType") == "physics")
        {
            if (physics.Contains(s))
            {
                Debug.Log("----------IN-------------");
                return true;
            }
        }
        else if (PlayerPrefs.GetString("gameType") == "medical")
        {
            if (medical.Contains(s))
            {
                Debug.Log("----------IN-------------");
                return true;
            }
        }

        Debug.Log("----------OUT-------------");
        return false;
    }
}
