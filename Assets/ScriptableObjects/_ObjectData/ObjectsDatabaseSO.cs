using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsDatabaseSO", menuName = "ScriptableObjects/ObjectsDatabaseSO")]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<BaseData> objectDatas;
}
