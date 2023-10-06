using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageController : MonoBehaviour
{
    #region Ресурсы
    [SerializeField]
    private Resource food;
    [SerializeField]
    private Resource wood;
    [SerializeField]
    private Resource metal;
    
    private int people;
    #endregion

    private void Start()
    {
        InitResources();
    }

    public void AddEmployee(ResourceSource src)
    {
        people += src.AddEmployee();
    }

    public void RemoveEmployee(ResourceSource src)
    {
        people += src.RemoveEmployee();
    }

    /// <summary>
    /// Инициализация словаря дохода ресурсов
    /// </summary>
    private void InitResources()
    {
        food.Init();
        wood.Init();
        metal.Init();
    }

    [System.Serializable]
    public class Resource
    {
        public ResourceSource source;
        public ResourceInfo ui;
        public TMPro.TMP_Text employeeField;
        public int initialAmount;

        public int curAmount { get; private set; }

        public void Init()
        {
            curAmount = initialAmount;
            UpdateUI();

            source.timer.onLoopEnds += () => AddResource(source.loopIncome);
            source.onEmployeesChanged += () => { employeeField.text = source.employeesCount.ToString(); };
        }

        public void AddResource(int amount)
        {
            SetResource(curAmount + amount);
        }

        private void SetResource(int amount)
        {
            amount = Mathf.Clamp(amount, 0, 9999);

            curAmount = amount;
            UpdateUI();
        }

        private void UpdateUI()
        {
            ui.SetAmount(curAmount);
        }
    }
}