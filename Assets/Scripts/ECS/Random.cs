namespace ECS
{
    public class Random
    {
        public Random(int seed) => m_Random = new System.Random(seed);
        public Random() => m_Random = new System.Random();

        System.Random m_Random;

        public float Value => (float)m_Random.NextDouble();
        public int Int(int threshold) => m_Random.Next(-threshold, threshold);
        public int Int(int min, int max) => m_Random.Next(min, max);
        public int Int(uint min, uint max) => m_Random.Next((int)min, (int)max);
        public float Float(float threshold) => 2f * (float)m_Random.NextDouble() * threshold - threshold;
        public float Float(float min, float max) => (float)m_Random.NextDouble() * (max - min) + min;
        public bool Bool() => m_Random.Next(100) % 2 == 0;
    }
}
