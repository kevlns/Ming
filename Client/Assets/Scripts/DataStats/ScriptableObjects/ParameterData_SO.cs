using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Params Data", menuName = "Data Stats/Parameter Data")]
public class ParameterData_SO : ScriptableObject
{
    public int health;
    public int maxHealth;
    public int defence;
}
