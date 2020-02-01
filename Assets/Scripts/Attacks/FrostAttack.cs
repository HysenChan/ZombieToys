//此脚本处理霜冻攻击。霜冻攻击是一种持续攻击，在其效果范围内对敌人造成霜冻减益。攻击没有冷却时间，而是使用弧形的网状对撞机来确定哪些敌人//
//This script handles the frost attack. The frost attack is a continual attack which places a frost debuff on enemies
//in its cone of effect. The attack has no cooldown and uses a mesh collider in the shape of an arc to determine which 
//enemies are in range

using UnityEngine;

public class FrostAttack : MonoBehaviour
{
    [Header("Weapon Specs")]
    //一次可以冻结多少个敌人。这很有用，因为它限制了需要跟踪的除霜效果的数量，从而减少了滞后
    [SerializeField] int maxFreezableEnemies = 20;    //How many enemies can be freezed at once. This is helpful as it limits the number of frost debuffs that need to be tracked and thus reduces lag

    [Header("Weapon References")]
    //参考霜冻debuff预制
    [SerializeField] GameObject frostDebuffPrefab;    //Reference to the frost debuff prefab
    //引用带有弧形网格对撞机的游戏对象
    [SerializeField] GameObject frostCone;        //Reference to the game object with the arc mesh collider on it
    //引用弧形网格对撞机
    [SerializeField] MeshCollider frostArc;       //Reference to the arc mesh collider

    //已创建的霜冻减益效果数组
    FrostDebuff[] debuffs;                //Array of frost debuffs that have been created
    //当前正在发起攻击吗？
    bool isFiring = false;                //Is the attack currently firing?

    // Reset（）定义检查器中属性的默认值
    //Reset() defines the default values for properties in the inspector
    void Reset()
    {
        //抓取第一个子游戏对象（由于只有一个，因此将成为冰霜锥）
        //Grab the first child game object (which will be the frost cone since there is only one)
        frostCone = transform.GetChild(0).gameObject;
        //从frostCone获取网格对撞机参考
        //Get the mesh collider reference from the frostCone
        frostArc = GetComponentInChildren<MeshCollider>();
    }

    void Awake()
    {
        //创建一个新的FrostDebuffs数组
        //Create a new array of FrostDebuffs 
        debuffs = new FrostDebuff[maxFreezableEnemies];
        //遍历数组...
        //Loop through the array...
        for (int i = 0; i < maxFreezableEnemies; i++)
        {
            // ...创建（实例化）新的游戏对象...
            //...create (instantiate) a new game object...
            GameObject obj = (GameObject)Instantiate(frostDebuffPrefab);
            // ...停用它...
            //...deactivate it...
            obj.SetActive(false);
            // ..然后将其保存到数组中
            //..and then save it into the array
            debuffs[i] = obj.GetComponent<FrostDebuff>();
        }
    }

    //当我们禁用此游戏对象时，调用StopFiring（）方法
    //When we disable this game object, call the StopFiring() method
    void OnDisable()
    {
        StopFiring();
    }

    //从PlayerAttack脚本调用
    //Called from PlayerAttack script
    public void Fire()
    {
        //如果我们当前不射击...
        //If we aren't currently firing...
        if (!isFiring)
        {
            // ...打开frostCone游戏对象并启用网格碰撞器
            //...turn on the frostCone game object and enable the mesh collider
            frostCone.SetActive(true);
            frostArc.enabled = true;

            //我们正在射击。
            //We are now firing.
            isFiring = true;
        }
    }

    //从PlayerAttack脚本和OnDisable（）调用
    //Called from PlayerAttack script and OnDisable()
    public void StopFiring()
    {
        //如果当前未触发，请保留此方法
        //If we aren't currently firing, leave this method
        if (!isFiring)
            return;

        //关闭frostCone游戏对象并禁用对撞机
        //Turn off the frostCone game object and disable the collider
        frostCone.SetActive(false);
        frostArc.enabled = false;

        //We are no longer firing
        isFiring = false;

        //Loop through our array of debuffs and...
        for (int i = 0; i < debuffs.Length; i++)
        {
            //...if the debuff is currently active (which means it is attached to an enemy) tell
            //it to release its enemy
            if (debuffs[i].gameObject.activeInHierarchy)
                debuffs[i].ReleaseEnemy();
        }
    }

    //Attaches a debuff to the specified enemy. This is called from OnTriggerEnter()
    void AttachDebuffToEnemy(EnemyMovement enemy)
    {
        //Loop through our array of debuffs, looking for an inactive one...
        for (int i = 0; i < debuffs.Length; i++)
        {
            //...if this debuff isn't active...
            if (!debuffs[i].gameObject.activeInHierarchy)
            {
                //...activate it and tell it to attach to the specified enemy...
                debuffs[i].gameObject.SetActive(true);
                debuffs[i].AttachToEnemy(enemy);
                //...then return so we don't try to attach more than one
                return;
            }
        }
    }

    //Called when an object enters the arc mesh collider
    void OnTriggerEnter(Collider other)
    {
        //Try to get a reference to an EnemyMovement script from the object that hit the collider
        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        //If there is no EnemyMovement script, then this isn't an enemy and we should leave the method
        if (enemy == null)
            return;

        //If the enemy already has a frost debuff attached, tell that frost debuff
        //to reattach to the enemy (this happens when a frozen enemy leaves and reenters the arc mesh collider)
        if (enemy.FrostDebuff != null)
            enemy.FrostDebuff.AttachToEnemy(enemy);
        //If there isn't already a frost debuff attached, attach a new one
        else
            AttachDebuffToEnemy(enemy);
    }

    //Called when an object leaves the arc mesh collider
    void OnTriggerExit(Collider other)
    {
        //Try to get a reference to an EnemyMovement script from the object that hit the collider
        EnemyMovement enemy = other.GetComponent<EnemyMovement>();
        //If there is no EnemyMovement script, then this isn't an enemy and we should leave the method
        if (enemy == null)
            return;

        //If the enemy already has a frost debuff attached, tell that frost debuff
        //to release the enemy
        if (enemy.FrostDebuff != null)
            enemy.FrostDebuff.ReleaseEnemy();
    }
}
