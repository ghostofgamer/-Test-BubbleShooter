using UI.Screens;
using UnityEngine;

namespace UI.Buttons
{
    public class CloseButton : AbstractButton
    {
        [SerializeField]private AbstractScreen _screen;
    
        public override void OnClick()
        { 
            _screen.CloseScreen();
        }
    }
}