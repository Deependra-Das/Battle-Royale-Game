using BattleRoyale.Network;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.UI
{
    public class GameOverUIService
    {
        private GameOverUIView _gameOverUIView;
        private Transform _scoreboardContentTransform;
        private ScoreboardEntryUIView _scoreboardEntryUIPrefab;

        public GameOverUIService(GameOverUIView GameOverUIPrefab, ScoreboardEntryUIView scoreboardEntryUIPrefab)
        {
            Transform canvasTransform = CanvasUIManager.Instance.canvasTransform;
            _gameOverUIView = Object.Instantiate(GameOverUIPrefab, canvasTransform);
            _scoreboardContentTransform = _gameOverUIView.GetScoreboardContentTransform();
            _scoreboardEntryUIPrefab = scoreboardEntryUIPrefab;

            PopulateScoreboard();
            HideUI();
        }

        public void ShowUI()
        {
            _gameOverUIView.EnableView();
        }

        public void HideUI()
        {
            _gameOverUIView.DisableView();
        }

        public void Dispose()
        {
            Object.Destroy(_gameOverUIView.gameObject);
            _gameOverUIView = null;
        }

        public void PopulateScoreboard()
        {
            var allPlayerData = PlayerSessionManager.Instance.GetAllPlayerSessionData();

            List<PlayerSessionData> sortedPlayers = new List<PlayerSessionData>(allPlayerData.Values);
            sortedPlayers.Sort((a, b) => a.Rank.CompareTo(b.Rank));

            foreach (var player in sortedPlayers)
            {
                ScoreboardEntryUIView entry = Object.Instantiate(_scoreboardEntryUIPrefab, _scoreboardContentTransform);

                entry.SetupEntry(
                    player.Rank,
                    player.Username.ToString()
                );
            }
        }
    }
}
