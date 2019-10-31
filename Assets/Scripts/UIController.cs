using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UIController : MonoBehaviour
{
    public static UIController UI;

    public ActionSelectedEvent actionSelectedEvent;
    public AttackSelectedEvent attackSelectedEvent;
    public SpAttackSelectedEvent spAttackSelectedEvent;
    public ExAttackSelectedEvent exAttackSelectedEvent;
    public GuardSelectedEvent guardSelectedEvent;
    public ItemSelectedEvent itemSelectedEvent;
    public RunAwayEvent runAwayEvent;
    public RestartGameEvent restartGameEvent;

    [SerializeField] GameObject GameEndedPanel;

    private void Awake()
    {
        #region Singleton
        if (UI == null)
            UI = this;
        else if (UI != this)
            Destroy(gameObject);
        #endregion
    }

    void Start()
    {
        actionSelectedEvent = new ActionSelectedEvent();
        attackSelectedEvent = new AttackSelectedEvent();
        spAttackSelectedEvent = new SpAttackSelectedEvent();
        exAttackSelectedEvent = new ExAttackSelectedEvent();
        guardSelectedEvent = new GuardSelectedEvent();
        itemSelectedEvent = new ItemSelectedEvent();
        runAwayEvent = new RunAwayEvent();
        restartGameEvent = new RestartGameEvent();
    }

    private void OnEnable()
    {
        EventController.AddListener<BattleLoseEvent>(OpenEndedGamePanel);
        EventController.AddListener<BattleWinEvent>(OpenEndedGamePanel);

    }

    private void OnDisable()
    {
        EventController.RemoveListener<BattleLoseEvent>(OpenEndedGamePanel);
        EventController.RemoveListener<BattleWinEvent>(OpenEndedGamePanel);
    }

    public void ActionSelected()
    {
        EventController.TriggerEvent(actionSelectedEvent);
    }

    public void AttackSelected()
    {
        EventController.TriggerEvent(attackSelectedEvent);
    }

    public void SpAttackSelected()
    {
        EventController.TriggerEvent(spAttackSelectedEvent);
    }

    public void ExAttackSelected()
    {
        EventController.TriggerEvent(exAttackSelectedEvent);
    }

    public void GuardSelected()
    {
        EventController.TriggerEvent(guardSelectedEvent);
    }

    public void RunAwaySelected()
    {
        EventController.TriggerEvent(runAwayEvent);
    }

    public void ItemSelected()
    {
        EventController.TriggerEvent(itemSelectedEvent);
    }

    public void RestartGame()
    {
        EventController.TriggerEvent(restartGameEvent);
    }

    void OpenEndedGamePanel(BattleWinEvent evento)
    {
        GameEndedPanel.SetActive(true);
    }

    void OpenEndedGamePanel(BattleLoseEvent evento)
    {
        GameEndedPanel.SetActive(true);
    }
}
