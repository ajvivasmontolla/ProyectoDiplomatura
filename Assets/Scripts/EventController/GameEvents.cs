using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent  { }

    public class GameStartEvent : GameEvent { }
    public class BattleLoseEvent : GameEvent { }
    public class BattleWinEvent : GameEvent { }
    public class EscapeBattleEvent : GameEvent { }
    public class EndTurnEvent : GameEvent { }
    public class RestartGameEvent : GameEvent { }

    #region Eventos de Accion
    public class ActionSelectedEvent : GameEvent { }
    public class AttackSelectedEvent: GameEvent { }
    public class SpAttackSelectedEvent : GameEvent { }
    public class ExAttackSelectedEvent : GameEvent { }
    public class GuardSelectedEvent : GameEvent { }
    public class ItemSelectedEvent : GameEvent { }
    public class RunAwayEvent : GameEvent { }
    #endregion

    public class FighterDead : GameEvent
    {
        public FighterType fighterType = FighterType.None;
    }
    public class ObjectiveSelectedEvent : GameEvent
    {
        public GameObject objectiveSelected;
        public GameObject playerSelected;
        public GameObject enemySelected;
    }
}


