namespace LowLevel
{
    public class ConstConfig
    {
        // Rotations (radians)
        internal const float LEFT_ROTATION = -1.57079633f;
        internal const float RIGHT_ROTATION = 1.57079633f;
        internal const float UP_ROTATION = 0;
        internal const float DOWN_ROTATION = 3.14159265f;

        // N tiles to side frog can move
        internal const float BACKWARD_LIMIT = 0f;

        internal const float AXIS_DEADZONE = 0.2f;

        // Must be 2!
        internal const int TILE_SIZE = 2;
    }
}