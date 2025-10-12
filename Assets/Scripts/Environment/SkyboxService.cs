using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.EnvironmentModule
{
    public class SkyboxService
    {
        private List<SkyboxTypeMaterialMapping> _skyboxMaterials;

        public SkyboxService(List<SkyboxTypeMaterialMapping> materials)
        {
            if (materials == null || materials.Count == 0)
            {
                Debug.LogError("Skybox materials list is empty or null. Cannot initialize SkyboxService.");
                return;
            }

            _skyboxMaterials = new List<SkyboxTypeMaterialMapping>(materials.Count);
            foreach (var material in materials)
            {
                _skyboxMaterials.Add(new SkyboxTypeMaterialMapping
                {
                    skyboxType = material.skyboxType,
                    skyboxMaterial = material.skyboxMaterial
                });
            }
        }

        public void ApplyRandomSkyboxMaterial()
        {
            if (_skyboxMaterials == null || _skyboxMaterials.Count == 0)
            {
                Debug.LogWarning("No skybox materials available to apply.");
                return;
            }

            int randomIndex = Random.Range(0, _skyboxMaterials.Count);
            SkyboxType randomSkybox = _skyboxMaterials[randomIndex].skyboxType;

            ApplySkyboxMaterialByType(randomSkybox);
        }

        public void ApplySkyboxMaterialByType(SkyboxType skyboxType)
        {
            if (_skyboxMaterials == null || _skyboxMaterials.Count == 0)
            {
                Debug.LogWarning("No skybox materials available to apply.");
                return;
            }

            SkyboxTypeMaterialMapping material = _skyboxMaterials.Find(m => m.skyboxType == skyboxType);

            if (material != null)
            {
                RenderSettings.skybox = material.skyboxMaterial;
            }
            else
            {
                Debug.LogWarning($"Skybox material not found for the type: {skyboxType}");
            }
        }

        public void Dispose()
        {
            _skyboxMaterials.Clear();
        }
    }
}