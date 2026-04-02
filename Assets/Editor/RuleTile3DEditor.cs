using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RuleTile3D))]
public class RuleTile3DEditor : Editor
{
    private readonly Vector2Int[] positions = new Vector2Int[]
    {
        new(-1,1), new(0,1), new(1,1),
        new(-1,0),           new(1,0),
        new(-1,-1), new(0,-1), new(1,-1)
    };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var rulesProp = serializedObject.FindProperty("rules");

        for (int r = 0; r < rulesProp.arraySize; r++)
        {
            var rule = rulesProp.GetArrayElementAtIndex(r);
            var neighbors = rule.FindPropertyRelative("neighbors");
            if (neighbors.arraySize != 8)
            {
                neighbors.arraySize = 8;
            }

            var prefab = rule.FindPropertyRelative("prefab");
            var edgePrefab = rule.FindPropertyRelative("edgePrefab");

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(prefab);
            EditorGUILayout.PropertyField(edgePrefab);

            DrawGrid(neighbors);

            if (GUILayout.Button("Remove Rule"))
            {
                rulesProp.DeleteArrayElementAtIndex(r);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Rule"))
        {
            int index = rulesProp.arraySize;
            rulesProp.InsertArrayElementAtIndex(index);

            var newRule = rulesProp.GetArrayElementAtIndex(index);
            var prefabProp = newRule.FindPropertyRelative("prefab");

            prefabProp.objectReferenceValue = null; // hoặc defaultTile nếu bạn muốn
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawGrid(SerializedProperty neighbors)
    {
        for (int y = 0; y < 3; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < 3; x++)
            {
                if (x == 1 && y == 1)
                {
                    GUILayout.Box("X", GUILayout.Width(40), GUILayout.Height(40));
                    continue;
                }

                int index = GetIndex(x, y);
                var cell = neighbors.GetArrayElementAtIndex(index);

                NeighborCondition cond = (NeighborCondition)cell.enumValueIndex;

                GUI.backgroundColor = GetColor(cond);

                if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(40)))
                {
                    cond = (NeighborCondition)(((int)cond + 1) % 3);
                    cell.enumValueIndex = (int)cond;
                }

                GUI.backgroundColor = Color.white;
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    int GetIndex(int x, int y)
    {
        int[,] map =
        {
            {0,1,2},
            {3,-1,4},
            {5,6,7}
        };

        return map[y, x];
    }

    Color GetColor(NeighborCondition cond)
    {
        return cond switch
        {
            NeighborCondition.Same => Color.green,
            NeighborCondition.Different => Color.red,
            _ => Color.gray
        };
    }
}
