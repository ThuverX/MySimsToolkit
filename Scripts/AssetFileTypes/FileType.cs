using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public abstract class FileType
{
    public abstract string Name { get; }
    public abstract string[] Extensions { get; }
    public abstract string Icon { get; }
    public virtual string Description  { get; }

    protected bool Equals(FileType other)
    {
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FileType)obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }
}