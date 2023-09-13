
using UnityEngine;
using UnityEditor;
using Devarc;


public static class CustomEditorUtil
{
    public static string clipboard
    {
        get
        {
            TextEditor te = new TextEditor();
            te.Paste();
            return te.text;
        }
        set
        {
            TextEditor te = new TextEditor();
            te.text = value;
            te.OnFocus();
            te.Copy();
        }
    }

    public static string ToShaderName(string _value)
    {
        if (string.IsNullOrEmpty(_value))
            return "";
        return _value.Substring(_value.LastIndexOf('/') + 1);
    }

    public static void DrawRect(Vector3 _pos, Rect _rect)
    {
        float width = _rect.width;
        float height = _rect.height;
        Vector3 pos = _pos + new Vector3(_rect.x, _rect.y, 0);

        {
            Vector3 from = pos + new Vector3(0, height, 0);
            Vector3 dest = pos + new Vector3(width, height, 0);
            Gizmos.DrawLine(from, dest);
        }

        {
            Vector3 from = pos + new Vector3(0, 0, 0);
            Vector3 dest = pos + new Vector3(width, 0, 0);
            Gizmos.DrawLine(from, dest);
        }

        {
            Vector3 from = pos + new Vector3(0, 0, 0);
            Vector3 dest = pos + new Vector3(0, height, 0);
            Gizmos.DrawLine(from, dest);
        }

        {
            Vector3 from = pos + new Vector3(width, 0, 0);
            Vector3 dest = pos + new Vector3(width, height, 0);
            Gizmos.DrawLine(from, dest);
        }
    }


    public static void SplitRect(Rect _position, int _lineCount, out Rect[] _rects)
    {
        _rects = new Rect[_lineCount];

        float height = _position.height / (float)_lineCount;
        for (int i = 0; i < _lineCount; i++)
        {
            _rects[i] = _position;
            _rects[i].y = _position.y + (float)i * height;
            _rects[i].height = height;
        }
    }

    public static void SplitRect(Rect _position, float _indent, out Rect[] _rects, params float[] _ratios)
    {
        _rects = new Rect[_ratios.Length];
        float contentWidth = Mathf.Max(0, _position.width - _indent);
        for (int i = 0; i < _ratios.Length; i++)
        {
            _rects[i].height = _position.height;
            _rects[i].width = contentWidth * _ratios[i];
            if (i == 0)
            {
                _rects[i].x = _indent;
            }
            else
            {
                _rects[i].x = _rects[i - 1].x + _rects[i - 1].width;
            }
            _rects[i].y = _position.y;
        }
        for (int i = 0; i < _rects.Length; i++)
        {
            _rects[i].width += 15f;
        }
    }


    public static Vector3 ToUISpace(Camera _worldCam, Vector3 _worldPos, Canvas _canvas)
    {
        Vector3 screenPos = _worldCam.WorldToScreenPoint(_worldPos);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _canvas.worldCamera, out movePos);
        return _canvas.transform.TransformPoint(movePos);
    }
}
