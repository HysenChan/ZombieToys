//此脚本控制盟友的生成功能。它跟踪玩家拥有的可花费的“点数”以及
//This script controls the ally spawning functionality. It keeps track of the number of spendable "points" the player has as well as
//已生成盟友的当前状态。请注意，此脚本中的盟友点与游戏
//the current state of a spawned ally. Note that points for allies in this script are different from the points stored in the Game 
// Manager中存储的点不同。最后，此脚本负责了解是否可以生成同盟并显示适当的UI图像。
//Manager. Finally, this script is responsible for know if an ally can be spawned and showing the appropriate UI image

using UnityEngine;
using UnityEngine.UI;

public class AllyManager : MonoBehaviour
{
    //盟友召唤的点数
    [SerializeField] int allyCost;        //The amount of points an ally costs to summon
    //召唤盟友的预制件
    [SerializeField] GameObject allyPrefab;   //The prefab of the ally to be summoned
    //召唤盟友的地方
    [SerializeField] Transform allySpawnPoint;  //Where the ally should be summoned
    //参考UI图像，使玩家知道一个盟友可用
    [SerializeField] Image allyImage;     //A reference to the UI image that lets the player know an ally is available

    //引用任何当前产生的盟友
    Ally ally;                  //A reference to any currently spawned ally
    //玩家目前必须在盟友身上花费多少点
    int allyPoints;               //How many points the player currently has to spend on allies

    void Awake()
    {
        //从实例化实例开始一个盟友游戏对象
        //Start by instantiating an ally game object from a prefab
        GameObject obj = (GameObject)Instantiate(allyPrefab);
        //然后将其作为该游戏对象的父对象
        //Then parent it to this game object
        obj.transform.parent = transform;
        //然后获取对其Ally脚本的引用
        //Then grab a reference to its Ally script
        ally = obj.GetComponent<Ally>();
        //最后将其禁用，以便以后使用
        //Finally disable it so it is ready to be used later
        obj.SetActive(false);
        //如果存在allyImage，请将其禁用
        //If the allyImage exists, disable it
        if (allyImage != null)
            allyImage.enabled = false;
    }

    //从GameManager脚本中调用，以赋予AllyManager一些要花费在盟友上的点
    //Called from the GameManager script to give the AllyManager some points to
    //spend on allies
    public void AddPoints(int amount)
    {
        //增加点数
        //Add the amount of points
        allyPoints += amount;

        //如果有一个allyImage并且玩家可以召唤一个盟友，则显示该图片
        //If there is an allyImage and the player can summon an ally, show the image
        if (allyImage != null && CanSummonAlly())
            allyImage.enabled = true;
    }

    public bool CanSummonAlly()
    {
        //如果玩家有足够的点数，并且场景中当前没有盟友，则返回
        //If the player has enough points and there isn't currently an ally in the scene, return 
        // true（可以生成一个盟友），否则返回false
        //true (an ally can be spawned), otherwise return false
        return (allyPoints >= allyCost) && !ally.gameObject.activeSelf;
    }

    //此方法将盟友召唤到场景中，并返回对盟友的引用
    //This method summons the ally into the scene and returns a reference 
    //to the ally
    public Ally SummonAlly()
    {
        //如果无法召唤一个盟友，则返回null值（无）
        //If an ally can't be summoned, return a value of null (nothing)
        if (!CanSummonAlly())
            return null;

        //将盟军的位置和旋转与生成点对齐
        //Align the ally's position and rotation with the spawn point
        ally.transform.position = allySpawnPoint.position;
        ally.transform.rotation = allySpawnPoint.rotation;
        //启用盟军并告诉其向敌人的目标移动（应该是玩家）
        //Enable the ally and tell it to move towards the enemy's target (which
        //should be the player)
        ally.gameObject.SetActive(true);
        ally.Move(GameManager.Instance.EnemyTarget.position);

        //如果有allyImage，请将其关闭
        //If there is an allyImage, turn it off
        if (allyImage != null)
            allyImage.enabled = false;
        //将对盟友的引用返回给调用者
        //Return a reference to the ally back to the caller
        return ally;
    }

    //此方法关闭盟友
    //This method turns the ally off
    public void UnSummonAlly()
    {
        //盟友离开后，移除所有盟友点。这样可以防止玩家
        //Once the ally goes away, remove all ally points. This is the prevent the 
        //积累大量盟友积分，然后有效地
        //player from banking a large amount of ally points and then effectively 
        //召集盟友（这既不好玩又不具挑战性）
        //chain summoning allies (which would not be fun or challenging)
        allyPoints = 0;
        //关闭盟友
        //Turn the ally off
        ally.gameObject.SetActive(false);
    }
}
