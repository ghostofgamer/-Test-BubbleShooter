using ScoreContent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class EndGameScreen : AbstractScreen
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField]private Image _backgroundImage; 
        [SerializeField] private TMP_Text _currentScoreText;
        [SerializeField] private TMP_Text _recordScoreText;

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

        public void SetLabelText(string labelText)
        {
            _label.text = labelText;
        }
    }
}