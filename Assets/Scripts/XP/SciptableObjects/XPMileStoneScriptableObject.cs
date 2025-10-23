using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.XPModule
{
    [CreateAssetMenu(fileName = "XPMileStoneScriptableObject", menuName = "ScriptableObjects/XPMileStoneScriptableObject")]
    public class XPMileStoneScriptableObject : ScriptableObject
    {
        public List<int> xpMilestones = new List<int>();
    }
}
