//此脚本处理玩家的攻击能力。该脚本的最大职责是保持攻击冷却的时间，以便玩家不能过快地攻击。通常，这是一个“通过”或“桥接”脚本，这意味着它从
// PlayerInput脚本接收输入，然后将输入传递给适当的攻击。
//脚本中几乎没有攻击逻辑（除了定时）。
//This script handles the player's ability to attack. The biggest responcibility of this script is to maintain the timing of attack cooldowns
//so that the player cannot attack too fast. Mostly, this is a "pass through" or "bridge" script, which means that it receives input from
//the PlayerInput scripts and then passes the input along to the appropriate attack. Very little attack logic (apart from timing) exists in this 
//script. 

using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attacks")]
    //参考闪电攻击脚本
    [SerializeField] LightningAttack lightningAttack; //Reference to a lightning attack script
    //引用霜冻攻击脚本
    [SerializeField] FrostAttack frostAttack;     //Reference to a frost attack script
    //引用恶臭攻击脚本
    [SerializeField] StinkAttack stinkAttack;     //Reference to a stink attack script
    //引用粘液攻击脚本
    [SerializeField] SlimeAttack slimeAttack;     //Reference to a slime attack script
    //玩家的攻击次数
    [SerializeField] int numberOfAttacks;       //The number of attacks the player has

    [Header("UI")]
    //对倒计时滑块的引用
    [SerializeField] Countdown countDown;       //A reference to the countdown slider

    //玩家当前正在使用的攻击的idnex 
    int attackIndex = 0;                //The idnex of the attack the player is currently using
    //玩家必须等待多长时间才能再次攻击
    float attackCooldown = 0f;              //How long the player must wait before attacking again
    //玩家最后被攻击的时间
    float timeOfLastAttack = 0f;            //The time when the player last attacked
    //玩家是否可以攻击
    bool canAttack = true;                //Whether or not the player can attack

    //此方法切换对玩家的主动攻击
    //This method switches the active attack on the player
    public void SwitchAttack()
    {
        //如果玩家无法攻击，请离开
        //If the player can't attack, leave
        if (!canAttack)
            return;
        //增加攻击索引，如果索引过高，则将其设置回0
        //Increase the attack index, then if the index is too high, set it back to 0
        attackIndex++;
        if (attackIndex >= numberOfAttacks)
            attackIndex = 0;

        //关闭所有启用的攻击
        //Turn off all enabled attacks
        DisableAttacks();
        // switch语句基本上是简化的if / else if语句。在这里
        //正在检查attackIndex的值，然后结果在下面的情况中列出
        //The switch statement is basically a streamlined if / else if statement. Here
        //the value of attackIndex is being examined and then the results are listed as
        //cases below
        switch (attackIndex)
        {
            //如果attackIndex的值为0 ...
            //If the value of attackIndex is 0...
            case 0:
                // ...，如果存在闪电攻击，请启用它
                //...and if lightningAttack exists, enable it
                if (lightningAttack != null)
                    lightningAttack.gameObject.SetActive(true);
                break;
            //如果attackIndex的值为1 ...
            //If the value of attackIndex is 1...
            case 1:
                // ...如果存在霜冻攻击，请启用它
                //...and if frostAttack exists, enable it
                if (frostAttack != null)
                    frostAttack.gameObject.SetActive(true);
                break;
            //如果attackIndex的值为2 ...
            //If the value of attackIndex is 2...
            case 2:
                // ...，如果恶臭攻击存在，则启用它
                //...and if stinkAttack exists, enable it
                if (stinkAttack != null)
                    stinkAttack.gameObject.SetActive(true);
                break;
            //如果attackIndex的值为3 ...
            //If the value of attackIndex is 3...
            case 3:
                // ...，如果存在沾液攻击，请启用它
                //...and if slimeAttack exists, enable it
                if (slimeAttack != null)
                    slimeAttack.gameObject.SetActive(true);
                break;
        }
    }

    //只要玩家按下攻击输入就会调用此方法
    //This method is called whenever the player presses the attack input
    public void Fire()
    {
        //如果攻击尚未准备就绪，或者玩家无法进行攻击，请离开
        //If the attack isn't ready, or the player cannot attack, leave
        if (!ReadyToAttack() || !canAttack)
            return;
        //检查AttackIndex的值。请注意，这里我们只处理闪电和
        //霜冻。这是因为，只有当我们松开攻击按钮时，才会发出臭味和粘液攻击。因此，它们在
        // StopFiring（）方法中触发

        //Examing the value of attackIndex. Note that we are only handling lightning
        //and frost here. This is because the stink and slime attacks only fire
        //when we release the attack button. Therefore, they are fired in the 
        //StopFiring() method
        switch (attackIndex)
        {
            //If the value of attackIndex is 0, call ShootLightning()
            case 0:
                ShootLightning();
                break;
            //If the value of attackIndex is 1, call ToggleFrost()
            case 1:
                ToggleFrost(true);
                break;
        }
    }

    //只要玩家释放攻击输入，就会调用此方法
    //This method is called whenever the player releases the attack input
    public void StopFiring()
    {
        //如果攻击尚未准备就绪，或者玩家无法进行攻击，请离开
        //If the attack isn't ready, or the player cannot attack, leave
        if (!ReadyToAttack() || !canAttack)
            return;

        //检查AttackIndex的值。
        //Examing the value of attackIndex. 
        switch (attackIndex)
        {
            //If the value of attackIndex is 1, call ToggleFrost()
            case 1:
                ToggleFrost(false);
                break;
            //If the value of attackIndex is 2, call ShootStink()
            case 2:
                ShootStink();
                break;
            //If the value of attackIndex is 3, call ShootSlime()
            case 3:
                ShootSlime();
                break;
        }
    }

    //句柄告诉雷电起火
    //Handles telling the lightning attack to fire
    void ShootLightning()
    {
        //如果没有闪电攻击，请离开
        //If there is no lightning attack, leave
        if (lightningAttack == null)
            return;

        //Fire lightning
        lightningAttack.Fire();
        //记录闪电攻击的冷却时间
        //Record the cooldown of the lightning attack
        attackCooldown = lightningAttack.Cooldown;
        //记录要等多久才能再次发起攻击
        //record how long to wait before we can attack again
        BeginCountdown();
    }

    //处理切换霜的开关状态。请注意，霜冻攻击没有冷却时间
    //Handles toggling frost on and off. Note that the frost attack has no cooldown
    void ToggleFrost(bool isAttacking)
    {
        //如果没有霜冻攻击，请离开
        //If there is no frost attack, leave
        if (frostAttack == null)
            return;
        //如果将true传递给此方法，请射霜
        //If true is passed into this method, shoot frost
        if (isAttacking)
            frostAttack.Fire();
        //否则，停止射霜
        //Otherwise, stop shooting frost
        else
            frostAttack.StopFiring();
    }

    //句柄告诉发臭蛋的攻击
    //Handles telling the stink attack to fire
    void ShootStink()
    {
        //If there is no stink attack, leave
        if (stinkAttack == null)
            return;
        //射出臭味的弹丸
        //Shoot a stink projectile
        stinkAttack.Fire();
        //记录臭味攻击的冷却时间
        //record the cooldown of the stink attack
        attackCooldown = stinkAttack.Cooldown;
        //记录要等多久才能再次发起攻击
        //record how long to wait before we can attack again
        BeginCountdown();
    }

    //句柄告诉粘液攻击
    //Handles telling the slime attack to fire
    void ShootSlime()
    {
        //如果没有粘液攻击，请离开
        //If there is no slime attack, leave
        if (slimeAttack == null)
            return;
        //尝试发射粘液。如果成功...
        //Attempt to fire slime. If it was successful...
        if (slimeAttack.Fire())
        {
            // ...记录史莱姆攻击的冷却时间...
            //...record the cooldown of the slime attack...
            attackCooldown = slimeAttack.Cooldown;
            // ...并记录要等多久才能再次发起攻击
            //...and record how long to wait before we can attack again
            BeginCountdown();
        }
    }

    bool ReadyToAttack()
    {
        //如果经过了足够的时间，则返回true（玩家可以进行攻击），否则返回false
        //If enough time has passed return true (the player can attack) otherwise return false
        return Time.time >= timeOfLastAttack + attackCooldown;
    }

    //从PlayerHealth脚本调用
    //Called from PlayerHealth script
    public void Defeated()
    {
        //玩家无法攻击
        //Player cannot attack
        canAttack = false;
        //关闭所有攻击
        //Turn off all attacks
        DisableAttacks();
    }

    //此方法设置倒计时，直到玩家再次攻击
    //This method sets the countdown until the player can attack again
    void BeginCountdown()
    {
        //记录当前时间
        //Record the current time
        timeOfLastAttack = Time.time;
        //如果有倒数滑块，请告诉它开始倒数
        //If there is a countdown slider, tell it to begin counting down
        if (countDown != null)
            countDown.BeginCountdown(attackCooldown);
    }

    //此方法关闭所有攻击
    //This method turns off all attacks
    void DisableAttacks()
    {
        //经历每一次攻击，如果存在它的游戏对象，请将其关闭
        //Go through each attack and if a game object for it exists, turn it off
        if (lightningAttack != null)
            lightningAttack.gameObject.SetActive(false);

        if (frostAttack != null)
            frostAttack.gameObject.SetActive(false);

        if (stinkAttack != null)
            stinkAttack.gameObject.SetActive(false);

        if (slimeAttack != null)
            slimeAttack.gameObject.SetActive(false);
    }
}
