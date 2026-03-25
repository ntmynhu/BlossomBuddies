using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsDatabaseSO", menuName = "Scriptable Objects/ObjectsDatabaseSO")]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<BaseData> objectDatas;
}
