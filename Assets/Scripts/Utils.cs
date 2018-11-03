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

        public static TransformState GetSpawnLocation(int order) {
            float angleStep = (Mathf.PI * 2f) / 4f;
            float angle = angleStep * order;
            return new TransformState() {
                position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 5f,
                rotation = Quaternion.AngleAxis(-90f + angle * Mathf.Rad2Deg, new Vector3(0, 0, 1))
            };
        }
    }

    public struct TransformState {
        public Vector3 position;
        public Quaternion rotation;
    }
}
