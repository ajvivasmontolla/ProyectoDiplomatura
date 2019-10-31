using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class Fighter : MonoBehaviour
{
    //Stats del personaje ==> van a venir del scriptable object
    int baseLifePoints = 100;
    int baseAttackPoints = 15;
    int baseDefensePoints = 15;
    int baseActionPoints = 100;

    //Valores usados durante la pelea
    //pueden ser modificados mediante acciones/items
    int battleLifePoints;
    int battleAttackPoints;
    int battleDefensePoints;
    int battleActionPoints;

    public Attack attack;
    public SpecialAttack specialAttack;
    public SpecialAttack extraAttack;
    public Item item;

    public FighterDead fighterDead;
    public FighterType fighterType = FighterType.None;

    public bool isDead = false;
    private static bool _waitingAction = false;
    private static bool _executingAction = false;

    public EndTurnEvent endTurnEvent;

    private void Start()
    {
        fighterDead = new FighterDead();
        endTurnEvent = new EndTurnEvent();
    }

    // Start is called before the first frame update
    void Awake()
    {
        battleActionPoints = baseActionPoints;
        battleLifePoints = baseLifePoints;
        battleDefensePoints = baseDefensePoints;
        battleAttackPoints = baseAttackPoints;
        
        if (gameObject.CompareTag("Player"))
            fighterType = FighterType.Player;
        if (gameObject.CompareTag("Enemy"))
            fighterType = FighterType.Enemy;
    }

    public void SelectAction()
    {
        Debug.Log("SelectAction Started - " + gameObject.name);
        _waitingAction = true;
        _executingAction = true;

        if (gameObject.CompareTag("Enemy"))
        {
            UIController.UI.ActionSelected();

            int action = (int)Random.Range(0f, 3.9f);
            Debug.Log("Accion AI seleccionada: " + action);
            switch (action)
            {
                case 0:
                    UIController.UI.AttackSelected();
                    break;
                case 1:
                    UIController.UI.SpAttackSelected();
                    break;
                case 2:
                    UIController.UI.ExAttackSelected();
                    break;
                case 3:
                    UIController.UI.GuardSelected();
                    break;
                default:
                    break;
            }
        }
    }

    void OnEnable()
    {
        EventController.AddListener<ActionSelectedEvent>(ActionSelected);
        EventController.AddListener<RestartGameEvent>(GameRestarted);
    }

    void OnDisable()
    {
        EventController.RemoveListener<ActionSelectedEvent>(ActionSelected);
        EventController.RemoveListener<RestartGameEvent>(GameRestarted);
    }

    void ActionSelected(ActionSelectedEvent evento)
    {
        _waitingAction = false;
    }

    public void Attack(GameObject objetivo, GameObject playerObjetivo, GameObject enemyObjetivo)
    {
        //Usamos los valores del ScriptableObject del ataque para calcular daños
        Debug.Log("Used Attack - " + gameObject.name);
        Fighter enemyStats;

        if (fighterType == FighterType.Player)
            enemyStats = enemyObjetivo.GetComponent<Fighter>();
        else if (fighterType == FighterType.Enemy)
            enemyStats = playerObjetivo.GetComponent<Fighter>();
        else enemyStats = objetivo.GetComponent<Fighter>();

        int danio = Mathf.FloorToInt(0.5f * attack.attackPower * (GetAttackPoints() / enemyStats.GetDefensePoints())) + 1;

        enemyStats.GetDamage(danio);
        EventController.TriggerEvent(endTurnEvent);
    }

    public void SpecialAttack(GameObject objetivo, GameObject playerObjetivo, GameObject enemyObjetivo)
    {
        if (battleActionPoints >= specialAttack.actionPointsRequired)
        {

            //Usamos los valores del ScriptableObject del ataque especial para calcular daños y efectos
            Debug.Log("Used SpAttack - " + gameObject.name);
            Fighter enemyStats;

            if (fighterType == FighterType.Player)
                enemyStats = enemyObjetivo.GetComponent<Fighter>();
            else if (fighterType == FighterType.Enemy)
                enemyStats = playerObjetivo.GetComponent<Fighter>();
            else enemyStats = objetivo.GetComponent<Fighter>();

            int danio = Mathf.FloorToInt(0.5f * specialAttack.attackPower * (GetAttackPoints() / enemyStats.GetDefensePoints())) + 5;
            battleActionPoints = battleActionPoints - specialAttack.actionPointsRequired;

            enemyStats.GetDamage(danio);

            EventController.TriggerEvent(endTurnEvent);
        }
        else
        {
            Debug.Log("Not enough action points...");
            this.SelectAction();
        }
    }

    public void ExtraAttack(GameObject objetivo, GameObject playerObjetivo, GameObject enemyObjetivo)
    {
        if (extraAttack != null)
        {
            if (battleActionPoints >= extraAttack.actionPointsRequired)
            {

                //Usamos los valores del ScriptableObject del ataque especial para calcular daños y efectos
                Debug.Log("Used ExAttack - " + gameObject.name);
                Fighter enemyStats;

                if (fighterType == FighterType.Player)
                    enemyStats = enemyObjetivo.GetComponent<Fighter>();
                else if (fighterType == FighterType.Enemy)
                    enemyStats = playerObjetivo.GetComponent<Fighter>();
                else enemyStats = objetivo.GetComponent<Fighter>();

                int danio = Mathf.FloorToInt(0.5f * extraAttack.attackPower * (GetAttackPoints() / enemyStats.GetDefensePoints())) + 5;
                battleActionPoints = battleActionPoints - extraAttack.actionPointsRequired;

                enemyStats.GetDamage(danio);

                EventController.TriggerEvent(endTurnEvent);
            }
            else
            {
                Debug.Log("Not enough action points...");
                this.SelectAction();

            }
        }
        else Debug.Log("No has debloqueado ninguna habilidad extra!");
        this.SelectAction();
    }

    public void Guard(GameObject objetivo, GameObject playerObjetivo, GameObject enemyObjetivo)
    {
        //No requiere Scriptable object
        Debug.Log("Used Guard - " + gameObject.name);
        //_waitingAction = false;
        EventController.TriggerEvent(endTurnEvent);
    }

    public void UseItem(GameObject objetivo, GameObject playerObjetivo, GameObject enemyObjetivo)
    {
        if ((item != null) && item.itemQuantity > 0 && battleActionPoints >= item.actionPointsRequired)
        {
            //Usamos los valores del ScriptableObject del ataque especial para calcular daños y efectos
            Debug.Log("Used Item - " + gameObject.name);
            Fighter enemyStats;

            if (fighterType == FighterType.None)
                enemyStats = objetivo.GetComponent<Fighter>();
            else if (fighterType != item.appliesOn)
                enemyStats = enemyObjetivo.GetComponent<Fighter>();
            else //(fighterType == item.appliesOn)
                enemyStats = playerObjetivo.GetComponent<Fighter>();            

            item.Effect(enemyStats);
            item.itemQuantity--;
            battleActionPoints = battleActionPoints - specialAttack.actionPointsRequired;

            EventController.TriggerEvent(endTurnEvent);
        }
        else
        {
            if (item == null)
                Debug.Log("You don't have any item...");
            if (item.itemQuantity <= 0)
                Debug.Log("You don't have any " + item.itemName + " left...");
            else if (battleActionPoints < item.actionPointsRequired)
                Debug.Log("Not enough action points...");

            this.SelectAction();
        }
    }

    public void GetDamage(int danio)
    {
        if (!this.isDead)
        {
            Debug.Log("Aplicando daño: " + danio);
            battleLifePoints = battleLifePoints - danio;

            ScreenLog.log.AddEvent(this.name + " recibio " + danio + " puntos de daño!");

            if (battleLifePoints <= 0)
            {
                fighterDead.fighterType = fighterType;
                this.isDead = true;
                EventController.TriggerEvent(fighterDead);
            }
        }
    }

    private void GameRestarted(RestartGameEvent evento)
    {
        battleLifePoints = baseLifePoints;
        battleActionPoints = baseActionPoints;
        if (item != null)
        {
            item.itemQuantity = 5;
        }
    }

    public int GetLifePoints() { return battleLifePoints; }
    public int GetDefensePoints() { return battleDefensePoints; }
    public int GetActionPoints() { return battleActionPoints; }
    public int GetAttackPoints() { return battleAttackPoints; }

    public int GetBaseLifePoints() { return baseLifePoints; }
    public int GetBaseDefensePoints() { return baseDefensePoints; }
    public int GetBaseActionPoints() { return baseActionPoints; }
    public int GetBaseAttackPoints() { return baseAttackPoints; }

    public void SetLifePoint(int value) { battleLifePoints = value; }
    public void SetDefensePoints(int value) { battleDefensePoints = value; }
    public void SetActionPoints(int value) { battleActionPoints = value; }
    public void SetAttackPoints(int value) { battleAttackPoints = value; }
}
