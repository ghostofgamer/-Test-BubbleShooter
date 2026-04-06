using BallContent;
using ScoreContent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class EndGameScreen : AbstractScreen
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _currentScoreText;
        [SerializeField] private TMP_Text _recordScoreText;

        private BallLauncher _ballLauncher;
        
        public void Init(BallLauncher ballLauncher)
        {
            _ballLauncher = ballLauncher;
            _ballLauncher.AllBallUsed += ShowDefeatScreen;
        }
        
        public override void OpenScreen()
        {
            base.OpenScreen();

            if (ScoreManager.Instance.TrySetNewRecord())
            {
                _currentScoreText.text = $"New Record!!! {ScoreManager.Instance.CurrentScore.ToString()}";
                _recordScoreText.gameObject.SetActive(false);
            }
            else
            {
                _currentScoreText.text = $"Score: {ScoreManager.Instance.CurrentScore.ToString()}";
                _recordScoreText.text = $"Record: {ScoreManager.Instance.CurrentScore.ToString()}";
            }
        }

        private void ShowVictoryScreen()
        {
            _label.text = "Victory!";

            OpenScreen();
        }

        private void ShowDefeatScreen()
        {
            _label.text = "Defeat!";
            OpenScreen();
        }
        
        private void OnDestroy()
        {
            if (_ballLauncher != null)
                _ballLauncher.AllBallUsed -= ShowDefeatScreen;
        }
    }
}