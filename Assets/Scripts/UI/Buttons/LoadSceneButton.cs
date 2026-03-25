using UI.Screens;
using UnityEngine;

namespace UI.Buttons
{
    public class LoadSceneButton : AbstractButton
    {
        [SerializeField] SceneType _sceneType;
        
        public override void OnClick()
        {
           LoadingScreen.Instance.LoadScene(_sceneType.ToString());
        }
    }
}