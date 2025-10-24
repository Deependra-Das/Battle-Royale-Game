using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale.XPModule
{
    public class XPService
    {
        private int _xpToAdd = 100;
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
            _currentTotalXP += Mathf.FloorToInt(_xpToAdd/rank);

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
