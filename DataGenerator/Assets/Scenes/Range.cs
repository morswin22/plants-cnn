public class Range
{
    public float min;
    public float max;
    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
    public float Random()
    {
        return UnityEngine.Random.Range(min, max);
    }
}
