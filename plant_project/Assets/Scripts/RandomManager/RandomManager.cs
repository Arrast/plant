
using System;

public class RandomManager
{
    private Random _random;
    private int _hashCode;

    public RandomManager()
    {
        _hashCode = Guid.NewGuid().GetHashCode();
        UnityEngine.Debug.Log($"[RandomManager] Seed is {_hashCode}");
        _random = new Random(_hashCode);
    }

    public int GetRandom(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}
