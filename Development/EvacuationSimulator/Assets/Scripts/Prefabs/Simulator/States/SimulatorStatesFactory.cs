using Assets.Scripts.Prefabs.Simulator.States.Context;

namespace Assets.Scripts.Prefabs.Simulator.States
{
    public enum SimulatorStateType
    {
        START,
        GENERATE_NAVIGATION_GRID,
        GENERATE_FIRE,
        GENERATE_BOTS,
        START_ROUND,
        POSITION_BOTS,
        START_FIRE,
        START_ALARM,
        OPEN_DOORS,
        OBSERVE_BOTS,
        DISPLAY_STATS,
        STOP_FIRE,
        STOP_ALARM,
        CLOSE_DOORS,
        ROUND_CHECK,
        END
    }

    class SimulatorStatesFactory
    {
        public static SimulatorState MakeSimulatorState(SimulatorStateType stateType, SimulatorContext context)
        {
            SimulatorState state = null;

            switch(stateType)
            {
                case SimulatorStateType.START:
                    state = new Start(context);
                    break;
                case SimulatorStateType.GENERATE_NAVIGATION_GRID:
                    state = new GenerateNavigationGrid(context);
                    break;
                case SimulatorStateType.GENERATE_FIRE:
                    state = new GenerateFire(context);
                    break;
                case SimulatorStateType.GENERATE_BOTS:
                    state = new GenerateBots(context);
                    break;
                case SimulatorStateType.START_ROUND:
                    state = new StartRound(context);
                    break;
                case SimulatorStateType.POSITION_BOTS:
                    state = new PositionBots(context);
                    break;
                case SimulatorStateType.START_FIRE:
                    state = new StartFire(context);
                    break;
                case SimulatorStateType.START_ALARM:
                    state = new StartAlarm(context);
                    break;
                case SimulatorStateType.OPEN_DOORS:
                    state = new OpenDoors(context);
                    break;
                case SimulatorStateType.OBSERVE_BOTS:
                    state = new ObserveBots(context);
                    break;
                case SimulatorStateType.DISPLAY_STATS:
                    state = new DisplayStats(context);
                    break;
                case SimulatorStateType.STOP_FIRE:
                    state = new StopFire(context);
                    break;
                case SimulatorStateType.STOP_ALARM:
                    state = new StopAlarm(context);
                    break;
                case SimulatorStateType.CLOSE_DOORS:
                    state = new CloseDoors(context);
                    break;
                case SimulatorStateType.ROUND_CHECK:
                    state = new RoundCheck(context);
                    break;
                case SimulatorStateType.END:
                    state = new End(context);
                    break;
            }

            return state;
        }
    }
}
