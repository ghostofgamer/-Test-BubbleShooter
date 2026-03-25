using UI.Screens;
using UnityEngine;

namespace UI.Buttons
{
    public class OpenScreenButton : AbstractButton
    {
        [SerializeField] private AbstractScreen _openScreen;

        public override void OnClick()
        {
            _openScreen.OpenScreen();
        }
    }
}