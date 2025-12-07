using System.Numerics;

public interface IGUID
{
    public abstract string GetGUID { get; }
    public abstract void EvaluateGUID(Vector2 objWorldPos);
}
