using UnityEngine;

namespace Ramjet {
    public static class Utilities {
        public static int PackColor(Color col) {
            int result = 0;
            result |= (byte)(col.r * 255f) << 0;
            result |= (byte)(col.g * 255f) << 8;
            result |= (byte)(col.b * 255f) << 16;
            result |= (byte)(col.a * 255f) << 24;
            return result;
        }

        public static Color UnpackColor(int col) {
            Color result = new Color();
            result.r = ((col >> 0) & 0x000000FF) / 255f;
            result.g = ((col >> 8) & 0x000000FF) / 255f;
            result.b = ((col >> 16) & 0x000000FF) / 255f;
            result.a = ((col >> 24) & 0x000000FF) / 255f;
            return result;
        }
    }
}
