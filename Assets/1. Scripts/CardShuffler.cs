using System.Collections.Generic;
using UnityEngine;

public class CardShuffler : ICardShuffler
{
    private Dictionary<int, Sprite> pairIdToSprite;

    public CardShuffler()
    {
        pairIdToSprite = new Dictionary<int, Sprite>();
    }

    public List<(int pairId, Sprite sprite)> GenerateShuffledCards(
        int rows,
        int cols,
        Sprite[] allSprites,
        float adjacentPairRatio)
    {
        int totalCards = rows * cols;
        int pairCount = totalCards / 2;

        // 1. 이미지 무작위 매핑
        List<int> imageIndices = new();
        for (int i = 0; i < allSprites.Length; i++)
            imageIndices.Add(i);

        ShuffleRandom(imageIndices);

        pairIdToSprite.Clear();
        for (int i = 0; i < pairCount; i++)
        {
            pairIdToSprite[i] = allSprites[imageIndices[i]];
        }

        // 2. 쌍 목록 생성
        List<(int pairId, Sprite sprite)> cardInfos = new();
        for (int i = 0; i < pairCount; i++)
        {
            cardInfos.Add((i, null)); // sprite는 나중에 할당
            cardInfos.Add((i, null));
        }

        // 3.AdjacentPairRatio 기반으로 카드 위치 섞기 
        ShuffleWithAdjacentPairRatio(cardInfos, rows, cols, adjacentPairRatio);

        // 4. 이미지 매칭
        for (int i = 0; i < cardInfos.Count; i++)
        {
            int id = cardInfos[i].pairId;
            cardInfos[i] = (id, pairIdToSprite[id]);
        }

        return cardInfos;
    }

    private void ShuffleWithAdjacentPairRatio(List<(int pairId, Sprite sprite)> cards, int rows, int cols, float ratio)
    {
        int totalCards = rows * cols;
        int pairCount = totalCards / 2;
        int targetAdjacent = Mathf.RoundToInt(pairCount * ratio); //인접하게 배치할 카드 쌍 개수

        (int pairId, Sprite)?[] grid = new (int, Sprite)?[totalCards];
        List<int> available = new();
        for (int i = 0; i < totalCards; i++) available.Add(i);

        System.Random rand = new();
        int currentId = 0; // 현재 배치할 pairId
        int placed = 0; // 인접하게 배치된 카드 쌍 수

        // 인접 카드 배치 시도
        while (currentId < pairCount && available.Count >= 2)
        {
            int first = available[rand.Next(available.Count)];
            List<int> neighbors = GetNeighbors(first, rows, cols);
            ShuffleRandom(neighbors);

            bool success = false;
            foreach (int neighbor in neighbors)
            {
                if (available.Contains(neighbor))
                {
                    grid[first] = (currentId, null);
                    grid[neighbor] = (currentId, null);
                    available.Remove(first);
                    available.Remove(neighbor);
                    currentId++;
                    placed++;
                    success = true;
                    break;
                }
            }

            if (!success) break; // 더 이상 인접 배치 불가 시 종료
            if (placed >= targetAdjacent) break; // 목표치 달성 시 종료
        }

        //인접 배치 외에 나머지는 무작위 배치
        ShuffleRandom(available);
        while (currentId < pairCount && available.Count >= 2)
        {
            int a = available[0];
            int b = available[1];
            grid[a] = (currentId, null);
            grid[b] = (currentId, null);
            available.RemoveRange(0, 2);
            currentId++;
        }

        cards.Clear();
        foreach (var item in grid)
        {
            if (item.HasValue)
                cards.Add((item.Value.pairId, null));
        }
    }

    private List<int> GetNeighbors(int index, int rows, int cols)
    {
        List<int> neighbors = new();
        int r = index / cols; // 현재 행 계산
        int c = index % cols; // 현재 열 계산

        // 상하좌우 인접한 위치가 유효하면 neighbors에 추가
        if (r > 0) neighbors.Add((r - 1) * cols + c);
        if (r < rows - 1) neighbors.Add((r + 1) * cols + c);
        if (c > 0) neighbors.Add(r * cols + (c - 1));
        if (c < cols - 1) neighbors.Add(r * cols + (c + 1));

        return neighbors;
    }

    private void ShuffleRandom<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
