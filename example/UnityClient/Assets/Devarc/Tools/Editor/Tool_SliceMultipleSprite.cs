//
// @author: Kim, Hyoung Joon
// @e-mail: maoshy@nate.com)
// @license: http://www.apache.org/licenses/LICENSE-2.0
//

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Devarc
{

    public class Tool_SliceMultipleSprite : ScriptableWizard
    {
        [MenuItem("Tools/Devarc/Slice Multiple Sprite")]
        static void ShowEditor()
        {
            ScriptableWizard.DisplayWizard<Tool_SliceMultipleSprite>("Slice Multiple Sprite");
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            GUILayout.Label("Extract sprites");

            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.Box(drop_area, "Drag & drop a multiple-sprite...");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object obj in DragAndDrop.objectReferences)
                        {
                            string path = AssetDatabase.GetAssetPath(obj);
                            string dir = System.IO.Path.GetDirectoryName(path);

                            var list = AssetDatabase.LoadAllAssetsAtPath(path);
                            List<Sprite> sprites = new List<Sprite>();
                            foreach (var temp in list)
                            {
                                Sprite s = temp as Sprite;
                                if (s == null)
                                    continue;
                                sprites.Add(s);
                            }
                            if (sprites == null || sprites.Count <= 1)
                                continue;

                            string newDir = Path.Combine(dir, obj.name);
                            string projectDir = Path.GetDirectoryName(Path.GetDirectoryName(Application.streamingAssetsPath));
                            string fullDir = Path.Combine(projectDir, newDir);
                            if (!Directory.Exists(fullDir))
                            {
                                AssetDatabase.CreateFolder(dir, obj.name);
                            }

                            foreach (var sprite in sprites)
                            {
                                var texture = save(sprite, newDir);
                            }

                            EditorUtility.DisplayDialog("Update", "Sprite is extracted.", "Success");
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        Texture2D save(Sprite sprite, string dir)
        {
            int width = (int)sprite.rect.width;
            int height = (int)sprite.rect.height;
            int startX = (int)sprite.rect.x;
            int startY = (int)sprite.rect.y;
            var texture = new Texture2D(width, height);
            var pixels = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels[y * width + x] = sprite.texture.GetPixel(startX + x, startY + y);
                }
            }
            texture.SetPixels(pixels);
            texture.alphaIsTransparency = sprite.texture.alphaIsTransparency;
            texture.filterMode = sprite.texture.filterMode;
            texture.wrapMode = sprite.texture.wrapMode;
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            string path = string.Format("{0}.png", System.IO.Path.Combine(dir, sprite.name));
            System.IO.File.WriteAllBytes(path, bytes);
            UnityEditor.AssetDatabase.Refresh();

            TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(path);
            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
            settings.spritePixelsPerUnit = sprite.pixelsPerUnit;
            settings.filterMode = sprite.texture.filterMode;
            ti.SetTextureSettings(settings);
            ti.SaveAndReimport();

            return texture;
        }
    }

}