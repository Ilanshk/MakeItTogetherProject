using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ObjectSO : ScriptableObject
{
    public Transform prefab;
    public string objectName;
    public int amount;
    public int priceForItem;
}
