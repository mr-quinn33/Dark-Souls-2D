using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    [CreateAssetMenu(fileName = "New State", menuName = "Scriptable Object/Pluggable AI/State")]
    public sealed class State : ScriptableObject
    {
        public Color sceneGizmoColor;
        public List<Action> actionList;
        public List<Transition> transitionList;

        public void UpdateState(StateController controller)
        {
            TakeActions(controller);
            CheckTransitions(controller);
        }

        private void TakeActions(StateController stateController)
        {
            foreach (Action action in actionList)
            {
                action.Act(stateController);
            }
        }

        private void CheckTransitions(StateController stateController)
        {
            if (!stateController.NavMeshAgent.enabled)
            {
                stateController.TransitionToState(stateController.deadState);
            }
            foreach (Transition transition in transitionList)
            {
                if (transition.decision.Decide(stateController))
                {
                    stateController.TransitionToState(transition.trueState);
                }
                else
                {
                    stateController.TransitionToState(transition.falseState);
                }
            }
        }
    }
}