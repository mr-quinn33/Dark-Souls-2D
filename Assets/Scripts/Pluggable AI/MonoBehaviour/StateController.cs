using Assets.Scripts.CharacterControllers;
using Assets.Scripts.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.PluggableAI
{
    [RequireComponent(typeof(NavMeshAgent), typeof(CharacterController2D))]
    public sealed class StateController : MonoBehaviour
    {
        #region Fields
        public State currentState;
        public State deadState;
        public Vision vision;
        [HideInInspector] public new Transform transform;
        [HideInInspector] public Transform target;

        private float timeElapsed = default;
        #endregion

        #region Properties
        public NavMeshAgent NavMeshAgent { get; private set; }

        public Character Character { get; private set; }

        public AttackController AttackController { get; private set; }
        #endregion

        #region Callbacks
        private void Awake()
        {
            transform = GetComponent<Transform>();
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Character = GetComponent<CharacterController2D>().character;
            AttackController = GetComponentInChildren<AttackController>();
        }

        private void Update() => currentState.UpdateState(this);

        private void OnDrawGizmos()
        {
            if (currentState)
            {
                Gizmos.color = currentState.sceneGizmoColor;
                Gizmos.DrawWireSphere((transform == null ? GetComponent<Transform>() : transform).position, vision.radius);
            }
        }
        #endregion

        #region Methods
        public void TransitionToState(State nextState)
        {
            if (nextState)
            {
                currentState = nextState;
                timeElapsed = default;
            }
        }

        public bool IsTimeElapsed(float duration)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > duration)
            {
                timeElapsed = default;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}