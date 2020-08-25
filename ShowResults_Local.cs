using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShowResults_Local : MonoBehaviour
{
    public Image myImage;
    public Text myname;
    public Text myScore;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("picID"))
        {
            myImage.sprite = NetworkManager.Instance.sprites[PlayerPrefs.GetInt("picID")];
        }
        else
        {
            myImage.sprite = NetworkManager.Instance.sprites[0];
        }

        myname.text = NetworkManager.Instance.username;
        myScore.text = PlayerPrefs.GetString("localScore");

    }

    public void Back()
    {
        SceneManager.LoadScene(1);
    }
}
