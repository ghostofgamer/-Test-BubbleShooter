using System;
using UnityEngine;

namespace ScoreContent
{
    public class ScoreManager : MonoBehaviour
    {
        private const string RecordScoreName = "RecordScore";

        public static ScoreManager Instance { get; private set; }

        public int RecordScore;
        public int CurrentScore;

        public event Action<int> ScoreChanged;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            RecordScore = PlayerPrefs.GetInt(RecordScoreName, 0);
        }

        public void AddScore(int points)
        {
            CurrentScore += points;
            UpdateScore();
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            UpdateScore();
        }

        public void UpdateRecord()
        {
            if (CurrentScore > RecordScore)
                PlayerPrefs.SetInt(RecordScoreName, CurrentScore);
        }
        
        public bool TrySetNewRecord()
        {
            if (CurrentScore > RecordScore)
            {
                RecordScore = CurrentScore;
                PlayerPrefs.SetInt(RecordScoreName, RecordScore);
                return true;
            }
            return false;
        }

        private void UpdateScore()
        {
            ScoreChanged?.Invoke(CurrentScore);
        }
    }
}