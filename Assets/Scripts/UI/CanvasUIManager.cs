using BattleRoyale.Utilities;
using UnityEngine;

namespace BattleRoyale.UI
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
