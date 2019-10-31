using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class TurnController : MonoBehaviour
{
    public static TurnController turnController;
    public BattleLoseEvent battleLoseEvent;
    public BattleWinEvent battleWinEvent;
    public ObjectiveSelectedEvent objectiveSelectedEvent;
    public EndTurnEvent endTurnEvent;

    public delegate void ActionDelegate(GameObject seleccion, GameObject player, GameObject enemy);
    public ActionDelegate executeSelectedAction;

    private List<Fighter> fighters;
    Fighter fighter_now;
    private int fighters_cant;
    private int fighters_turn;
    private int enemies_cant;
    private int players_cant;

    private Fighter player_1;
    private Fighter player_2;
    private Fighter enemy_1;
    private Fighter enemy_2;
    GameObject enemySelected = null;
    GameObject playerSelected = null;
    GameObject fighterSelected = null;

    int dead_players = 0;
    int dead_enemies = 0;


    public bool _battleRunning = true;
    public bool _waitingForSelection = false;
    Event keyEvent;
    KeyCode pressedKey;

    [SerializeField] Light spotlight;
    [SerializeField] GameObject targetSprite;

    void Start()
    {
        #region "Singleton"
        if (turnController == null)
        {
            turnController = this;
        }
        else
        {
            Destroy(this);
        }
        #endregion

        fighters = new List<Fighter>();
        fighters_turn = 2;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Jugadores agregados");
        players_cant = players.Length;
        Debug.Log(players_cant);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("Enemigos agregados");
        enemies_cant = enemies.Length;
        Debug.Log(enemies_cant);

        foreach (GameObject player in players)
        {
            Fighter temp = player.GetComponent<Fighter>();
            Debug.Log(temp);

            if (player_1 == null) player_1 = temp;
            else player_2 = temp;

            fighters.Add(temp);
        }
        foreach (GameObject enemy in enemies)
        {
            Fighter temp = enemy.GetComponent<Fighter>();
            Debug.Log(temp);

            /*Buscar como agregarlos dinamicamente para seleccionar enemigo o aliado*/
            if (enemy_1 == null) enemy_1 = temp;
            else enemy_2 = temp;

            fighters.Add(temp);
        }

        fighters_cant= fighters.Count;

        battleLoseEvent = new BattleLoseEvent();
        battleWinEvent = new BattleWinEvent();
        objectiveSelectedEvent = new ObjectiveSelectedEvent();
        endTurnEvent = new EndTurnEvent();

        if (enemySelected == null)
            enemySelected = enemies[Random.value > 0.5f ? 0 : 1];
        if (playerSelected == null)
            playerSelected = players[Random.value > 0.5 ? 0 : 1];
        if (fighterSelected == null)
            fighterSelected = (Random.value > 0.5f ? playerSelected : enemySelected);

        fighters_turn = (int)Random.Range(0, 3);

        this.NextTurn();

        StartCoroutine(ObjectiveSelected());
        StartCoroutine(MoveSpotlight());
    }

    void NextTurn()
    {
        if (_battleRunning)
        {
            Debug.Log("Turno: " + fighters_turn);
            fighter_now = fighters[fighters_turn];

            Debug.Log("personaje actual: " + fighter_now);
            ScreenLog.log.AddEvent(fighter_now.name + " debe jugar...");

            fighters_turn = (fighters_turn + 1) % fighters_cant;
            if (!fighter_now.isDead)
            {
                Debug.Log("Invocando SelectAction() para el personaje actual");
                fighter_now.SelectAction();
            }
            else EventController.TriggerEvent(endTurnEvent);
        }
    }

    void OnEnable()
    {
        EventController.AddListener<AttackSelectedEvent>(AttackSelected);
        EventController.AddListener<SpAttackSelectedEvent>(SpAttackSelected);
        EventController.AddListener<ExAttackSelectedEvent>(ExAttackSelected);
        EventController.AddListener<GuardSelectedEvent>(GuardSelected);
        EventController.AddListener<ItemSelectedEvent>(ItemSelected);
        EventController.AddListener<EndTurnEvent>(TurnEnded);
        EventController.AddListener<FighterDead>(CountKills);
        EventController.AddListener<ObjectiveSelectedEvent>(ExecuteAction);
        EventController.AddListener<ActionSelectedEvent>(SelectObjective);
        EventController.AddListener<RestartGameEvent>(GameRestarted);
    }

    void OnDisable()
    {
        EventController.RemoveListener<AttackSelectedEvent>(AttackSelected);
        EventController.RemoveListener<SpAttackSelectedEvent>(SpAttackSelected);
        EventController.RemoveListener<ExAttackSelectedEvent>(ExAttackSelected);
        EventController.RemoveListener<GuardSelectedEvent>(GuardSelected);
        EventController.RemoveListener<ItemSelectedEvent>(ItemSelected);
        EventController.RemoveListener<EndTurnEvent>(TurnEnded);
        EventController.RemoveListener<FighterDead>(CountKills);
        EventController.RemoveListener<ObjectiveSelectedEvent>(ExecuteAction);
        EventController.RemoveListener<ActionSelectedEvent>(SelectObjective);
        EventController.RemoveListener<RestartGameEvent>(GameRestarted);
    }

    void OnGUI()
    {
        keyEvent = Event.current;
        if(keyEvent.isKey)
        {
            switch (keyEvent.keyCode)
            {
                case KeyCode.Q:
                    Debug.Log("Player1 targeted");
                    playerSelected = player_1.gameObject;
                    fighterSelected = player_1.gameObject;
                    break;
                case KeyCode.A:
                    Debug.Log("Player2 targeted");
                    playerSelected = player_2.gameObject;
                    fighterSelected = player_2.gameObject;
                    break;
                case KeyCode.E:
                    Debug.Log("Enemy1 targeted");
                    enemySelected = enemy_1.gameObject;
                    fighterSelected = enemy_1.gameObject;
                    break;
                case KeyCode.D:
                    Debug.Log("Enemy2 targeted");
                    enemySelected = enemy_2.gameObject;
                    fighterSelected = enemy_2.gameObject;
                    break;
            }
        }
    }

    #region Invocacion de Acciones
    void AttackSelected(AttackSelectedEvent evento)
    {
        /*fighter_now.Attack();*/
        Debug.Log("Attack() asignado para aejecutarse");
        executeSelectedAction = fighter_now.Attack;
    }

    void SpAttackSelected(SpAttackSelectedEvent evento)
    {
        /*fighter_now.SpecialAttack();*/
        Debug.Log("SpeAttack asignado para ejecutarse");
        executeSelectedAction = fighter_now.SpecialAttack;
    }

    void ExAttackSelected(ExAttackSelectedEvent evento)
    {
        /*fighter_now.ExtraAttack();*/
        Debug.Log("ExtraAttack asigando para ejecutarse");
        executeSelectedAction = fighter_now.ExtraAttack;
    }

    void GuardSelected(GuardSelectedEvent evento)
    {
        /*fighter_now.Guard();*/
        Debug.Log("Guard asignado para ejecutarse");
        executeSelectedAction = fighter_now.Guard;
    }

    void ItemSelected(ItemSelectedEvent evento)
    {
        /*fighter_now.UseItem();*/
        Debug.Log("UseItem asignado para ejecutarse");
        executeSelectedAction = fighter_now.UseItem;
    }

    void ExecuteAction(ObjectiveSelectedEvent evento)
    {
        Debug.Log("Ejecutando la accion asignada");
        executeSelectedAction(evento.objectiveSelected, evento.playerSelected, evento.enemySelected);
    }

    void TurnEnded(EndTurnEvent evento)
    {
        this.NextTurn();
    }
    #endregion

    void CountKills(FighterDead evento)
    { 
        if (evento.fighterType == FighterType.Enemy) dead_enemies++;
        if (evento.fighterType == FighterType.Player) dead_players++;
        Debug.Log("Luchadores muertos: Player-" + dead_players + " Enemigos-" + dead_enemies);

        if (dead_enemies >= enemies_cant)
        {
            _battleRunning = false;
            EventController.TriggerEvent(battleWinEvent);
        }
        if (dead_players >= players_cant)
        {
            _battleRunning = false;
            EventController.TriggerEvent(battleLoseEvent);
        }
    }

    public void SelectObjective(ActionSelectedEvent evento)
    {
        Debug.Log("Waiting for Selection ==> true");
        ScreenLog.log.AddEvent("Elegí tu objetivo...");
        _waitingForSelection = true;
    }

   private IEnumerator ObjectiveSelected()
   {        
        while (_battleRunning)
        {
            if(fighter_now.fighterType == FighterType.Enemy)
            {
                enemySelected = (Random.value > 0.5f) ? enemy_1.gameObject : enemy_2.gameObject;
                playerSelected = (Random.value > 0.5) ? player_1.gameObject : player_2.gameObject;
                fighterSelected = (Random.value > 0.5f ? playerSelected : enemySelected);
            }
            if ((Input.GetKeyUp(KeyCode.Space) && _waitingForSelection) || (fighter_now.fighterType == FighterType.Enemy))
            {
                objectiveSelectedEvent.enemySelected = enemySelected;
                objectiveSelectedEvent.playerSelected = playerSelected;
                objectiveSelectedEvent.objectiveSelected = fighterSelected;

                Debug.Log("Objetivo/s seleccionados.");
                EventController.TriggerEvent(objectiveSelectedEvent);
                _waitingForSelection = false;
            }

            yield return null;
        }
    }

    private IEnumerator MoveSpotlight()
    {
        while (_battleRunning)
        { 
            Vector3 newPosition = new Vector3(fighter_now.transform.position.x, spotlight.transform.position.y, fighter_now.transform.position.z);
            spotlight.transform.position = newPosition;

            yield return null;
        }

    }

    private void GameRestarted(RestartGameEvent evento)
    {
        fighters_turn = 0;
        UnpauseBattle();
        this.NextTurn();
        StartCoroutine(ObjectiveSelected());
        StartCoroutine(MoveSpotlight());
        dead_enemies = 0;
        dead_players = 0;
    }

    public void PauseBattle()
    {
        _battleRunning = false;
    }

    public void UnpauseBattle()
    {
        _battleRunning = true;
        StartCoroutine(ObjectiveSelected());
        StartCoroutine(MoveSpotlight());
    }

    /*
    private IEnumerator MoveTargetSprite()
    {
        while (_battleRunning && _waitingForSelection)
        {
            if (fighter_now.fighterType == FighterType.Player)
            {
                targetSprite.GetComponent<SpriteRenderer>().enabled = (targetSprite.GetComponent<SpriteRenderer>().enabled ? false : true);
                //calcular posicion de 3D a pantalla.
            }

            yield return new WaitForSeconds(0.5f);
        }
    }*/
}
