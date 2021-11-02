public static class Constants
{
    public const float BLOCK_WIDTH = 1;
    public const int WINDOW_BLOCK_HEIGHT = 13;
    public const int NUM_COLUMNS_RENDERED = 50;
    public const int TOP_HEIGHT = WINDOW_BLOCK_HEIGHT / 2;
    public const int BOTTOM_HEIGHT = -WINDOW_BLOCK_HEIGHT / 2 - 1;
    public const int CAVE_RADIUS = 7;
    public const int DISTANCE_BETWEEN_SAVES = 200;

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