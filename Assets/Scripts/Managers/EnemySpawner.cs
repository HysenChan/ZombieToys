//This script controls the spawning of enemies. The enemy spawner is a "pooled" spawner. A "pool" is effectively a collection of objects
//that are generally disabled. Then when they are needed, an object is enabled, used, and then disabled again when it is done. This is in
//contrast to a system where we intantiate and destroy objects when we need them and need to get rid of them. Instantiating and destroying 
//can cause lag as well as memory spikes which are both bad things. This script also has a maximum number of enemies it can spawn to prevent
//the scene from being flooded with enemies which makes the game very difficult and can cause lag / crashes.

//此脚本控制敌人的产生。敌方生成器是“池式”生成器。“池”实际上是通常禁用的对象的集合。
//然后，在需要它们时，将启用，使用对象，然后在完成后再次将其禁用。这与
//我们需要并需要摆脱它们的对象进行实例化和销毁的系统形成对比。实例化和破坏
//会导致延迟以及内存峰值，这都是不好的事情。此脚本还具有可以生成的最大敌人数量，以防止
//场景被敌人淹没，这使游戏非常困难并可能导致滞后崩溃。

using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Properties")]
    //产生敌人的预制件
    [SerializeField] GameObject enemyPrefab;  //The enemy prefab to spawn
    //以秒为单位的速率产生敌人
    [SerializeField] float spawnRate = 5f;    //Rate, in seconds, to spawn enemies
    //此生成器一次可以拥有的最大敌人数
    [SerializeField] int maxEnemies = 10;   //Maximum number of enemies that this spawner can have at a time

    [Header("Debugging Properties")]
    //此生成器可以生成敌人吗？当您想关闭生成器时，这对于测试很有用
    [SerializeField] bool canSpawn = true;    //Can this spawner spawn enemies? This is useful for testing when you want to turn a spawner off

    //汇集敌人的数组
    EnemyHealth[] enemies;            //An array of the pooled enemies
    //尝试产生敌人之间的延迟
    WaitForSeconds spawnDelay;          //The delay between attempts to spawn an enemy

    void Awake()
    {
        //创建一个数组来存储敌人池
        //Create an array to store the pool of enemies
        enemies = new EnemyHealth[maxEnemies];
        //遍历数组并...
        //Loop through the array and...
        for (int i = 0; i < maxEnemies; i++)
        {
            // ...通过预制实例化敌方游戏对象...
            //...instantiate an enemy game object from a prefab...
            GameObject obj = (GameObject)Instantiate(enemyPrefab);
            // ...获取对其EnemyHealth脚本的引用...
            //...get a reference to its EnemyHealth script...
            EnemyHealth enemy = obj.GetComponent<EnemyHealth>();
            // ...将此游戏对象作为父对象...
            //...parent it to this gamn object...
            obj.transform.parent = transform;
            //...disable it...
            // ...禁用它...
            obj.SetActive(false);
            // ...最后，将敌人的健康脚本存储在池中
            //...finally, store the enemy's health script in the pool
            enemies[i] = enemy;
        }
        //Create the WaitForSeconds variable that will be used to delay spawning
        //创建将用于延迟生成的WaitForSeconds变量
        spawnDelay = new WaitForSeconds(spawnRate);
    }

    //这是Start（）的有趣用法，其中Start（）方法本身
    //被用作协程。您可以让Start（）方法运行不同的协程
    //以达到相同的效果，但了解使用
    //这样的Start（）方法是很有用的
    //This is an interesting use of Start() where the Start() method itself is
    //used as a coroutine. You could have the Start() method run a different coroutine to
    //achieve the same effect, but it's useful to know that using the Start() method like
    //this is possible
    IEnumerator Start()
    {
        //当生成器可以生成...
        //While the spawner can spawn...
        while (canSpawn)
        {
            // ...等待指定的延迟...
            //...wait the specified delay...
            yield return spawnDelay;
            // ...然后生成一个敌人，然后再次循环
            //...and then spawn an enemy before looping again
            SpawnEnemy();

            //如果您想知道，可以交换55和57行。
            //这样做将导致游戏开始时立即产生一个敌人。
            //现在情况如此，但是生成器首先等待，这使
            // player可以自己定位
            //In case you are wondering, you could swap lines 55 and 57. Doing
            //so would cause an enemy to be immediately spawned when the game starts.
            //The way it is now, however, the spawner waits first which gives the 
            //player a chance to orient themselves
        }
    }

    //此方法通过在池中搜索可用的敌人并启用它来将敌人生成到场景中。毫无疑问，创建一个系统效率更高，
    //我们不必在池中搜索可用的敌人，而是将池中所有不可用的敌人撤出池。但是，它
    //以这种方式构造，试图使代码尽可能简单和干净。而且，池的大小非常小，因此两种方法的效率差异可以忽略不计
    //This method spawns an enemy into the scene by searching the pool for an available enemy
    //and enabling it. It's worth nothing that it would be more efficient to create a system
    //where we didn't have to search the pool for an available enemy and instead pulled any
    //enemies that weren't available out of the pool. It is constructed this way, however, in 
    //an attempt to keep the code as simple and clean as possible. Also, the size of the pools are
    //very small, so the difference in efficiency between the two ways of doing this is negligable
    void SpawnEnemy()
    {
        //在敌人池中循环
        //Loop through the pool of enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            //如果当前敌人可用（未激活）...
            //If the current enemy is available (not active)...
            if (!enemies[i].gameObject.activeSelf)
            {
                // ...使用生成器对其进行定向...
                //...orient it with the spawner...
                enemies[i].transform.position = transform.position;
                enemies[i].transform.rotation = transform.rotation;
                // ...启用它...
                //...enable it...
                enemies[i].gameObject.SetActive(true);
                // ...并保留此方法，以免意外产生更多敌人
                //...and leave this method so it doesn't accidently spawn more enemies
                return;
            }
        }
    }
}
