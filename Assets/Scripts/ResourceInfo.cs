using TMPro;
using UnityEngine;

public class ResourceInfo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text amountText;

    public void SetAmount(int amount)
    {
        amountText.text = amount.ToString();
    }
}
