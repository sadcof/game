using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    private GameObject player;
    private bool isDeteced;
    public float Speed;
    public Enemy Enemy;
    public float Hp;
    public float Stamin;
    public float Damage;
    public List<int> AvailableBlocks;
    public List<int> AvailableAttacks;
    private bool readyAttack = true;
    private bool readyBlock = true;
    public bool IsBlocked = false;
    private void Awake() 
    {
        Enemy = new Enemy(Hp, Stamin, Damage, AvailableAttacks, AvailableBlocks, Speed);
    }
    private void Update() 
    {
        if (isDeteced)
            MoveToPlayer();
        if (Enemy.State.Hp <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Move()
    {
        
    }

    public void Attack()
    {
        if (readyAttack)
        {
            StartCoroutine(WaitAttack());
        }
    }

    public void BLock()
    {
        StopCoroutine(WaitAttack());
        if (readyBlock)
        {
            StartCoroutine(BlockCD());
            Enemy.State.BlockType = Enemy.AvailableBlocks[Random.Range(0, Enemy.AvailableBlocks.Count)];
            if (Enemy.State.Stamina != 0)
            {
                if (Enemy.State.BlockType == PlayerCont.Player.State.AttackType)
                {
                    Enemy.State.Stamina -= PlayerCont.Player.State.Damage;
                }
                else
                {
                    Enemy.State.Hp -= PlayerCont.Player.State.Damage;
                    Enemy.State.Stamina -= PlayerCont.Player.State.Damage;
                }
            }
            else
            {
                Enemy.State.Hp -= PlayerCont.Player.State.Damage;
            }
        }
    }

    private void MoveToPlayer()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, Time.deltaTime * Enemy.State.Speed);
    }
    
    public void DetectedPlayerTrue(GameObject player)
    {
        this.player = player;
        isDeteced = true;
    }
    public void DetectedPlayerFalse()
    {
        isDeteced = false;
    }

    IEnumerator BlockCD()
    {
        readyBlock = false;
		yield return new WaitForSeconds(1f);
        readyBlock = true;
    }

    IEnumerator WaitAttack()
    {
        readyAttack = false;
        Enemy.State.AttackType = Enemy.AvailableAttacks[Random.Range(0, Enemy.AvailableAttacks.Count)];
        Debug.Log("Attack" + Enemy.State.AttackType);
		yield return new WaitForSeconds(1f);
        if (IsBlocked)
        {
            if (PlayerCont.Player.State.Stamina != 0)
            {
                if (PlayerCont.Player.State.BlockType == Enemy.State.AttackType)
                {
                    PlayerCont.Player.State.Stamina -= Enemy.State.Damage;
                }
                else
                {
                    PlayerCont.Player.State.Hp -= Enemy.State.Damage;
                    PlayerCont.Player.State.Stamina -= Enemy.State.Damage;
                }
            }
            else
            {
                PlayerCont.Player.State.Hp -= Enemy.State.Damage;
            }
        }
        else
        {
            PlayerCont.Player.State.Stamina -= Enemy.State.Damage;
            PlayerCont.Player.State.Hp -= Enemy.State.Damage;
        }
        IsBlocked = false;
        Debug.Log("Stamina" + PlayerCont.Player.State.Stamina);
        Debug.Log("Hp" + PlayerCont.Player.State.Hp);
        yield return new WaitForSeconds(2f);
        readyAttack = true;
    }
}
