using DG.Tweening;
using Enums;
using UnityEngine;

public class ScreenAnimation : MonoBehaviour
{
// @formatter:off        
        [Header("References")]
        [SerializeField] private RectTransform _rect;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [Header("Configurations")]
        [SerializeField] private AnimationType _openType;
        [SerializeField] private AnimationType _closeType;
        [SerializeField] private Ease _ease = Ease.OutBack;
        [SerializeField] private float _duration = 0.5f;
// @formatter:on

        public void PlayOpen()
        {
            switch (_openType)
            {
                case AnimationType.SlideFromBottom:
                    SlideFromBottom();
                    break;

                case AnimationType.SlideFromTop:
                    SlideFromTop();
                    break;

                case AnimationType.ScaleIn:
                    ScaleIn();
                    break;

                case AnimationType.FadeIn:
                    FadeIn();
                    break;
            }
        }

        public void PlayClose()
        {
            switch (_closeType)
            {
                case AnimationType.SlideFromBottom:
                    SlideToBottom();
                    break;

                case AnimationType.SlideFromTop:
                    SlideToTop();
                    break;

                case AnimationType.ScaleIn:
                    ScaleOut();
                    break;

                case AnimationType.FadeIn:
                    FadeOut();
                    break;
            }
        }

        private void SlideFromBottom()
        {
            SetCanvasValue(true);
            _rect.localScale = Vector3.one;
            FadeIn();
            _rect.anchoredPosition = new Vector2(0, -Screen.height);
            _rect.DOAnchorPos(Vector2.zero, _duration).SetEase(_ease);
        }

        private void SlideFromTop()
        {
            SetCanvasValue(true);
            _rect.localScale = Vector3.one;
            FadeIn();
            _rect.anchoredPosition = new Vector2(0, Screen.height);
            _rect.DOAnchorPos(Vector2.zero, _duration).SetEase(_ease);
        }

        private void SlideToBottom()
        {
            FadeOut();
            _rect.DOAnchorPos(new Vector2(0, -Screen.height), _duration).SetEase(_ease)
                .OnComplete(() => SetCanvasValue(false));
        }

        private void SlideToTop()
        {
            FadeOut();
            _rect.DOAnchorPos(new Vector2(0, Screen.height), _duration).SetEase(_ease)
                .OnComplete(() => SetCanvasValue(false));
        }

        private void ScaleIn()
        {
            _rect.anchoredPosition = new Vector2(0, 0);
            FadeIn();
            SetCanvasValue(true);
            _rect.localScale = Vector3.zero;
            _rect.DOScale(1f, _duration).SetEase(_ease);
        }

        private void ScaleOut()
        {
            _rect.anchoredPosition = new Vector2(0, 0);
            FadeOut();
            _rect.DOScale(0f, _duration).SetEase(_ease).OnComplete(() => SetCanvasValue(false));
        }

        private void FadeIn()
        {
            _rect.localScale = Vector3.one;
            SetCanvasValue(true);
            _rect.anchoredPosition = new Vector2(0, 0);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _duration);
        }

        private void FadeOut()
        {
            _rect.anchoredPosition = new Vector2(0, 0);
            _canvasGroup.DOFade(0f, _duration).OnComplete(() => SetCanvasValue(false));
        }

        private void SetCanvasValue(bool value)
        {
            _canvasGroup.blocksRaycasts = value;
            _canvasGroup.interactable = value;
        }
}