using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Algorithm https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
public static class PoissonDiskSampling
{
    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 25) {
        // Step 0
        float cellSize = radius/Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x/cellSize), Mathf.CeilToInt(sampleRegionSize.y/cellSize)];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        // Step 1
        spawnPoints.Add(sampleRegionSize/2);

        // Step 2
        while (spawnPoints.Count > 0) 
        {
            // Choose a random spawn point from the list
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                // Generate random point around spawnCentre
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Mathf.Sqrt(Random.Range(radius*radius, 4*radius*radius));
                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) 
                {
                    // Add point to list
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x/cellSize), (int)(candidate.y/cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                // Remove spawn point from list
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) 
    {
        // Discard candidates that are outside of the sample region
        if (candidate.x < 0 || candidate.x >= sampleRegionSize.x || candidate.y < 0 || candidate.y >= sampleRegionSize.y)
            return false;
        
        int cellX = (int)(candidate.x/cellSize);
        int cellY = (int)(candidate.y/cellSize);
        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

        for (int x = searchStartX; x <= searchEndX; x++)
        {
            for (int y = searchStartY; y <= searchEndY; y++)
            {
                int pointIndex = grid[x, y] - 1;
                if (pointIndex != -1)
                {
                    float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                    if (sqrDst < radius*radius)
                    {
                        // Discard candidate if it is too close to another point
                        return false;
                    }
                }
            }
        }

        // Candidate is valid
        return true;
    }
}
