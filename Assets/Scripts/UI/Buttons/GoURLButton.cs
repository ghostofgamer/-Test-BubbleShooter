using UnityEngine;

namespace UI.Buttons
{
    public class GoURLButton : AbstractButton
    {
        [SerializeField] private string _url;
    
        public override void OnClick()
        {
            Application.OpenURL(_url);
        }
    }
}
