using UnityEngine;


[CreateAssetMenu(menuName ="Game Data Type/Outfit")]
public class Outfit : ScriptableObject
{
    [Tooltip("the enemy type based on prefab name")]
    public int enemyID = 0;

    [Tooltip("the prefab of this outfit. (ig: hat,..)")]
    public GameObject asset;

    [Tooltip("the local position of the outfit")]
    public Vector3 position;

}
