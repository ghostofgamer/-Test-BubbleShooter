using TMPro;
using UnityEngine;

namespace ScoreContent
{
    public class ScoreViewer : MonoBehaviour
    {
        private const string RecordScore = "RecordScore";

        [SerializeField] private TMP_Text _scoreCurrentText;

        private void OnEnable()
        {
            ScoreManager.Instance.ScoreChanged += UpdateUI;
        }

        private void OnDisable()
        {
            ScoreManager.Instance.ScoreChanged -= UpdateUI;
        }

        private void Start()
        {
            UpdateUI(0);
        }

        private void UpdateUI(int value)
        {
            _scoreCurrentText.text = $"Score : {value.ToString()}";
        }
    }
}