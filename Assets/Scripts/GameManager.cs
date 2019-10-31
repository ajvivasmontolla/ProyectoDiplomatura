using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    public bool isRunning = true;

    private GameObject[] fighters;
    public Object[] _allItems; //Scriptable Object
    public Object[] _allAttacks; //Scriptabel Objects
    public Object[] _allSpecialAttacks; //Scriptable Objects


    void Awake()
    {
        #region Singleton
        if (GM == null)
        {
            DontDestroyOnLoad(gameObject);
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }
        #endregion

        _allItems = Resources.LoadAll("Resources/ScriptObjs/Items");
    }

    void OnEnable()
    {
        EventController.AddListener<BattleLoseEvent>(BattleLost);
        EventController.AddListener<BattleWinEvent>(BattleWon);
        EventController.AddListener<EscapeBattleEvent>(EscapeBattle);
    }

    void OnDisable()
    {
        EventController.RemoveListener<BattleLoseEvent>(BattleLost);
        EventController.RemoveListener<BattleWinEvent>(BattleWon);
        EventController.RemoveListener<EscapeBattleEvent>(EscapeBattle);
    }

    void BattleLost(BattleLoseEvent evento)
    {
        Debug.Log("Perdiste.");
        ScreenLog.log.AddEvent("Perdiste! T-T");
    }

    void BattleWon(BattleWinEvent evento)
    {
        Debug.Log("Ganaste.");
        ScreenLog.log.AddEvent("Ganaste! TuT");
    }

    void EscapeBattle(EscapeBattleEvent evento)
    {
        Debug.Log("Escapaste.");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
