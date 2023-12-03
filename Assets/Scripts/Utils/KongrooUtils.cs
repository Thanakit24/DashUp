using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;


[System.Serializable]
public struct TransformLite
{
    public Vector3 pos;

    //Euler positioning doesnt fix it (unity probably stores it as eulerangles)
    public Quaternion rot;
    // public Vector3 rot;
}

#if UNITY_EDITOR

public class ScriptableObjectIdAttribute : PropertyAttribute { }
[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        if (string.IsNullOrEmpty(property.stringValue))
        {
            property.stringValue = Guid.NewGuid().ToString();
        }
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif

[System.Serializable]
public struct SerialKeyValuePair<K, V>
{
    public K key;
    public V val;
}

public static class KongrooUtils
{
    public static Dictionary<K, V> ToDict<K, V>(this SerialKeyValuePair<K, V>[] self)
    {
        return self.ToDictionary(keyVal => keyVal.key, keyVal => keyVal.val);
    }

    public static Vector2 Vector2FromAngle(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    public static Vector2 ClampMagnitude(Vector2 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if (sm > max * (double)max) return v.normalized * max;
        else if (sm < min * (double)min) return v.normalized * min;
        return v;
    }
    public static T[] ShuffleArray<T>(T[] array)
    {
        int currentIndex = array.Length;
        while (currentIndex != 0)
        {
            //Pick random index
            int rand = Random.Range(0, currentIndex);
            currentIndex--;
            T temp1 = array[rand];
            T temp2 = array[currentIndex];

            array[currentIndex] = temp1;
            array[rand] = temp2;
        }

        return array;
    }
    public static T RandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
    public static T RandomElement<T>(this T[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }

    public static void DestroyAllWithTag(string tag)
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("DrawnLine"))
            GameObject.Destroy(go);
    }

    public static bool approx(float current, float target, float epsilon)
    {
        return (current > target - epsilon && current < target + epsilon);
    }

    public static float RemapRange(float value, float inputA, float inputB, float outputA, float outputB)
    {
        return (value - inputA) / (inputB - inputA) * (outputB - outputA) + outputA;
    }

    public static Vector3 SlerpCenter(Vector3 p1, Vector3 p2, Vector3 center, float t)
    {
        var startNormalized = p1 - center;
        var endNormalized = p2 - center;
        return Vector3.Slerp(startNormalized, endNormalized, t) + center;
    }
//TODO: Make own version of slerp using ray(direction, origin and radius)    

    public static void DrawGizmoCircle(Vector2 center, float radius, Color color, int segments = 200)
    {
        Gizmos.color = color;
        float angle = 0;
        float increment = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 firstLoc = direction * radius + center;
            angle += increment;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 secondLoc = direction * radius + center;
            Gizmos.DrawLine(firstLoc, secondLoc);
        }
    }


    public static void DrawDebugCircle(Vector2 center, float radius, Color color, float duration, int segments = 200)
    {
        float angle = 0;
        float increment = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 firstLoc = direction * radius + center;
            angle += increment;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 secondLoc = direction * radius + center;
            Debug.DrawLine(firstLoc, secondLoc, color, duration);
        }
    }

    public static void DrawDebugCircle(Vector2 center, float radius, Color color, int segments = 200)
    {
        float angle = 0;
        float increment = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 firstLoc = direction * radius + center;
            angle += increment;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 secondLoc = direction * radius + center;
            Debug.DrawLine(firstLoc, secondLoc, color);
        }
    }

    public static void DrawGraphLine(ref Vector2 oldPoint, Vector2 newPoint, Vector2? iOrigin = null)
    {
        var origin = iOrigin ?? Vector2.zero;
        Debug.DrawLine(oldPoint, newPoint, Color.green, 99999f);
        oldPoint = newPoint;
    }


    public static void Test()
    {
        Transform[] chars = new Transform[12];

        bool isFull = true;
        for (int i = 0; i < 12; i++)
        {
            if (chars[i] == null)
            {
                // chars[i] = Instantio
                isFull = false;
                break;
            }
        }

        if (isFull)
        {
            Debug.Log("Fugg we full no more");
        }
    }

    public static void DebugDrawSphere(Vector3 center, float rad)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        plane.transform.position = center;
    }

    public static void DrawWireCapsule(Vector3 _pos, Vector3 _pos2, float _radius, float _height,
        Color _color = default)
    {
#if UNITY_EDITOR
        if (_color != default) Handles.color = _color;

        var forward = _pos2 - _pos;
        var _rot = Quaternion.LookRotation(forward);
        var pointOffset = _radius / 2f;
        var length = forward.magnitude;
        var center2 = new Vector3(0f, 0, length);

        Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);

        using (new Handles.DrawingScope(angleMatrix))
        {
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, _radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.left * pointOffset, -180f, _radius);
            Handles.DrawWireArc(Vector3.zero, Vector3.left, Vector3.down * pointOffset, -180f, _radius);
            Handles.DrawWireDisc(center2, Vector3.forward, _radius);
            Handles.DrawWireArc(center2, Vector3.up, Vector3.right * pointOffset, -180f, _radius);
            Handles.DrawWireArc(center2, Vector3.left, Vector3.up * pointOffset, -180f, _radius);

            DrawLine(_radius, 0f, length);
            DrawLine(-_radius, 0f, length);
            DrawLine(0f, _radius, length);
            DrawLine(0f, -_radius, length);
        }
#endif
    }

    private static void DrawLine(float arg1, float arg2, float forward)
    {
#if UNITY_EDITOR
        Handles.DrawLine(new Vector3(arg1, arg2, 0f), new Vector3(arg1, arg2, forward));
#endif
    }
}

/*
Vector Range Attribute by Just a Pixel (Danny Goodayle @DGoodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
USAGE
[VectorRange(minX, maxX, minY, maxY, clamped)]
public Vector2 yourVector;
*/

#if UNITY_EDITOR
public class VectorRangeAttribute : PropertyAttribute
{
    public readonly float fMinX, fMaxX, fMinY, fMaxY;
    public readonly bool bClamp;

    public VectorRangeAttribute(float fMinX, float fMaxX, float fMinY, float fMaxY, bool bClamp)
    {
        this.fMinX = fMinX;
        this.fMaxX = fMaxX;
        this.fMinY = fMinY;
        this.fMaxY = fMaxY;
        this.bClamp = bClamp;
    }
}
#endif

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(VectorRangeAttribute))]
public class VectorRangeAttributeDrawer : PropertyDrawer
{
    const int helpHeight = 30;
    const int textHeight = 16;

    VectorRangeAttribute rangeAttribute
    {
        get { return (VectorRangeAttribute) attribute; }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Color previous = GUI.color;
        GUI.color = !IsValid(property) ? Color.red : Color.white;
        Rect textFieldPosition = position;
        textFieldPosition.width = position.width;
        textFieldPosition.height = position.height;
        EditorGUI.BeginChangeCheck();
        Vector2 val = EditorGUI.Vector2Field(textFieldPosition, label, property.vector2Value);
        if (EditorGUI.EndChangeCheck())
        {
            if (rangeAttribute.bClamp)
            {
                val.x = Mathf.Clamp(val.x, rangeAttribute.fMinX, rangeAttribute.fMaxX);
                val.y = Mathf.Clamp(val.y, rangeAttribute.fMinY, rangeAttribute.fMaxY);
            }

            property.vector2Value = val;
        }

        Rect helpPosition = position;
        helpPosition.y += 16;
        helpPosition.height = 16;
        DrawHelpBox(helpPosition, property);
        GUI.color = previous;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!IsValid(property))
        {
            return 32;
        }

        return base.GetPropertyHeight(property, label);
    }

    void DrawHelpBox(Rect position, SerializedProperty prop)
    {
        // No need for a help box if the pattern is valid.
        if (IsValid(prop))
            return;

        EditorGUI.HelpBox(position,
            string.Format("Invalid Range X [{0}]-[{1}] Y [{2}]-[{3}]", rangeAttribute.fMinX, rangeAttribute.fMaxX,
                rangeAttribute.fMinY, rangeAttribute.fMaxY), MessageType.Error);
    }

    bool IsValid(SerializedProperty prop)
    {
        Vector2 vector = prop.vector2Value;
        return vector.x >= rangeAttribute.fMinX && vector.x <= rangeAttribute.fMaxX &&
               vector.y >= rangeAttribute.fMinY && vector.y <= rangeAttribute.fMaxY;
    }
}
#endif