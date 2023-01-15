public static class Constants
{
    public const float BLOCK_WIDTH = .5f;
    public const int WINDOW_BLOCK_HEIGHT = 26;
    public const int NUM_COLUMNS_RENDERED = 100;
    public const int TOP_HEIGHT = WINDOW_BLOCK_HEIGHT / 2;
    public const int BOTTOM_HEIGHT = -WINDOW_BLOCK_HEIGHT / 2 - 1;
    public const int CAVE_RADIUS = 12;
    public const int DISTANCE_BETWEEN_SAVES = 615;
    public const int COLORS_PER_CYCLE = 16;
    public const int DIST_BETWEEN_OBSTACLES = 30;

    public static class Tags
    {
        public const string Block = "Block";
        public const string Helicopter = "Helicopter";
        public const string ColoredUI = "ColoredUI";
    }

    public static class Layers
    {
        public const int Blocks = 1 << 9;
    }
}