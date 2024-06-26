using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap VisualGrid;
    public Tilemap Tilemap;
    public GameObject SaveMarker;
    public float ObstaclePerlinScale;
    public float ObstaclePerlinCutoff;
    public GameObject GemPrefab;

    // Coins
    public GameObject Coin;
    // A dictionary with x being the key and y being the midpoint of the coin curve.
    public Dictionary<int, float> CoinCurveMidPoints = new Dictionary<int, float>();

    public Tile[] TileCases;

    private int highestRenderedBlock;
    private int lastObstacleSpawnXPos;
    private System.Random random;
    private Vector2Int renderedRanges;
    private List<GameObject> SaveMarkers;
    private List<GameObject> InstantiatedGems;
    private static readonly Color[] colors =
    {
        ColorExtensions.Create("#e23f44"),
        ColorExtensions.Create("#e95e32"),
        ColorExtensions.Create("#ef8c2a"),
        ColorExtensions.Create("#efb824"),
        ColorExtensions.Create("#f6e31f"),
        ColorExtensions.Create("#e23fde"),
        ColorExtensions.Create("#e23faf"),
        ColorExtensions.Create("#e23f7a"),
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

    private static Color GetColor(int index)
    {
        return colors[index % colors.Length];
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        InstantiatedGems = new List<GameObject>();
        SaveMarkers = new List<GameObject>();
        random = new System.Random(0);
    }

    void Start()
    {
        InitiallySetupTiles();
    }

    void Update()
    {
        while (highestRenderedBlock < GetHelicopterBlockPos() + Constants.NUM_COLUMNS_RENDERED / 2)
        {
            highestRenderedBlock += 1;
            ConfigureColumn(highestRenderedBlock);

            if (highestRenderedBlock / Constants.DISTANCE_BETWEEN_SAVES > renderedRanges.y)
            {
                renderedRanges.y = highestRenderedBlock / Constants.DISTANCE_BETWEEN_SAVES;
            }
        }
    }

    private int GetHelicopterBlockPos()
    {
        return Managers.Helicopter.Distance;
    }

    public void ResetGems()
    {
        foreach (GameObject gem in InstantiatedGems)
        {
            gem.GetComponent<Gem>().Reset();
        }
    }

    private void InitiallySetupTiles()
    {
        for (int i = GetHelicopterBlockPos() - Constants.NUM_COLUMNS_RENDERED / 2; i < GetHelicopterBlockPos() + Constants.NUM_COLUMNS_RENDERED / 2; i++)
        {
            ConfigureColumn(i);
        }

        this.highestRenderedBlock = GetHelicopterBlockPos() + Constants.NUM_COLUMNS_RENDERED / 2 - 1;
    }

    private void ConfigureColumn(int x)
    {
        SpawnTilesForColumn(x);
        SpawnSaveMarkerIfApplicable(x - 1);
        SpawnObstacle(x - 4);
        SpawnGem(x - 5);
        UpdateVisualGridForColumn(x - 10);
        // SpawnCoin(x - DIST_BETWEEN_CURVE_CENTERS - 5);
    }

    private void SpawnTilesForColumn(int x)
    {
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
            perlinValue += distanceToMid * .04f;

            if (perlinValue < ObstaclePerlinCutoff)
            {
                continue;
            }

            SetTile(x, y);
        }

        SetTile(x, Constants.BOTTOM_HEIGHT);
        SetTile(x, Constants.BOTTOM_HEIGHT + 1);
        SetTile(x, Constants.TOP_HEIGHT);
        SetTile(x, Constants.TOP_HEIGHT - 1);
    }

    private void UpdateVisualGridForColumn(int x)
    {
        for (int y = Constants.BOTTOM_HEIGHT - 1; y <= Constants.TOP_HEIGHT + 1; y++)
        {
            UpdateVisualGrid(x, y);
        }
    }

    private void UpdateVisualGrid(int x, int y)
    {
        int whichCase = 0;
        Vector3Int pos = new Vector3Int(x, y, 0);
        if (Tilemap.HasTile(pos))
        {
            whichCase = whichCase | 1;
        }

        pos.x += 1;
        if (Tilemap.HasTile(pos))
        {
            whichCase = whichCase | 2;
        }

        pos.y += 1;
        if (Tilemap.HasTile(pos))
        {
            whichCase = whichCase | 4;
        }

        pos.x -= 1;
        if (Tilemap.HasTile(pos))
        {
            whichCase = whichCase | 8;
        }

        VisualGrid.SetTile(new Vector3Int(x, y, 0), TileCases[whichCase]);
    }

    private static int GetObstacleHeight()
    {
        return 7;
    }

    private static int CenterGapObstacleHalfHeight()
    {
        return 3;
    }

    private void SpawnObstacle(int x)
    {
        if (x % Constants.DIST_BETWEEN_OBSTACLES != 0 || x <= 0)
        {
            return;
        }

        // Don't spawn on top of save point.
        if (x % Constants.DISTANCE_BETWEEN_SAVES == 0)
        {
            return;
        }

        int obstacleType = this.random.Next(0, 3);
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

        int index = x / DIST_BETWEEN_CURVE_CENTERS;
        CoinCurveMidPoints[index] = findLargestGap(x);
    }

    private void SpawnGem(int x)
    {
        if ((x + Constants.DIST_BETWEEN_OBSTACLES / 2) % Constants.DIST_BETWEEN_OBSTACLES != 0 || x <= 0)
        {
            return;
        }

        float midPoint = findLargestGap(x);
        int index = x / DIST_BETWEEN_CURVE_CENTERS;
        CoinCurveMidPoints[index] = midPoint;

        int obstacleIndex = (x + Constants.DIST_BETWEEN_OBSTACLES / 2) / Constants.DIST_BETWEEN_OBSTACLES;
        if (obstacleIndex % 2 != 0)
        {
            return;
        }

        if (x <= GameState.Player.GetHighestRegionUnlocked() * Constants.DISTANCE_BETWEEN_SAVES)
        {
            return;
        }

        float xActualPos = x * Constants.BLOCK_WIDTH;
        GameObject gem = Instantiate(
            GemPrefab,
            new Vector3(xActualPos, midPoint, 0),
            new Quaternion(),
            this.transform);
        gem.GetComponent<Gem>().Init(x);
        InstantiatedGems.Add(gem);
    }

    private const int DIST_BETWEEN_CURVE_CENTERS = Constants.DIST_BETWEEN_OBSTACLES / 2;
    // There are two curves for each gap between a gem and an obstacle.
    private const int DIST_BETWEEN_CURVES = Constants.DIST_BETWEEN_OBSTACLES / 4;
    private const float HALF_CURVE_WORLD_DIST = DIST_BETWEEN_CURVES * Constants.BLOCK_WIDTH;
    private void SpawnCoin(int x)
    {
        Vector2? pos = Interpolate(x);

        if (pos == null)
        {
            return;
        }

        RemoveTile((int)(pos.Value.x / Constants.BLOCK_WIDTH), (int)(pos.Value.y / Constants.BLOCK_WIDTH));
        RemoveTile((int)(pos.Value.x / Constants.BLOCK_WIDTH), (int)((pos.Value.y + 1) / Constants.BLOCK_WIDTH));
        RemoveTile((int)(pos.Value.x / Constants.BLOCK_WIDTH), (int)((pos.Value.y - 1) / Constants.BLOCK_WIDTH));

        if (!shouldSpawnCoin(x))
        {
            return;
        }

        var coin = Instantiate(Coin, pos.Value, new Quaternion());
        coin.GetComponent<Coin>().Init(x);
    }

    bool shouldSpawnCoin(int x)
    {
        if (x % 2 != 0)
        {
            return false;
        }

        int fives = (x / 10) % 20;
        if (fives == 3 || fives == 8 || fives == 14)
        {
            return true;
        }

        int tens = (x / 20) % 20;
        if (tens == 14 || tens == 1 || tens == 10 || tens == 19)
        {
            return true;
        }

        int twenties = (x / 40) % 20;
        if (fives == 5 || fives == 12)
        {
            return true;
        }

        return false;
    }

    private Vector2? Interpolate(int x)
    {
        Vector2? a = GetPoint(x);
        Vector2? c = GetPoint(x + DIST_BETWEEN_CURVES);

        if (a == null || c == null)
        {
            return null;
        }

        Vector2 b;
        if (IsFirstHalfOfCurve(x))
        {
            b = new Vector2(c.Value.x - 1, a.Value.y);
        }
        else
        {
            b = new Vector2(a.Value.x + 1, c.Value.y);
        }

        // Binary search the spline for a close position.
        float targetXPos = x * Constants.BLOCK_WIDTH;
        float t = .5f;
        float remainingSize = .5f;
        Vector2 result = -Vector3.one;
        int breakout = 0;
        while (Mathf.Abs(result.x - targetXPos) > .05f)
        {
            // Debug.Log($"Log t at {t}, result at {result}, and target at {targetXPos}");
            var ab = Vector2.Lerp(a.Value, b, t);
            var bc = Vector2.Lerp(b, c.Value, t);
            result = Vector2.Lerp(ab, bc, t);

            if (result.x > targetXPos)
            {
                t -= remainingSize;
            }
            else
            {
                t += remainingSize;
            }

            remainingSize /= 2;

            breakout += 1;
            if (breakout > 10)
            {
                Debug.Log($"Giving up with t at {t}, result at {result}, and target at {targetXPos}");
                return result;
            }
        }

        return result;
    }

    private Vector2? GetPoint(int x)
    {
        int curveIndex = x / DIST_BETWEEN_CURVES;
        int centerIndex = x / DIST_BETWEEN_CURVE_CENTERS;

        if (IsFirstHalfOfCurve(x))
        {
            if (!CoinCurveMidPoints.ContainsKey(centerIndex))
            {
                return null;
            }

            return new Vector2(
                (centerIndex * DIST_BETWEEN_CURVE_CENTERS) * Constants.BLOCK_WIDTH,
                CoinCurveMidPoints[centerIndex]);
        }
        else
        {
            if (!CoinCurveMidPoints.ContainsKey(centerIndex) ||
                !CoinCurveMidPoints.ContainsKey(centerIndex + 1))
            {
                return null;
            }

            float midY =
                CoinCurveMidPoints[centerIndex] +
                (CoinCurveMidPoints[centerIndex + 1] - CoinCurveMidPoints[centerIndex]) / 2;

            return new Vector2((curveIndex * DIST_BETWEEN_CURVES) * Constants.BLOCK_WIDTH, midY);
        }
    }

    private bool IsFirstHalfOfCurve(int x)
    {
        return (((x / DIST_BETWEEN_CURVES) * DIST_BETWEEN_CURVES) % DIST_BETWEEN_CURVE_CENTERS == 0);
    }

    private float findLargestGap(int x)
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

        bool isOdd = (largestGapBounds.y - largestGapBounds.x) % 2 != 0;
        Vector2 largestGap = ((Vector2)largestGapBounds) * Constants.BLOCK_WIDTH;
        if (isOdd) largestGap.y -= Constants.BLOCK_WIDTH;
        else largestGap.y -= Constants.BLOCK_WIDTH / 2;
        return largestGap.x + (largestGap.y - largestGap.x) / 2f;
    }

    private void SpawnVerticalBarObstacle(int x)
    {
        int centerHeight = GetCaveMidAtPos(x) + this.random.Next(-1, 1);

        for (int y = centerHeight - GetObstacleHeight() / 2; y < centerHeight + GetObstacleHeight() / 2; y++)
        {
            for (int xi = x - 1; xi <= x + 1; xi++)
            {
                SetTile(xi, y);
            }
        }
    }

    private int GetSlabCount()
    {
        return 3;
    }

    private void SpawnHorizontalSlabs(int x)
    {
        int centerHeight = GetCaveMidAtPos(x);
        int obstacleWidth = GetObstacleHeight();
        int distanceBetweenSlabs = (Constants.CAVE_RADIUS * 2) / (GetSlabCount() + 1);
        for (int slabIndex = 0; slabIndex < GetSlabCount(); slabIndex++)
        {
            for (int xi = x - obstacleWidth / 2; xi < x + obstacleWidth; xi++)
            {
                int yPos = centerHeight + Constants.BOTTOM_HEIGHT + slabIndex * distanceBetweenSlabs;
                for (int y = yPos; y <= yPos + 1; y++)
                {
                    SetTile(xi, y);
                }
            }
        }
    }

    private void SpawnCenterGapObstacle(int x)
    {
        Vector3Int pos = new Vector3Int(x, 0, 0);
        for (int y = CenterGapObstacleHalfHeight(); y < Constants.TOP_HEIGHT; y++)
        {
            for (int xi = x - 1; xi <= x + 1; xi++)
            {
                SetTile(xi, y);
                SetTile(xi, -y);
            }
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
            SaveMarker save = marker.GetComponent<SaveMarker>();
            save.ZoneIndex = x / Constants.DISTANCE_BETWEEN_SAVES;
            if (x < GameState.Player.HighestDistanceUnlocked)
            {
                save.ForceLight();
            }

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
        float index = (((float)x) / Constants.DISTANCE_BETWEEN_SAVES) % Constants.COLORS_PER_CYCLE;
        index /= Constants.COLORS_PER_CYCLE;
        return Color.HSVToRGB(index, .5f, .9f);
        // return Color.Lerp(
        //     GetColor(mod((index - 1), (colors.Length - 1))),
        //     GetColor(mod(index, (colors.Length - 1))),
        //     (float)(x % Constants.DISTANCE_BETWEEN_SAVES) / Constants.DISTANCE_BETWEEN_SAVES);
    }

    static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    private void SetTile(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Tilemap.SetTile(pos, TileCases[0]);
    }

    private void RemoveTile(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Tilemap.SetTile(pos, null);
    }

    public static int GetCaveMidAtPos(int x)
    {
        int scaledX = x / 2;
        return (int)(Mathf.Sin(.4f * scaledX) + Mathf.Cos(.3f * scaledX) + Mathf.Sin(.4f * scaledX) + Mathf.Cos(.4f * scaledX) + Mathf.Sin(.6f * scaledX));
    }
}
