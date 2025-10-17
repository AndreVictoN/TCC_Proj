using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class Enemy : Subject, IHealthManager
{
    public Image healthBar;
    public float healthAmount;
    public Animator animator;
    public float myDamage;
    
    public float dodgeChance = 0.3f;

    [SerializeField] private bool _myTurn;
    private bool _isDead;

    void Start()
    {
        Subscribe(GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleManager>());

        if(healthBar == null) healthBar = GameObject.FindGameObjectWithTag("EnemyHealth").GetComponent<Image>();
        if(animator == null) animator = this.gameObject.GetComponent<Animator>();
        _isDead = false;

        if(healthAmount == 0 && (PlayerPrefs.GetString("pastScene") == "PrototypeScene" || (PlayerPrefs.GetString("pastScene").Equals("Floor2") && PlayerPrefs.GetString("currentState").Equals("Start"))))
        {
            healthAmount = 100;
            if (PlayerPrefs.GetString("pastScene").Equals("Floor2")) myDamage = 7;
            else myDamage = 10;
        }else if(PlayerPrefs.GetString("pastScene") == "Class" && PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            healthAmount = 115;
            if (PlayerPrefs.GetString("pastScene").Equals("Floor2")) myDamage = 7;
            else myDamage = 10;
        }
        /*for (int i = 0; i < 10; i++)
        {
            TakeDamage(10);
        }*/
    }

    void Update()
    {
        if(healthAmount <= 0 && !_isDead)
        {
            _isDead = true;
            Notify(EventsEnum.EnemyDead);
            Destroy(this.gameObject);
        }
    }

    public void Attack(PlayerController player)
    {
        StartCoroutine(AttackActions(player));
    }

    IEnumerator AttackActions(PlayerController player)
    {
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("Attack");
        if (PlayerPrefs.GetString("pastScene") == "Class" && PlayerPrefs.GetString("currentState").Equals("GroupClass")) yield return new WaitForSeconds(0.3f);
        else { yield return new WaitForSeconds(1f); }
        player.SetAnimationTrigger("Damage");
        player.TakeDamage(myDamage);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(0.5f);
        player.SetMyTurn(true);
        BattleManager.Instance.SetEnemyIsAttacking(false);
    }

    public void SetMyTurn(bool myTurn){_myTurn = myTurn;}

#region HealthManagement
    public void TakeDamage(float damage)
    {
        /*if (Random.value < dodgeChance)
        {
            Debug.Log("Inimigo desviou!");
            return;
        }*/
        float criticalHitRatio = Random.value;

        if(criticalHitRatio <= 0.1f){ damage *= 2; }

        healthAmount -= damage;
        if (PlayerPrefs.GetString("pastScene") == "Class" && PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            healthBar.fillAmount = healthAmount / 115f;
        }else{ healthBar.fillAmount = healthAmount / 100f; }
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }

    public float GetHealth(){ return healthAmount; }
#endregion

    /*public void RestartScene()
    {
        PlayerPrefs.DeleteAll(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }*/
}
