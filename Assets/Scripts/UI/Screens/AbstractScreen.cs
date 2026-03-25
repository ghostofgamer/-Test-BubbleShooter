using UnityEngine;

namespace UI.Screens
{
    public class AbstractScreen : MonoBehaviour
    {
        private ScreenAnimation _animation;

        protected virtual void Awake()
        {
            _animation = GetComponent<ScreenAnimation>();
        }

        public virtual void OpenScreen()
        {
            if (_animation != null)
                _animation.PlayOpen();
            else
                gameObject.SetActive(true);
        }

        public virtual void CloseScreen()
        {
            if (_animation != null)
                _animation.PlayClose();
            else
                gameObject.SetActive(false);
        }
    }
}