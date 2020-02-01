//此脚本处理游戏管理。游戏管理员通常是完全不同的，通常会提供单个游戏可能需要的特定
//This script handles the game management. Game managers are often completely different and generally provide whatever
//多样化服务。在这个项目中，为了使代码易于理解和
//specific and varied services an individual game may need. In this project, in an effort to make the code simple to understand
//模块化，游戏管理器被捆绑到玩家，敌人和盟友的几个核心功能中。即，此管理器
//and modular, the game manager is tied into several core functions of the player, enemies, and allies. Namely, this manager
//跟踪玩家的状态和玩家的状态，处理所有得分，与UI交互，召唤盟友，和
//keeps track of the player and the players state, handles all scoring, interfaces with the UI, summons the allies, and 
//在玩家被击败后重新加载场景
//reloads the scene when the player is defeated

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //此脚本与MouseLocation一样，具有对自身的公共静态引用，以便其他脚本，可以从任何地方访问它而无需查找对其的引用
    public static GameManager Instance;       //This script, like MouseLocation, has a public static reference to itself to that other scripts
                                              //can access it from anywhere without needing to find a reference to it

    [Header("Player and Enemy Properties")]
    //对玩家健康脚本的引用，将其视为“玩家”
    public PlayerHealth Player;           //A reference to the player's health script which will be considered "the player"
    //敌人追逐的对象。这需要与玩家分开，因为游戏管理可以使敌人追逐不是玩家的东西（同盟国一样）
    public Transform EnemyTarget;         //The object that enemies are chasing. This needs to be separate from the player because the game manager
                                          //can make enemies chase something that isn't the player (as is the case with allies)

    //玩家被击败后要等待多长时间
    [SerializeField] float delayOnPlayerDeath = 1f; //How long to wait once the player has been defeated

    [Header("UI Properties")]
    //用于向玩家显示信息的文本UI元素（信息可以是说明或玩家的得分）
    [SerializeField] Text infoText;         //Text UI element to show info to the player (info may be instructions or the player's score)
    //文本用户界面元素，通知玩家他们已经输了
    [SerializeField] Text gameoverText;       //Text UI element informing the player that they have lost

    [Header("Player Selection Properties")]
    //所有敌人生成器的游戏对象父对象
    [SerializeField] GameObject enemySpawners;    //The game object parent of all of the enemy spawners
    //对摄像机动画师的引用（在选择播放器时进行转换）
    [SerializeField] Animator cameraAnimator;   //A reference to the animator on the camera (to transition it when the player is chosen)

    [Header("Ally Properties")]
    //对随附的盟军管理脚本的引用
    [SerializeField] AllyManager allyManager;   //A reference to the attached ally manager script
    //玩家当前得分
    int score = 0;                  //The player's current score

    void Awake()
    {
        //这是通过引用自身来处理类的常用方法。
        //This is a common approach to handling a class with a reference to itself.
        //如果实例变量不存在，则将此对象分配给它
        //If instance variable doesn't exist, assign this object to it
        if (Instance == null)
            Instance = this;
        //否则，如果实例变量确实存在，但不是该对象，则销毁该对象。
        //Otherwise, if the instance variable does exist, but it isn't this object, destroy this object.
        //这很有用，这样我们一次在一个场景中不能有多个GameManager对象。
        //This is useful so that we cannot have more than one GameManager object in a scene at a time.
        else if (Instance != this)
            Destroy(this);
    }

    //在游戏开始时选择了玩家时，由PlayerSelect脚本调用
    //Called by the PlayerSelect script when a player has been selected at the beginning of the game
    public void PlayerChosen(PlayerHealth selected)
    {
        //将玩家记录到Player变量中
        //Record the player into the Player variable
        Player = selected;
        //将敌人的目标设置为玩家的变换
        //Set the target of the enemies to the player's transform
        EnemyTarget = Player.transform;

        //如果信息文本UI元素存在，则告诉它说“得分：0”
        //If the info text UI element exists, tell it to say "Score: 0"
        if (infoText != null)
            infoText.text = "Score: 0";

        //如果存在敌方生成器游戏对象，则启用它
        //If the enemy spawners game object exists, enable it
        if (enemySpawners != null)
            enemySpawners.SetActive(true);

        //如果存在对摄像机动画管理器的引用，则触发Start参数
        //If the reference to the camera's animator exists, trigger the Start parameter
        if (cameraAnimator != null)
            cameraAnimator.SetTrigger("Start");
    }

    //当玩家被击败时从PlayerHealth脚本中调用
    //Called from the PlayerHealth script when the player has been defeated
    public void PlayerDied()
    {
        //敌人不再有目标
        //The enemies no longer have a target
        EnemyTarget = null;

        //如果存在基于文本的游戏UI元素，请将其打开
        //If the game over text UI element exists, turn it on
        if (gameoverText != null)
            gameoverText.enabled = true;
    }

    //在播放完死亡动画后从PlayerHealth脚本调用
    //Called from the PlayerHealth script when the player is done playing their death animation
    public void PlayerDeathComplete()
    {
        //设置延迟后调用ReloadScene（）方法
        //Call the ReloadScene() method after the set delay
        Invoke("ReloadScene", delayOnPlayerDeath);
    }

    //当敌人被击败时从EnemyHealth脚本中调用
    //Called from the EnemyHealth script when an enemy is defeated
    public void AddScore(int points)
    {
        //为玩家的分数加分
        //Add points to the player's score
        score += points;
        //如果信息文本UI元素存在，请更新它以表示玩家的得分
        //If the info text UI element exists, update it to say the player's score
        if (infoText != null)
            infoText.text = "Score: " + score;
        //如果盟友管理器存在，请给玩家一些积分以供玩家花在盟友身上
        //If the ally manager exists, give it some points for the player to spend on an ally
        if (allyManager != null)
            allyManager.AddPoints(points);
    }

    //从PlayerInput脚本和/或“ Ally”按钮的OnClick（）事件调用
    //Called from the PlayerInput scripts and / or the Ally button OnClick() event
    public void SummonAlly()
    {
        //如果没有盟友管理员，请离开
        //If there is no ally manager, leave
        if (allyManager == null)
            return;
        //通过调用SummonAlly（）尝试获得盟友
        //Attempt to get an Ally by calling SummonAlly()
        Ally ally = allyManager.SummonAlly();
        //如果SummonAlly（）能够产生盟友...
        //If SummonAlly() was able to spawn an ally...
        if (ally != null)
        {
            // ...将盟友设为敌人的目标...
            //...set the ally as the target of the enemies...
            EnemyTarget = ally.transform;
            // ...并在设置的延迟后调用UnsummonAlly（）
            //...and call UnsummonAlly() after a set delay
            Invoke("UnSummonAlly", ally.Duration);
        }
    }

    //此方法从场景中移除盟友
    //This method removes the ally from the scene
    void UnSummonAlly()
    {
        //敌人回到追逐玩家
        //Enemies go back to chasing the player
        EnemyTarget = Player.transform;
        //告诉盟友管理员移除盟友
        //Tell the ally manager to remove the ally
        allyManager.UnSummonAlly();
    }

    //此方法在玩家被击败后重新加载场景
    //This method reloads the scene after the player has been defeated
    void ReloadScene()
    {
        //获取对当前场景的引用
        //Get a reference to the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        //告诉SceneManager加载当前场景（将其重新加载）
        //Tell the SceneManager to load the current scene (which reloads it)
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
