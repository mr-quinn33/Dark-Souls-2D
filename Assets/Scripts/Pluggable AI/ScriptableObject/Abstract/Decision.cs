using UnityEngine;

namespace Assets.Scripts.PluggableAI
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController stateController);
    }
}