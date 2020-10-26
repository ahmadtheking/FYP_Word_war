using System.Linq;
using UnityEngine;

public static class PreferenceManager
{
    public static string Username
    {
        get { return PlayerPrefs.GetString("USER_NAME", null); }

        set { PlayerPrefs.SetString("USER_NAME", value); }
    }

    public static string Gender
    {
        get { return PlayerPrefs.GetString("GENDER", "MALE"); }

        set { PlayerPrefs.SetString("GENDER", value); }
    }

    public static int Color
    {
        get { return PlayerPrefs.GetInt("COLOR", 1); }

        set { PlayerPrefs.SetInt("COLOR", value); }
    }

    public static int Cap
    {
        get { return PlayerPrefs.GetInt("CAP", 1); }

        set { PlayerPrefs.SetInt("CAP", value); }
    }

    public static int Glasses
    {
        get { return PlayerPrefs.GetInt("GLASSES", 1); }

        set { PlayerPrefs.SetInt("GLASSES", value); }
    }

    public static string TableKey
    {
        get { return PlayerPrefs.GetString("TABLE_KEY", ""); }

        set { PlayerPrefs.SetString("TABLE_KEY", value); }
    }
}
