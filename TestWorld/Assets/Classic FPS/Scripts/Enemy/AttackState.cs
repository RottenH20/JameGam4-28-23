

using UnityEngine;
using System.Collections;
//using UnityEngine.Networking.Types;

public class AttackState : IEnemyAI
{

    EnemyStates enemy;
    float timer;

    public AttackState(EnemyStates enemy)
    {
        this.enemy = enemy;
    }

    public void UpdateActions()
    {
        timer += Time.deltaTime;
        float distance = Vector3.Distance(enemy.chaseTarget.transform.position, enemy.transform.position);
        if (distance > enemy.attackRange && enemy.AttackType == EnemyAttackType.Melle)
        {
            ToChaseState();
        }
        if (distance > enemy.shootRange && enemy.AttackType != EnemyAttackType.Melle)
        {
            ToChaseState();
        }

        Watch();

        if (distance <= enemy.shootRange && distance > enemy.attackRange && enemy.AttackType != EnemyAttackType.Melle && timer >= enemy.rangeDelay)
        {
            Attack(true);
            timer = 0;
        }
        if (distance <= enemy.attackRange && timer >= enemy.melleDelay && enemy.AttackType != EnemyAttackType.Range)
        {
            Attack(false);
            timer = 0;
        }
       
    }
     
    void Attack(bool shoot)
    {
        if (shoot == false)
        {
            enemy.chaseTarget.SendMessage("TakeDamage", enemy.meleeDamage, SendMessageOptions.DontRequireReceiver);
            enemy.chaseTarget.SendMessage("DamageIndicatorFunction", enemy.transform, SendMessageOptions.DontRequireReceiver);
            enemy.GetComponent<Enemy>().source.PlayOneShot(enemy.GetComponent<Enemy>().MelleAttackSound[Random.Range(0, enemy.GetComponent<Enemy>().MelleAttackSound.Length)]);
        }
        else if (shoot == true)
        {
            GameObject missile = GameObject.Instantiate(enemy.missile, enemy.vision.position, Quaternion.identity);
            missile.GetComponent<Missile>().speed = enemy.missileSpeed;
            missile.GetComponent<Missile>().damage = enemy.missileDamage;
            missile.GetComponent<Missile>().source = enemy.transform;
            enemy.GetComponent<Enemy>().source.PlayOneShot(enemy.GetComponent<Enemy>().RangeAttackSound[Random.Range(0, enemy.GetComponent<Enemy>().RangeAttackSound.Length)]);
        }
    }

    void Watch()
    {
        if (!enemy.visionScript.EnemySpotted())
        {
            ToAlertState();
        }
    }

    public void OnTriggerEnter(Collider enemy)
    {

    }

    public void ToPatrolState()
    {
        Debug.Log("I shouldn't be able to do this!");
    }

    public void ToAttackState()
    {
        Debug.Log("I shouldn't be able to do this!");
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }

}