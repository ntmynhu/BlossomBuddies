using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ToolDatabaseSO", menuName = "Scriptable Objects/ToolDatabaseSO")]
public class ToolDatabaseSO : ScriptableObject
{
    public List<ToolInfo> toolDatas;
}
