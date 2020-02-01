//此脚本处理了独立平台（带有键盘和鼠标的平台）上的播放器输入。
//This script handled player input on standalone platforms (platforms with a keyboard and mouse). This script will 
//如果该移动对象是为移动设备构建的，则此脚本将//禁用自身
//disable itself if the porject is built for mobile

using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputPC : MonoBehaviour
{
    //参考玩家的动作脚本
    [SerializeField] PlayerMovement playerMovement = null;  //Reference to the player's movement script
    //参考玩家的攻击脚本
    [SerializeField] PlayerAttack playerAttack = null;    //Reference to the player's attack script
    //参考暂停菜单
    [SerializeField] PauseMenu pauseMenu;         //Reference to the pause menu

    // Reset（）定义检查器中属性的默认值
    //Reset() defines the default values for properties in the inspector
    void Reset()
    {
        //获取所需的组件引用
        //Grab the needed component references
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
        //在场景中找到PauseMenu脚本的实例
        //Find an instance of the PauseMenu script in the scene
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    //如果这是一个移动平台，则将启用第32至35行，此脚本会将其从播放器中删除
    //If this is a mobile platform, lines 32 through 35 will be enabled and this script will remove itself from the player
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
	void Awake()
	{
		Destroy(this);
	}
#endif

    void Update()
    {
        //如果有暂停菜单，并且玩家按下了取消输入轴，则暂停游戏
        //If there is a pause menu and the player presses the Cancel input axis, pause the game
        if (pauseMenu != null && Input.GetButtonDown("Cancel"))
            pauseMenu.Pause();
        //如果玩家无法更新，请离开
        //If the player cannot update, leave
        if (!CanUpdate())
            return;
        //处理行动，攻击和盟友的输入
        //Handle inputs for movement, attacking, and allies
        HandleMoveInput();
        HandleAttackInput();
        HandleAllyInput();
    }

    bool CanUpdate()
    {
        //如果游戏暂停，玩家将无法更新
        //If the game is paused, the player cannot update
        if (pauseMenu != null && pauseMenu.IsPaused)
            return false;
        //如果此玩家不是分配给GameManager的玩家，则该玩家无法更新
        //If this player isn't the player assigned to the GameManager, then this player cannot update
        if (GameManager.Instance.Player == null || GameManager.Instance.Player.transform != transform)
            return false;
        //如果以上两个陈述不正确，则播放器可以更新
        //If the above two statements aren't true, then the player can update
        return true;
    }

    void HandleMoveInput()
    {
        //如果没有移动脚本，请离开
        //If there is no movement script, leave
        if (playerMovement == null)
            return;
        //获取原始的水平和垂直输入（原始输入未应用平滑）
        //Get the raw Horizontal and Vertical inputs (raw inputs have no smoothing applied)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        //告诉运动脚本在X和Z轴上移动，而Y轴不移动
        //Tell the movement script to move on the X and Z axes with no Y axis movement
        playerMovement.MoveDirection = new Vector3(horizontal, 0, vertical);
        //如果有一个MouseLocation脚本并且鼠标的位置有效...
        //If there is a MouseLocation script and the mouse's position is valid...
        if (MouseLocation.Instance != null && MouseLocation.Instance.IsValid)
        {
            //通过从鼠标的位置减去玩家的位置来找到玩家应该看的点
            //Find the point the player should look at by subtracting the player's position from the mouse's position
            Vector3 lookPoint = MouseLocation.Instance.MousePosition - playerMovement.transform.position;
            //告诉玩家看哪个方向
            //Tell the player what direction to look
            playerMovement.LookDirection = lookPoint;
        }
    }

    void HandleAttackInput()
    {
        //如果没有攻击脚本，请离开
        //If there is no attack script, leave
        if (playerAttack == null)
            return;

        //如果玩家按下切换攻击输入轴，则告诉攻击脚本切换武器
        //If the player presses the SwitchAttack input axis, tell the attack script to switch weapons
        if (Input.GetButtonDown("SwitchAttack"))
        {
            playerAttack.SwitchAttack();
        }
        //如果玩家按下（或按住）Fire1，则开始射击
        //If the player presses (or holds) Fire1, start firing
        if (Input.GetButton("Fire1"))
        {
            playerAttack.Fire();
        }
        //否则，停止发射
        //Otherwise, stop firing
        else if (Input.GetButtonUp("Fire1"))
        {
            playerAttack.StopFiring();
        }
    }

    void HandleAllyInput()
    {
        //如果玩家按下召唤输入轴并且有一个GameManager，请告诉GameManager召唤一个盟友
        //If the player presses the SummonAlly input axis and there is a GameManager, tell the GameManager to summon an ally
        if (Input.GetButtonDown("SummonAlly") && GameManager.Instance != null)
            GameManager.Instance.SummonAlly();

    }
}

