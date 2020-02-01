//此脚本处理移动播放器。由于玩家不使用导航代理移动，因此必须进行一些计算才能
//This script handles moving the player. As the player doesn't move using a navmesh agent, some calculations have to be done to
//获得适当的控制级别。
//get the appropriate level of control.

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //玩家应该移动的方向
    [HideInInspector] public Vector3 MoveDirection = Vector3.zero;    //The direction the player should move
    //玩家应该面对的方向
    [HideInInspector] public Vector3 LookDirection = Vector3.forward; //The direction the player should face

    //玩家移动的速度
    [SerializeField] float speed = 6f;                  //The speed that the player moves
    //参考动画器组件
    [SerializeField] Animator animator;                 //Reference to the animator component
    //参考刚体组件
    [SerializeField] Rigidbody rigidBody;               //Reference to the rigidbody component

    //玩家可以移动吗？
    bool canMove = true;                        //Can the player move?

    // Reset（）定义检查器中属性的默认值
    //Reset() defines the default values for properties in the inspector
    void Reset()
    {
        //Grab the needed component references
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }

    //与物理一起移动，因此移动代码位于FixedUpdate（）中
    //Move with physics so the movement code goes in FixedUpdate()
    void FixedUpdate()
    {
        //如果玩家无法移动，请离开
        //If the player cannot move, leave
        if (!canMove)
            return;

        //从所需的移动方向删除任何Y值
        //Remove any Y value from the desired move direction
        MoveDirection.Set(MoveDirection.x, 0, MoveDirection.z);
        //使用其刚体组件的MovePosition（）方法移动播放器。这使玩家比transform.Translate（）更具物理精确度
        //Move the player using the MovePosition() method of its rigidbody component. This moves the player is a more
        //physically accurate way than transform.Translate() does
        rigidBody.MovePosition(transform.position + MoveDirection.normalized * speed * Time.deltaTime);

        //从所需的外观方向移除任何Y值
        //Remove any Y value from the desired look direction
        LookDirection.Set(LookDirection.x, 0, LookDirection.z);
        //使用其刚体组件的MoveRotation（）方法旋转玩家。与transform.Rotate（）相比，这使玩家旋转的方式更物理上准确。我们还使用四元数
        //Rotate the player using the MoveRotation() method of its rigidbody component. This rotates the player is a more
        //physically accurate way than transform.Rotate() does. We also use the LookRotation() method of the Quaternion
        //类的LookRotation（）方法来帮助将欧拉角转换为四元数
        //class to help use convert our euler angles into a quaternion
        rigidBody.MoveRotation(Quaternion.LookRotation(LookDirection));
        //设置动画师的IsWalking参数。如果移动方向有任何大小（量），则玩家正在行走
        //Set the IsWalking paramter of the animator. If the move direction has any magnitude (amount), then the player is walking
        animator.SetBool("IsWalking", MoveDirection.sqrMagnitude > 0);
    }

    //当玩家被击败时调用
    //Called when the player is defeated
    public void Defeated()
    {
        //玩家无法移动
        //Player can no longer move
        canMove = false;
    }
}

