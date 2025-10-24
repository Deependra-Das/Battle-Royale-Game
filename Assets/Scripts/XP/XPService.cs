using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BattleRoyale.XPModule
{
    public class XPService
    {
        private int _baseXP = 100;
        private int _currentTotalXP;
        private int _currentXPMileStone;
        private List<int> _xpMilestones;

        private const string CurrentTotalXPKey = "CurrentTotalXP";

        public XPService(List<int> xpMilestones)
        {
            if (xpMilestones.Count > 0)
            {
                this._xpMilestones = xpMilestones;
                _currentTotalXP = PlayerPrefs.GetInt(CurrentTotalXPKey, 0);
                UpdateXPMileStone();
            }
            else
            {
                Debug.Log("XP Milestones List is Empty");
            }
        }

        public void AddXPOnGameOver(int rank)
        {
            int xpToAdd = 0;

            switch (rank)
            {
                case 1:
                    xpToAdd = Mathf.RoundToInt(_baseXP * 1.0f);
                    break;
                case 2:
                    xpToAdd = Mathf.RoundToInt(_baseXP * 0.75f);
                    break;
                case 3:
                    xpToAdd = Mathf.RoundToInt(_baseXP * 0.5f);
                    break;
                default:
                    xpToAdd = 0;
                    break;
            }

            _currentTotalXP += xpToAdd;
            UpdateXPMileStone();
            PlayerPrefs.SetInt(CurrentTotalXPKey, _currentTotalXP);
            PlayerPrefs.Save();
        }

        private void UpdateXPMileStone()
        {
            _currentXPMileStone = 0;
            foreach (var threshold in _xpMilestones)
            {
                if (_currentTotalXP <= threshold)
                {
                    _currentXPMileStone = threshold;
                    break;
                }
            }
        }

        public void ResetXP()
        {
            _currentTotalXP = 0;
            PlayerPrefs.SetInt(CurrentTotalXPKey, _currentTotalXP);
            PlayerPrefs.Save();
            UpdateXPMileStone();
        }

        public int GetCurrentTotalXP()
        {
            return _currentTotalXP;
        }

        public int GetNextXPMileStone()
        {
            return _currentXPMileStone;
        }
    }
}
