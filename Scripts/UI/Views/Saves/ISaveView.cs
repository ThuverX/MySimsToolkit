using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.UI.Views.Saves;

public interface ISaveView
{
    void Draw(double dt);
    void Load(SaveService.SaveFileData save);
    void Save(SaveService.SaveFileData save);
}