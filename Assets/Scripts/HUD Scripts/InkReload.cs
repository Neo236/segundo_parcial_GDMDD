using UnityEngine;
using UnityEngine.UI;

public class InkReload : MonoBehaviour
{
    [Header("Sprites de Tinta")]
    [SerializeField] private Sprite[] inkSprites; // Sprites para cada estado de tinta

    private Image inkImage; // El componente Image del objeto UI
    private PlayerInk playerInk;

    void Awake()
    {
        inkImage = GetComponent<Image>(); // El Image del objeto al que está este script
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerInk = player.GetComponent<PlayerInk>();
        }
        else
        {
            Debug.LogError("Player GameObject not found. Make sure it has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (playerInk != null)
        {
            UpdateInkImage();
        }
    }

    private void UpdateInkImage()
    {
        // Suponiendo que playerInk._reloadInkAmount es 1, 2, 3 según el estado
        int index = Mathf.Clamp(playerInk._reloadInkAmount, 0, inkSprites.Length - 1);
       
        inkImage.sprite = inkSprites[index];
    }
}
