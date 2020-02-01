//此脚本跟踪玩家的健康状况。它还用于与GameManager通信
//This script keeps track of the player's health. It is also used to communicate with the GameManager

using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Properties")]
    [SerializeField] int maxHealth = 100;       //Player's maximum health
    [SerializeField] AudioClip deathClip = null;    //Sound clip for the player's death

    [Header("Script References")]
    //参考玩家的动作脚本
    [SerializeField] PlayerMovement playerMovement;   //Reference to the player's movement script
    //参考玩家的攻击脚本
    [SerializeField] PlayerAttack playerAttack;     //Reference to the player's attack script

    [Header("Components")]
    //对动画师组件的引用
    [SerializeField] Animator animator;         //Reference to the animator component
    //对音频源组件的引用
    [SerializeField] AudioSource audioSource;     //Reference to the audio source component	

    [Header("UI")]
    //参考DamageImage UI元素上的FlashFade脚本
    [SerializeField] FlashFade damageImage;       //Reference to the FlashFade script on the DamageImage UI element
    //表示玩家健康状况的滑块
    [SerializeField] Slider healthSlider;       //The slider that will represent the player's health

    [Header("Debugging Properties")]
    //玩家无敌吗？对调试有用，因此玩家不会受到损坏
    [SerializeField] bool isInvulnerable = false;   //Is the player invulnerable? Useful for debugging so the player won't take damage

    //玩家当前的健康状况
    int currentHealth;                  //The current health of the player

    // Reset（）定义检查器中属性的默认值
    //Reset() defines the default values for properties in the inspector
    void Reset()
    {
        //获取对所需组件的引用
        //Grab a reference to the needed components
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Awake()
    {
        //设置玩家的健康
        //Set the player's health
        currentHealth = maxHealth;
    }

    //此方法允许敌人为玩家分配伤害
    //This method allows the enemies to assign damage to the player
    public void TakeDamage(int amount)
    {
        //如果玩家不存在，请离开
        //If the player isn't alive, leave
        if (!IsAlive())
            return;

        //如果玩家无敌，请降低当前生命值
        //If the player is not invulnerable, reduce the current health
        if (!isInvulnerable)
            currentHealth -= amount;

        //如果有损坏的图像，告诉它闪烁
        //If there is a damage image, tell it to flash
        if (damageImage != null)
            damageImage.Flash();

        //如果有健康状况滑块，请更新其值
        //If there is a health slider, update its value
        if (healthSlider != null)
            healthSlider.value = currentHealth / (float)maxHealth;

        //如果玩家已被这次攻击击败...
        //If the player has been defeated by this attack...
        if (!IsAlive())
        {
            // ...如果有玩家移动脚本，告诉它被击败
            //...if there is a player movement script, tell it to be defeated
            if (playerMovement != null)
                playerMovement.Defeated();
            // ...如果有玩家攻击脚本，告诉它被击败
            //...if there is a player attack script, tell it to be defeated
            if (playerAttack != null)
                playerAttack.Defeated();

            // ...在动画器中设置Die参数
            //...set the Die parameter in the animator
            animator.SetTrigger("Die");
            // ...如果有音频源，请为其分配死亡片段
            //...if there is an audio source, assign the deathclip to it
            if (audioSource != null)
                audioSource.clip = deathClip;
            // ...最后，告诉GameManager玩家已被击败
            //...finally, tell the GameManager that the player has been defeated
            GameManager.Instance.PlayerDied();
        }
        //如果有音频源，请告诉它播放
        //If there is an audio source, tell it to play
        if (audioSource != null)
            audioSource.Play();
    }

    public bool IsAlive()
    {
        //如果currentHealth大于0，则返回true（玩家还活着），否则返回false
        //If the currentHealth is above 0 return true (the player is alive), otherwise return false
        return currentHealth > 0;
    }

    //此方法由播放器上Death动画中的事件调用
    //This method is called by an event in the Death animation on the player
    void DeathComplete()
    {
        //如果该玩家是GameManager的注册玩家，请告知GameManager该玩家
        //If this player is the registered player of the GameManager, tell the GameManager that this player
        //已经完成了死亡动画
        //has finished its death animation
        if (GameManager.Instance.Player == this)
            GameManager.Instance.PlayerDeathComplete();
    }
}
