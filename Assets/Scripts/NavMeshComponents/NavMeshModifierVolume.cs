using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifierVolume", 31)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifierVolume : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_Size = new Vector3(4.0f, 3.0f, 4.0f);
        public Vector3 Size { get => m_Size; set => m_Size = value; }

        [SerializeField]
        private Vector3 m_Center = new Vector3(0, 1.0f, 0);
        public Vector3 Center { get => m_Center; set => m_Center = value; }

        [SerializeField]
        private int m_Area;
        public int Area { get => m_Area; set => m_Area = value; }

        // List of agent types the modifier is applied for.
        // Special values: empty == None, m_AffectedAgents[0] =-1 == All.
        [SerializeField]
        private List<int> m_AffectedAgents = new List<int>(new int[] { -1 });    // Default value is All

        public static List<NavMeshModifierVolume> ActiveModifiers { get; } = new List<NavMeshModifierVolume>();

        private void OnEnable()
        {
            if (!ActiveModifiers.Contains(this))
            {
                ActiveModifiers.Add(this);
            }
        }

        private void OnDisable() => _ = ActiveModifiers.Remove(this);

        public bool AffectsAgentType(int agentTypeID) => m_AffectedAgents.Count != 0 && (m_AffectedAgents[0] == -1 || m_AffectedAgents.IndexOf(agentTypeID) != -1);
    }
}
