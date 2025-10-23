using BattleRoyale.MainModule;
using BattleRoyale.NetworkModule;
using BattleRoyale.XPModule;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.UIModule
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

            int myrank = sortedPlayers.Find(x => x.ClientId == NetworkManager.Singleton.LocalClientId).Rank;
            AddXpBasedOnRank(myrank);
        }

        private void AddXpBasedOnRank(int rank)
        {
            if(rank<=3)
            {
                GameManager.Instance.Get<XPService>().AddXPOnGameOver(rank);
            }
        }
    }
}
