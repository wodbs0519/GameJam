using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SakuraUiMaker : MonoBehaviour
{
    public SakuraUI[] sakuras;
    public Canvas sakuraUI;
    public EnemyBase targetEnemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            targetEnemy = other.GetComponent<EnemyBase>();
            targetEnemy.sakuraRecovery += RecoverySakura;
            targetEnemy.sakuraCut += CutSakura;
        }
    }

    public void RecoverySakura()
    {
        
    }
    
    public void CutSakura()
    {
        
    }
    

}
