using System.Collections.Generic;
using UnityEngine;

public interface ICardShuffler
{
    List<(int pairId, Sprite sprite)> GenerateShuffledCards(
        int rows,
        int cols,
        Sprite[] availableSprites,
        float adjacentPairRatio
    );
}
