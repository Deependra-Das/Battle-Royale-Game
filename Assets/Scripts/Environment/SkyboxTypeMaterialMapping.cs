using UnityEngine;

namespace BattleRoyale.EnvironmentModule
{
    [System.Serializable]
    public class SkyboxTypeMaterialMapping
    {
        public SkyboxType skyboxType;
        public Material skyboxMaterial;
    }

    public enum SkyboxType
    {
        Day,
        Night,
    }

}