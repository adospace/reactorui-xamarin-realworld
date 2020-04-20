using System;
using System.Collections.Generic;
using System.Text;
using XamarinReactorUI;

namespace RxRealWorld.Components
{
    public class ShellComponentState: IState
    {
    }

    public class ShellComponent : RxComponent<ShellComponentState>
    {
        public override VisualNode Render()
        {
            return new RxShell()
            {
                new HomeComponent()
            };
        }
    }

}
