using UnityEngine;

namespace UI.Buttons
{
    public class ExitApplicationButton : AbstractButton
    {
        public override void OnClick() => Application.Quit();
    }
}