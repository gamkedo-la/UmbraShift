using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public int currentAP;

    private BaseCharacterClass baseClass;

    // Start is called before the first frame update
    void Start()
    {
        baseClass = GetComponent<BaseCharacterClass>();
        ResetActionPoints();
        TurnManager.instance.ActionManagerReportingForDuty(this);
    }

    public void ResetActionPoints()
    {
        Debug.Log("Reseetintg action points");
        currentAP = baseClass.ActionPointRefill();
    }

    public bool AttemptToSpend(int cost, bool spendIfWeCan)
    {
        if (cost <= currentAP)
        {
            if (spendIfWeCan)
            {
                currentAP -= cost;
                Debug.Log($"Spent {cost} and have {currentAP} remaining.");
            }
            return true;
        }
        Debug.Log("Couldn't afford so didn't remove cost");
        return false;
    }

}
