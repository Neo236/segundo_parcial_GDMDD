using UnityEngine;

[CreateAssetMenu(fileName = "NewSpawnPointID", menuName = "SpawnPointID")]
public class SpawnPointID : ScriptableObject
{
    [TextArea]
    [Tooltip("Descripción del spawn point")] //Opcional
    public string description;
}
