using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageController : MonoBehaviour
{
    #region Ресурсы
    /// <summary>
    /// Начальные ресурсы
    /// </summary>
    public ResoursePair[] initialResources;

    /// <summary>
    /// Источники ресурсов
    /// </summary>
    [SerializeField]
    private ResourceSource[] sources;
    /// <summary>
    /// Текущее количество ресурсов
    /// </summary>
    private Dictionary<ResourceType, int> _resources;

    private int _freePeople;
    #endregion

    private void Start()
    {
        InitResources();
        ConnectTimers();
    }

    public void AddEmployee(ResourceSource src)
    {
        if (_freePeople > 0)
            _freePeople -= src.AddEmployee();
    }

    public void RemoveEmployee(ResourceSource src)
    {
        _freePeople -= src.RemoveEmployee();
    }

    /// <summary>
    /// Инициализация словаря дохода ресурсов
    /// </summary>
    private void InitResources()
    {
        _resources = new Dictionary<ResourceType, int>
        {
            { ResourceType.Food, 0 },
            { ResourceType.Wood, 0 },
            { ResourceType.Metal, 0 },
            { ResourceType.People, 0 }
        };

        if (initialResources != null)
            foreach (ResoursePair res in initialResources)
                AddResource(res.type, res.amount);

        _freePeople = _resources[ResourceType.People];
    }

    private void ConnectTimers()
    {
        foreach (ResourceSource src in sources) {
            src.timer.onLoopEnds += () => AddResource(src.type, src.loopIncome);
        }
    }

    private void AddResource(ResourceType type, int amount)
    {
        SetResource(type, _resources[type] + amount);
    }

    private void SetResource(ResourceType type, int amount)
    {
        //Проверка на наличие ресурса в словаре
        if (!_resources.ContainsKey(type))
            _resources.Add(type, 0);

        if (amount < 0) {
            _resources[type] = 0;
            return;
        }

        _resources[type] = amount;
    }

    [System.Serializable]
    public struct ResoursePair
    {
        public ResourceType type;
        public int amount;
    }
}

public enum ResourceType
{
    Food,
    Wood,
    Metal,
    People
}