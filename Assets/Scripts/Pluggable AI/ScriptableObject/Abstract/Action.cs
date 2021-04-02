using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController stateController);
    }
}