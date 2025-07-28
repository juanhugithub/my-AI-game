// BaseDataSO.cs
using UnityEngine;

public abstract class BaseDataSO : ScriptableObject
{
    [Tooltip("该资产的全局唯一ID，由系统自动生成，请勿手动修改。")]
    public string guid;
}