using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifier", 32)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifier : MonoBehaviour
    {
        [SerializeField]
        private bool m_OverrideArea;
        public bool OverrideArea { get => m_OverrideArea; set => m_OverrideArea = value; }

        [SerializeField]
        private int m_Area;
        public int Area { get => m_Area; set => m_Area = value; }

        [SerializeField]
        private bool m_IgnoreFromBuild;
        public bool IgnoreFromBuild { get => m_IgnoreFromBuild; set => m_IgnoreFromBuild = value; }

        // List of agent types the modifier is applied for.
        // Special values: empty == None, m_AffectedAgents[0] =-1 == All.
        [SerializeField]
        private List<int> m_AffectedAgents = new List<int>(new int[] { -1 });    // Default value is All

        public static List<NavMeshModifier> ActiveModifiers { get; } = new List<NavMeshModifier>();

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
