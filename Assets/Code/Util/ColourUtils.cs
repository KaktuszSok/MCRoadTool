using UnityEngine;

namespace Code.Util
{
    public static class ColourUtils
    {
        /// <summary>
        /// Adds two colours and returns the result with a set alpha value
        /// </summary>
        public static Color32 addClamped(Color32 a, Color32 b, byte resultAlpha)
        {
            return new Color32(
                (byte)Mathf.Clamp(a.r + b.r, 0, 255),
                (byte)Mathf.Clamp(a.g + b.g, 0, 255),
                (byte)Mathf.Clamp(a.b + b.b, 0, 255),
                resultAlpha
                );
        }
        
        /// <summary>
        /// Subtracts two colours and returns the result with a set alpha value
        /// </summary>
        public static Color32 subtractClamped(Color32 a, Color32 b, byte resultAlpha)
        {
            return new Color32(
                (byte)Mathf.Clamp(a.r - b.r, 0, 255),
                (byte)Mathf.Clamp(a.g - b.g, 0, 255),
                (byte)Mathf.Clamp(a.b - b.b, 0, 255),
                resultAlpha
            );
        }
    }
}