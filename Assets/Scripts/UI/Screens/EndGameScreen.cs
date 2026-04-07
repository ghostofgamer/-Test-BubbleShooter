using BallContent;
using ScoreContent;
using TMPro;
using TMPro.EditorUtilities;
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
        private GameManager _gameManager;

        public void Init(BallLauncher ballLauncher)
        {
            _ballLauncher = ballLauncher;
            _gameManager = GameManager.Instance;
            _ballLauncher.AllBallUsed += ShowDefeatScreen;
            _gameManager.VictoryGame += ShowVictoryScreen;
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
                _recordScoreText.text = $"Record: {ScoreManager.Instance.RecordScore.ToString()}";
            }
        }

        private void ShowVictoryScreen()
        {
            _label.text = "Victory!";
            _backgroundImage.color = Color.green;
            OpenScreen();
        }

        private void ShowDefeatScreen()
        {
            _label.text = "Defeat!";
            _backgroundImage.color = Color.red;
            OpenScreen();
        }

        private void OnDestroy()
        {
            if (_ballLauncher != null)
                _ballLauncher.AllBallUsed -= ShowDefeatScreen;

            if (_gameManager != null)
                _gameManager.VictoryGame += ShowVictoryScreen;
        }
    }
}