using UnityEngine;

namespace Assets.Scripts
{
    class Helpers
    {
        public static Vector2 convBlockID_XY(int blockID, Vector2 levelSize)
        {
            return new Vector2(Mathf.FloorToInt(blockID / levelSize.y), Mathf.FloorToInt(blockID % levelSize.y));
        }

        public static int convXY_BlockID(Vector2 coords, Vector2 levelSize)
        {
            return (int)coords.y + ((int)coords.x * (int)levelSize.y);
        }
    }
}
