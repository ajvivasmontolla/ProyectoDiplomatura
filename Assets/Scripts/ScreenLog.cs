using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenLog : MonoBehaviour
{
    // Private VARS
    public static ScreenLog log;
    private List<string> Eventlog = new List<string>();
    private string guiText = "";

    [SerializeField] Font font;
    [SerializeField] int maxLines = 7;
    [SerializeField] int fontSize = 30;

    private void Awake()
    {
        #region Singleton
        if (log == null)
        {
            DontDestroyOnLoad(gameObject);
            log = this;
        }
        else if (log != this)
        {
            Destroy(gameObject);
        }
        #endregion

    }

    void OnGUI()
    {
        GUI.skin.textArea.font = font;
        GUI.skin.textArea.fontSize = fontSize;
        GUI.Label(new Rect((Screen.width/3), Screen.height /4*3, Screen.width *2/3, Screen.height / 4), guiText, GUI.skin.textArea);
    }

    public void AddEvent(string eventString)
    {
        Eventlog.Add(eventString);

        if (Eventlog.Count >= maxLines)
            Eventlog.RemoveAt(0);

        guiText = "";

        foreach (string logEvent in Eventlog)
        {
            guiText += logEvent;
            guiText += "\n";
        }
    }
}

