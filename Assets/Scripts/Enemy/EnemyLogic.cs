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
    private float enemyPos;
    public SpriteRenderer EnemySptite;
    public Transform Weapon;
    public Animator WeaponAnim;
    private float staminaCD;
    private void Awake() 
    {
        enemyPos = this.gameObject.transform.position.x;
        Enemy = new Enemy(Hp, Stamin, Damage, AvailableAttacks, AvailableBlocks, Speed);
        StartCoroutine(StaminaRegen());
    }
    private void Update() 
    {
        if(enemyPos > this.gameObject.transform.position.x)
        {
            Weapon.localScale = new Vector3(-1, 1, 1);
            EnemySptite.flipX = true;
            enemyPos = this.gameObject.transform.position.x;
        }
        else if(enemyPos < this.gameObject.transform.position.x)
        {
            Weapon.localScale = new Vector3(1, 1, 1);
            EnemySptite.flipX = false;
            enemyPos = this.gameObject.transform.position.x;
        }
        if (isDeteced)
            MoveToPlayer();
        if (Enemy.State.Hp <= 0)
        {
            Destroy(this.gameObject);
        }
        staminaCD -= Time.deltaTime;
    }

    public void Move()
    {
        
    }

    public void TriggerAttack()
    {
        if (readyAttack)
        {
            staminaCD = 10f;
            StartCoroutine(WaitAttack());
        }
    }

    public void Attack()
    {
        if (IsBlocked)
        {
            if (PlayerCont.Player.State.Stamina > 0)
            {
                if (PlayerCont.Player.State.BlockType != 0)
                {
                    if (PlayerCont.Player.State.BlockType == Enemy.State.AttackType)
                    {
                        PlayerCont.Player.ShowEffect(Color.yellow);
                        Enemy.State.Stamina -= PlayerCont.Player.State.Damage;
                    }
                    else
                    {
                        PlayerCont.Player.ShowEffect(Color.white);
                        PlayerCont.Player.State.Stamina -= Enemy.State.Damage;
                    }
                }
                else
                {
                    PlayerCont.Player.ShowEffect(Color.red);
                    PlayerCont.Player.State.Stamina -= Enemy.State.Damage;
                    PlayerCont.Player.State.Hp -= Enemy.State.Damage;
                }
            }   
            else
            {
                PlayerCont.Player.ShowEffect(Color.red);
                PlayerCont.Player.State.Hp -= Enemy.State.Damage;
            }
        }
        else
        {
            PlayerCont.Player.ShowEffect(Color.red);
            PlayerCont.Player.State.Stamina -= Enemy.State.Damage;
            PlayerCont.Player.State.Hp -= Enemy.State.Damage;
        }
        IsBlocked = false;
    }

    

    public void BLock()
    {
        staminaCD = 10f;
        StopCoroutine("WaitAttack");
        if (readyBlock)
        {
            StartCoroutine(BlockCD());
        }
    }

    IEnumerator StaminaRegen()
    {
        while(true)
        {
            yield return new WaitForSeconds(3f);
            if(Enemy.State.Stamina < Stamin && staminaCD <= 0)
                Enemy.State.Stamina++;
        }
    }

    private void MoveToPlayer()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, player.transform.position, Time.deltaTime * Enemy.State.Speed);
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
        readyAttack = false;
        readyBlock = false;
        Enemy.State.BlockType = Enemy.AvailableBlocks[Random.Range(0, Enemy.AvailableBlocks.Count)];
        if (Enemy.State.Stamina > 0)
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
		yield return new WaitForSeconds(0.2f);
        readyAttack = true;
        yield return new WaitForSeconds(0.8f);
        readyBlock = true;
    }


    private void AnimAttackCont()
    {
        if(Enemy.State.AttackType == 1)
        {
            if (!EnemySptite.flipX)
                WeaponAnim.SetTrigger("UpAttack");
            else
                WeaponAnim.SetTrigger("UpAttackFlip");
        }
        else if(Enemy.State.AttackType == 2)
        {
            if (!EnemySptite.flipX)
                WeaponAnim.SetTrigger("DownAttack");
            else
                WeaponAnim.SetTrigger("DownAttackFlip");
        }
        else if(Enemy.State.AttackType == 3)
        {
            if (!EnemySptite.flipX)
                WeaponAnim.SetTrigger("MidAttack");
            else
                WeaponAnim.SetTrigger("MidAttackFlip");
        }
    }

    public IEnumerator WaitAttack()
    {
        readyAttack = false;
        Enemy.State.AttackType = Enemy.AvailableAttacks[Random.Range(0, Enemy.AvailableAttacks.Count)];
		yield return new WaitForSeconds(1f);
        PlayerCont.Player.RegenCD = 10f;
        AnimAttackCont();
        yield return new WaitForSeconds(1f);
        readyAttack = true;
    }
}
