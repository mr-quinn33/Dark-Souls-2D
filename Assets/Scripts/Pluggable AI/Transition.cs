using System;

namespace Assets.Scripts.PluggableAI
{
    [Serializable]
    public sealed class Transition
    {
        public Decision decision;
        public State trueState;
        public State falseState;
    }
}