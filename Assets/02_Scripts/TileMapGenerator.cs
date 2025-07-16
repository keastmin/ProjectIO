using UnityEngine;

public class TileMapGenerator
{
    [System.Serializable]
    public class GenerationSettings
    {
        public int tileTypeCount = 2;
        public float grassProbability = 0.7f;
        public bool usePerlinNoise = false;
        public float noiseScale = 0.1f;
    }

    public TileMapGenerationData Generate(Vector2Int size, GenerationSettings settings = null)
    {
        if (settings == null)
            settings = new GenerationSettings();

        int[] tileMap = new int[size.x * size.y];
        
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int index = y * size.x + x;
                
                if (settings.usePerlinNoise)
                {
                    // 펄린 노이즈를 사용한 자연스러운 지형 생성
                    float noiseValue = Mathf.PerlinNoise(x * settings.noiseScale, y * settings.noiseScale);
                    tileMap[index] = noiseValue > 0.5f ? 0 : 1;
                }
                else
                {
                    // 확률 기반 랜덤 생성
                    float randomValue = Random.Range(0f, 1f);
                    tileMap[index] = randomValue < settings.grassProbability ? 0 : 1;
                }
            }
        }
        
        return new TileMapGenerationData(size, tileMap);
    }

    // 기존 호환성을 위한 메서드
    public TileMapGenerationData Generate(Vector2Int size)
    {
        return Generate(size, null);
    }
}