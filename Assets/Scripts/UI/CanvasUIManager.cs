using BattleRoyale.UtilitiesModule;
using UnityEngine;

namespace BattleRoyale.UIModule
{
    public class CanvasUIManager : GenericMonoSingleton<CanvasUIManager>
    {
        public Transform canvasTransform;

        protected override void Awake()
        {
            base.Awake();
        }
    }
}
