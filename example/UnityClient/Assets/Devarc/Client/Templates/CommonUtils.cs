using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devarc
{
    public static class CommonUtils
    {
        public static T SafeGetComponent<T>(this GameObject go) where T : Component
        {
            T compo = go.GetComponent<T>();
            if (compo == null)
            {
                compo = go.AddComponent<T>();
            }
            return compo;
        }

        public static string SafeString(this string value)
        {
            if (value == null) return string.Empty;
            return value;
        }

        public static bool IsValid(this string value) => !string.IsNullOrWhiteSpace(value);

        public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

        public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
        {
            foreach (Transform child in self)
            {
                if (selector(child))
                {
                    return child;
                }

                var finding = child.FindRecursive(selector);

                if (finding != null)
                {
                    return finding;
                }
            }
            return null;
        }

        static string[] defaultStringArray = new string[0];
        public static string[] Split(this string value, string token)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultStringArray;
            }
            return value.Split(token);
        }

        public static T[] Split<T>(this string value, string token) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new T[0];
            }
            var list = value.Split(token, StringSplitOptions.RemoveEmptyEntries);
            T[] result = new T[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                System.Enum.TryParse<T>(list[i], out result[i]);
            }
            return result;
        }

        public static float GetFloat(this string[] list, int index)
        {
            if (list == null || list.Length <= index)
                return 0;
            float result = 0;
            float.TryParse(list[index], out result);
            return result;
        }

        public static Vector2 ToUIPos(this Vector3 worldPos, Camera worldCam, RectTransform canvasTr)
        {
            Vector2 viewPort = worldCam.WorldToViewportPoint(worldPos);
            Vector2 screenPos = new Vector2(
            ((viewPort.x * canvasTr.sizeDelta.x) - (canvasTr.sizeDelta.x * 0.5f)),
            ((viewPort.y * canvasTr.sizeDelta.y) - (canvasTr.sizeDelta.y * 0.5f)));

            var anchoredPosition = screenPos;
            return anchoredPosition;
        }
    }
}
