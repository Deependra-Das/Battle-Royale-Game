using UnityEngine;

namespace BattleRoyale.Environment
{
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