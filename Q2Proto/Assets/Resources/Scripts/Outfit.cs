using System;
using UnityEngine;


[CreateAssetMenu(menuName ="Game Data Type/Outfit")]
public class Outfit : ScriptableObject
{

    [Tooltip("the prefab of this outfit. (ig: hat,..)")]
    public GameObject asset;

    [Tooltip("the local position of the outfit")]
    public AssetPosition[] positions;

}

[Serializable]
public struct AssetPosition
{
    public int idEnemy;
    public Vector3 position;
}