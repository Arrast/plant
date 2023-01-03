
using System.Threading.Tasks;
using versoft.scene_manager;

public class ShopWindowController : WindowController
{
    public override async Task Init(WindowConfig windowConfig)
    {
        await base.Init(windowConfig);
    }

    public override void WindowOpened()
    {
        UnityEngine.Debug.LogError("Window Opened");
    }
}
