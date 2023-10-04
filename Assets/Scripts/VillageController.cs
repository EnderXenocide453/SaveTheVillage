using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageController : MonoBehaviour
{
    [SerializeField]
    private Dictionary<ResourceType, Resource> resources;

    [SerializeField]
    private Dictionary<ResourceType, Timer> timers;
}

/// <summary>
/// Класс ресурса. Хранит в себе название, иконку и количество
/// </summary>
[System.Serializable]
public class Resource
{
    public string name = "|Нет имени|";
    public Sprite icon;

    private int _count;

    /// <summary>
    /// Устанавливает количество равное count
    /// </summary>
    /// <param name="count">Количество ресурса</param>
    public void SetCount(int count)
    {
        if (count < 0) {
            _count = 0;
            return;
        }

        _count = count;
    }

    /// <summary>
    /// Возвращает количество ресурса
    /// </summary>
    /// <returns>Количество ресурса</returns>
    public int GetCount()
    {
        return _count;
    }
}

public enum ResourceType
{
    Food,
    Wood,
    Stone,
    Metall,
    People
}