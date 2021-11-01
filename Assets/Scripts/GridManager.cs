using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap Tilemap;
    public RuleTile BaseTile;
    public GameObject SaveMarker;
    public float ObstaclePerlinScale;
    public float ObstaclePerlinCutoff;
    public GameObject GemPrefab;

    private int highestRenderedBlock;
    private int lastObstacleSpawnXPos;
    private System.Random random;
    private Vector2Int renderedRanges;
    private List<GameObject> SaveMarkers;
    private static readonly Color[] CasualColors =
    {
        ColorExtensions.Create("#53d52e"),
        ColorExtensions.Create("#2ed549"),
        ColorExtensions.Create("#2ed589"),
        ColorExtensions.Create("#2ed5cb"),
        ColorExtensions.Create("#35a5ff"),
        ColorExtensions.Create("#4281ff"),
        ColorExtensions.Create("#4d4aff"),
        ColorExtensions.Create("#5839ff"),
        ColorExtensions.Create("#974cd9")
    };

    private static readonly Color[] IntenseColors =
    {
        ColorExtensions.Create("#e23f44"),
        ColorExtensions.Create("#e95e32"),
        ColorExtensions.Create("#ef8c2a"),
        ColorExtensions.Create("#efb824"),
        ColorExtensions.Create("#f6e31f"),
        ColorExtensions.Create("#e23fde"),
        ColorExtensions.Create("#e23faf"),
        ColorExtensions.Create("#e23f7a"),
    };

    private static Color GetColor(int index)
    {
        switch (GameState.Player.SelectedDifficulty)
        {
            case (DifficultySetting.Casual):
                return CasualColors[index % CasualColors.Length];
            case (DifficultySetting.Intense):
                return IntenseColors[index % IntenseColors.Length];
            default:
                throw new System.Exception("Un-switched difficulty " + GameState.Player.SelectedDifficulty);
        }
    }

    void Start()
    {
        SaveMarkers = new List<GameObject>();
        random = new System.Random(0);
        InitiallySetupTiles();
    }

    void Update()
    {
        while (highestRenderedBlock < GetHelicopterBlockPos() + Constants.NUM_COLUMNS_RENDERED / 2)
        {
            highestRenderedBlock += 1;
            SpawnTilesForColumn(highestRenderedBlock);
            // SpawnGemIfApplicable(highestRenderedBlock);
            SpawnSaveMarkerIfApplicable(highestRenderedBlock - 1);
            SpawnObstacle(highestRenderedBlock);

            if (highestRenderedBlock / Constants.DISTANCE_BETWEEN_SAVES > renderedRanges.y)
            {
                renderedRanges.y = highestRenderedBlock / Constants.DISTANCE_BETWEEN_SAVES;
            }
        }
    }

    private int GetHelicopterBlockPos()
    {
        // return (int)(Managers.Helicopter.transform.position.x / Constants.BLOCK_WIDTH);
        return (int)Camera.main.transform.position.x;
    }

    public void ResetGrid()
    {
        foreach (GameObject marker in SaveMarkers)
        {
            Destroy(marker);
        }

        InitiallySetupTiles();
    }

    private void InitiallySetupTiles()
    {
        for (int i = GetHelicopterBlockPos() - Constants.NUM_COLUMNS_RENDERED / 2; i < GetHelicopterBlockPos() + Constants.NUM_COLUMNS_RENDERED / 2; i++)
        {
            SpawnTilesForColumn(i);
        }

        this.highestRenderedBlock = GetHelicopterBlockPos() - Constants.NUM_COLUMNS_RENDERED / 2;
    }

    private void SpawnTilesForColumn(int x)
    {
        Color color = GetColorForColumn(x);
        int centerHeight = GetCaveMidAtPos(x);
        for (int y = Constants.BOTTOM_HEIGHT - 1; y <= Constants.TOP_HEIGHT + 1; y++)
        {
            // clear current and far back column.
            Tilemap.SetTile(new Vector3Int(x, y, 0), null);
            Tilemap.SetTile(new Vector3Int(x - Constants.DISTANCE_BETWEEN_SAVES - 100, y, 0), null);

            if (Mathf.Abs(centerHeight - y) <= Constants.CAVE_RADIUS / 2)
            {
                continue;
            }

            float perlinValue = 0;
            if (y > 0)
            {
                perlinValue = Mathf.PerlinNoise(x * ObstaclePerlinScale, y * ObstaclePerlinScale);
            }
            else
            {
                perlinValue = Mathf.PerlinNoise((x + 50) * ObstaclePerlinScale, (y + 50) * ObstaclePerlinScale);
            }

            float distanceToMid = Mathf.Abs(y - centerHeight);
            perlinValue += distanceToMid * .08f;
            if (perlinValue < ObstaclePerlinCutoff)
            {
                continue;
            }

            SetTile(x, y, color);
        }

        SetTile(x, Constants.BOTTOM_HEIGHT, color);
        SetTile(x, Constants.BOTTOM_HEIGHT + 1, color);
        SetTile(x, Constants.TOP_HEIGHT, color);
        SetTile(x, Constants.TOP_HEIGHT - 1, color);
    }

    private static int GetDistanceBetweenObstacles()
    {
        switch (GameState.Player.SelectedDifficulty)
        {
            case (DifficultySetting.Casual):
                return 25;
            case (DifficultySetting.Intense):
                return 20;
            default:
                throw new System.Exception("Unknown difficulty " + GameState.Player.SelectedDifficulty);
        }
    }

    private static int GetObstacleHeight()
    {
        switch (GameState.Player.SelectedDifficulty)
        {
            case (DifficultySetting.Casual):
                return 2;
            case (DifficultySetting.Intense):
                return 4;
            default:
                throw new System.Exception("Unknown difficulty " + GameState.Player.SelectedDifficulty);
        }
    }

    private static int CenterGapObstacleHalfHeight()
    {
        switch (GameState.Player.SelectedDifficulty)
        {
            case (DifficultySetting.Casual):
                return 4;
            case (DifficultySetting.Intense):
                return 2;
            default:
                throw new System.Exception("Unknown difficulty " + GameState.Player.SelectedDifficulty);
        }
    }


    private void SpawnObstacle(int x)
    {
        if (x % GetDistanceBetweenObstacles() != 0 || x <= 0)
        {
            return;
        }

        // Don't spawn on top of save point.
        if (x % Constants.DISTANCE_BETWEEN_SAVES == 0)
        {
            return;
        }

        int obstacleType = Random.Range(0, 3);
        switch (obstacleType)
        {
            case (0):
                SpawnVerticalBarObstacle(x);
                break;
            case (1):
                SpawnHorizontalSlabs(x);
                break;
            case (2):
                SpawnCenterGapObstacle(x);
                break;
            default:
                SpawnVerticalBarObstacle(x);
                break;
        }

        SpawnGemsForObstacle(x);
    }

    private const int DIST_BETWEEN_GEMS = 3;
    private void SpawnGemsForObstacle(int obstacleXPos)
    {
        if (obstacleXPos % 3 != 0)
        {
            return;
        }

        int gemCount = Random.Range(0, 2) == 0 ? 3 : 5;
        Vector3[] gemPositions = GetGemPositions(obstacleXPos, gemCount);
        foreach (Vector3 pos in gemPositions)
        {
            GameObject gem = Instantiate(GemPrefab, pos, new Quaternion());
            if (pos == gemPositions[gemPositions.Length / 2])
            {
                gem.GetComponent<Gem>().SetTier(random.Next(0, 5) == 0 ? Gem.GemTier.High : Gem.GemTier.Mid);
            }
            else
            {
                gem.GetComponent<Gem>().SetTier(Gem.GemTier.Low);
            }
        }

        float caveSlope = GetCaveMidAtPos(obstacleXPos) - GetCaveMidAtPos(obstacleXPos - 10);
    }

    private Vector3[] GetGemPositions(int xPos, int numGems)
    {
        Vector2Int largestGap = findLargestGap(xPos);
        float midPoint = (float)largestGap.x + (largestGap.y - largestGap.x) / 2f;
        int gemEdgeDelta = (numGems / 2) * DIST_BETWEEN_GEMS;
        float farLeftYPos = GetCaveMidAtPos(xPos - DIST_BETWEEN_GEMS * 2) + Constants.BLOCK_WIDTH / 2f;
        float farRightYPos = GetCaveMidAtPos(xPos + DIST_BETWEEN_GEMS * 2) + Constants.BLOCK_WIDTH / 2f;

        if (numGems == 1)
        {
            return new Vector3[] {
                new Vector3(xPos, midPoint, 0),
            };
        }
        else if (numGems == 3)
        {
            return new Vector3[] {
                new Vector3(xPos - DIST_BETWEEN_GEMS, farLeftYPos + (midPoint - farLeftYPos) / 2, 0),
                new Vector3(xPos, midPoint, 0),
                new Vector3(xPos + DIST_BETWEEN_GEMS, farRightYPos + (midPoint - farRightYPos) / 2, 0),
            };
        }
        else if (numGems == 5)
        {
            return new Vector3[] {
                new Vector3(xPos - DIST_BETWEEN_GEMS * 2, farLeftYPos, 0),
                new Vector3(xPos - DIST_BETWEEN_GEMS, farLeftYPos + (midPoint - farLeftYPos) / 2, 0),
                new Vector3(xPos, midPoint, 0),
                new Vector3(xPos + DIST_BETWEEN_GEMS, farRightYPos + (midPoint - farRightYPos) / 2, 0),
                new Vector3(xPos + DIST_BETWEEN_GEMS * 2, farRightYPos, 0),
            };
        }
        else
        {
            throw new System.Exception($"Spawning {numGems} gems is not supported");
        }
    }

    private Vector2Int findLargestGap(int x)
    {
        Vector2Int largestGapBounds = new Vector2Int(Constants.BOTTOM_HEIGHT, Constants.BOTTOM_HEIGHT);
        Vector2Int currentGapBounds = new Vector2Int(Constants.BOTTOM_HEIGHT, Constants.BOTTOM_HEIGHT);
        for (int y = Constants.BOTTOM_HEIGHT; y < Constants.TOP_HEIGHT; y++)
        {
            if (Tilemap.HasTile(new Vector3Int(x, y, 0)))
            {
                if (currentGapBounds.y - currentGapBounds.x > largestGapBounds.y - largestGapBounds.x)
                {
                    largestGapBounds = currentGapBounds;
                }

                currentGapBounds.x = currentGapBounds.y + 1;
                currentGapBounds.y = currentGapBounds.x;
            }
            else
            {
                currentGapBounds.y += 1;
            }
        }

        return largestGapBounds;
    }

    private void SpawnVerticalBarObstacle(int x)
    {
        Color color = GetColorForColumn(x);
        int centerHeight = GetCaveMidAtPos(x) + Random.Range(-1, 1);

        for (int y = centerHeight - GetObstacleHeight() / 2; y < centerHeight + GetObstacleHeight() / 2; y++)
        {
            SetTile(x, y, color);
        }
    }

    private int GetSlabCount()
    {
        switch (GameState.Player.SelectedDifficulty)
        {
            case (DifficultySetting.Casual):
                return 3;
            case (DifficultySetting.Intense):
                return 4;
            default:
                throw new System.Exception("No switch implemented for difficulty: " + GameState.Player.SelectedDifficulty);
        }
    }

    private void SpawnHorizontalSlabs(int x)
    {
        Color color = GetColorForColumn(x);
        int centerHeight = GetCaveMidAtPos(x);
        int obstacleWidth = GetObstacleHeight();
        int distanceBetweenSlabs = (Constants.CAVE_RADIUS * 2) / (GetSlabCount() + 1);
        for (int slabIndex = 0; slabIndex < GetSlabCount(); slabIndex++)
        {
            for (int xi = x - obstacleWidth; xi < x + obstacleWidth; xi++)
            {
                SetTile(xi, centerHeight + Constants.BOTTOM_HEIGHT + slabIndex * distanceBetweenSlabs, color);
            }
        }
    }

    private void SpawnCenterGapObstacle(int x)
    {
        Color color = GetColorForColumn(x);
        Vector3Int pos = new Vector3Int(x, 0, 0);
        for (int y = CenterGapObstacleHalfHeight(); y < Constants.TOP_HEIGHT; y++)
        {
            SetTile(x, y, color);
            SetTile(x, -y, color);
        }
    }

    private void SpawnSaveMarkerIfApplicable(int x)
    {
        if (x % Constants.DISTANCE_BETWEEN_SAVES == 0 && x > 0)
        {
            int markerYPos = GetCaveMidAtPos(x);
            while (markerYPos > Constants.BOTTOM_HEIGHT && !Tilemap.HasTile(new Vector3Int(x, markerYPos, 0)))
            {
                markerYPos -= 1;
            }
            markerYPos += 1;
            GameObject marker = Instantiate(
                SaveMarker,
                new Vector3(
                    x + Constants.BLOCK_WIDTH / 4,
                    markerYPos + Constants.BLOCK_WIDTH / 4,
                    10) * Constants.BLOCK_WIDTH, new Quaternion());
            SaveMarkers.Add(marker);
            marker.GetComponent<SaveMarker>().ZoneIndex = x / Constants.DISTANCE_BETWEEN_SAVES;
            for (int xi = x - 1; xi <= x + 1; xi++)
            {
                for (int y = markerYPos - 1; y > Constants.BOTTOM_HEIGHT; y--)
                {
                    SetTile(xi, y);
                }
            }
            Tilemap.SetTile(new Vector3Int(x - 1, markerYPos, 0), null);
            Tilemap.SetTile(new Vector3Int(x + 1, markerYPos, 0), null);
        }
    }

    public static Color GetColorForColumn(int x)
    {
        int index = (x / Constants.DISTANCE_BETWEEN_SAVES);
        return Color.Lerp(
            GetColor(mod((index - 1), (CasualColors.Length - 1))),
            GetColor(mod(index, (CasualColors.Length - 1))),
            (float)(x % Constants.DISTANCE_BETWEEN_SAVES) / Constants.DISTANCE_BETWEEN_SAVES);
    }

    static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    private void SetTile(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Tilemap.SetTile(pos, BaseTile);
        Tilemap.SetColor(pos, GetColorForColumn(x));
    }

    private void SetTile(int x, int y, Color color)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Tilemap.SetTile(pos, BaseTile);
        Tilemap.SetColor(pos, color);
    }

    public static int GetCaveMidAtPos(int x)
    {
        int scaledX = x / 2;
        return (int)(Mathf.Sin(.4f * scaledX) + Mathf.Cos(.3f * scaledX) + Mathf.Sin(.4f * scaledX) + Mathf.Cos(.4f * scaledX) + Mathf.Sin(.6f * scaledX));
    }
}
