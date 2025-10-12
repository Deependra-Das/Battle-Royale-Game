using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.EnvironmentModule
{
    [CreateAssetMenu(fileName = "EnvironmentScriptableObject", menuName = "ScriptableObjects/EnvironmentScriptableObject")]
    public class EnvironmentScriptableObject : ScriptableObject
    {
        public List<SkyboxTypeMaterialMapping> skyboxTypeMaterialMappings;
    }
}