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
/// ����� �������. ������ � ���� ��������, ������ � ����������
/// </summary>
[System.Serializable]
public class Resource
{
    public string name = "|��� �����|";
    public Sprite icon;

    private int _count;

    /// <summary>
    /// ������������� ���������� ������ count
    /// </summary>
    /// <param name="count">���������� �������</param>
    public void SetCount(int count)
    {
        if (count < 0) {
            _count = 0;
            return;
        }

        _count = count;
    }

    /// <summary>
    /// ���������� ���������� �������
    /// </summary>
    /// <returns>���������� �������</returns>
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