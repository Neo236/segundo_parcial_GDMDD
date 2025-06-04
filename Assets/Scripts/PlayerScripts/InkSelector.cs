using UnityEngine;

public class InkSelector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ElementType[] availableInks;
    public ElementType currentInk;

    private int currentInkIndex = 0;
    private void Awake()
    {
        if (availableInks.Length > 0)
        {
            SelectInk(0);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SelectInk(int index)
    {
        if (index >= 0 && index < availableInks.Length)
        {
            currentInkIndex = index;
            currentInk = availableInks[index];
            Debug.Log("Selected Ink: " + currentInk);
        }
        else
        {
            Debug.LogWarning("Invalid ink index: " + index);
        }
    }
    public void SelectNextInk()
    {
        int nextIndex = (currentInkIndex + 1) % availableInks.Length;
        SelectInk(nextIndex);
    }
}
