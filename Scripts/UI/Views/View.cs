using Godot;

namespace MySimsToolkit.Scripts.UI.Views;

public abstract partial class View : Node
{
    private UiRoot _root;
    public UiRoot Root => _root;

    public void SetUiRoot(UiRoot root)
    {
        _root = root;
    }
    
    public abstract void Draw(double dt);
}