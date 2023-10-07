using UnityEngine;
using UnityEngine.UI;

public class ResourceInfo : MonoBehaviour
{
    [SerializeField]
    private Text amountText;

    public void SetAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}
