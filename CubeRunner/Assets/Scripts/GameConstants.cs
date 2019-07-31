using System;

namespace Assets.Scripts
{
    public static class GameConstants
    {
        public static float blockWidth = (float)Math.Pow(2f, 0.5f);
        public static float halfblockWidth = ((float)Math.Pow(2f, 0.5f)) / 2f;
        public static int rowLeadLength = 17;

        public static bool highlightValidPath = false;
        public static bool setRandomColorBlocks = false;
        public static float switchAreaValue = 0.2f;

        public static int initialRowOddWidth = 5;
    }
}