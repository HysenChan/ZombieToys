//This script tracks the location of the mouse in 3D space. It does this by taking the location
//of the mouse on the screen (2D space). It then draws a line from the camera, through the mouse's 
//position on the screen into the world. Finally, it determines where this line hits a collider in
//the 3D scene. 

//此脚本跟踪鼠标在3D空间中的位置。它通过
//在屏幕上鼠标的位置（// 2D空间）来实现。然后，它通过屏幕上鼠标的
//位置从相机画一条线，进入世界。最后，它确定这条线在3D场景中撞到碰撞器的位置。

//This line of code is special and its purpose is to disable warning 0414 in this script. That warning
//writes to the console and tells you that this script has a variable that is created but never used. The IDE
//thinks we don't use the variable isTouchAiming because it is wrapped in platform specific code. Therefore
//when we are on PC we don't use that variable, but when we are mobile we do. Instead of having this warning
//constantly in the console window, this line simply turns that warning off (for this script only)

//此代码行很特殊，目的是禁用此脚本中的警告0414。该警告
//将写入控制台，并告诉您该脚本具有已创建但从未使用过的变量。IDE 
//认为我们不使用变量isTouchAiming，因为它包装在特定于平台的代码中。因此
//当我们在PC上时，我们不会使用该变量，但是当我们在移动设备上时，会使用该变量。此行不是
//在控制台窗口中始终显示此警告，而是仅关闭了该警告（仅针对此脚本）

#pragma warning disable 0414
using UnityEngine;

public class MouseLocation : MonoBehaviour
{
    //对MouseLacation对象的引用。这允许该类对其自身进行公共引用，以便其他脚本可以不对其进行引用而访问它。
    public static MouseLocation Instance;     //A reference to a MouseLacation object. This allows the class to have a public reference to itself to other scripts can 
                                              //access it without having a reference to it. 

    //鼠标光标在3D空间中的位置
    [HideInInspector] public Vector3 MousePosition; //Location in 3D space of the mouse cursor	
    //鼠标位置有效吗？
    [HideInInspector] public bool IsValid;      //Is the mouse location valid?

    //一个LayerMask，指示在确定鼠标位置时被认为是地面
    [SerializeField] LayerMask whatIsGround;    //A LayerMask indicating what is considered to be ground when determining the mouse's location

    //用于寻找鼠标的光线
    Ray mouseRay;                 //A ray that will be used to find the mouse
    //一个射线广播命中，它将存储有关raycast的信息
    RaycastHit hit;                 //A RaycastHit which will store information about a raycast
    //鼠标在屏幕上的位置
    Vector2 screenPosition;             //Where the mouse is on the screen
    //我们是否使用触摸来瞄准？如果我们在移动设备上，将使用此功能
    bool isTouchAiming;               //Are we using touch to aim? This will be used if we are on a mobile device

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

    void Update()
    {
        //假设鼠标位置无效
        //Assume the mouse location isn't valid
        IsValid = false;

        //This is platform specific code. Any code that isn't in the appropriate section
        //is effectively turned into a comment (essentialy doesn't exist when the project is built).
        //If this is a mobile platform (Android, iOS, or WP8)... 
        //这是平台特定的代码。不在适当部分的所有代码都被有效地转换为注释（构建项目时本质上不存在）。
        //如果这是一个移动平台（Android，iOS或WP8）...
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
        // ...如果未使用触摸瞄准，请离开
				//...and if it isn't using touch aiming, leave
				if (!isTouchAiming)
					return;
#else
        // ...否则，将鼠标的位置记录到screenPosition变量中
        //...otherwise, record the mouse's position to the screenPosition variable
        screenPosition = Input.mousePosition;
#endif
        //创建从主相机延伸到屏幕上鼠标位置的光线进入场景
        //Create a ray that extends from the main camera, through the mouse's position on the screen
        //into the scene
        mouseRay = Camera.main.ScreenPointToRay(screenPosition);

        //如果来自我们相机的光线撞击到地面上的东西...
        //If the ray from our camera hits something that is ground...
        if (Physics.Raycast(mouseRay, out hit, 100f, whatIsGround))
        {
            // ...鼠标位置有效...
            //...the mouse position is valid...
            IsValid = true;
            // ...并记录3D空间中射线撞击地面的点
            //...and record the point in 3D space that the ray hit the ground
            MousePosition = hit.point;
        }
    }

    //此方法开始触摸瞄准（将从触摸板脚本中调用）
    //This method starts touch aiming (will be called from the Touchpad script)
    public void StartTouchAim(Vector2 position)
    {
        //我们现在正在瞄准
        //We are now touch aiming
        isTouchAiming = true;
        //记录屏幕触摸的位置
        //Record the position of the screen touch
        screenPosition = position;
    }

    //此方法告诉M​​ouseLocation脚本玩家正在触摸的位置（将从触摸板脚本中调用）
    //This method tells the MouseLocation script where the player is touching (will be called from the Touchpad script)
    public void UpdatePosition(Vector2 position)
    {
        screenPosition = position;
    }

    //此方法停止触摸瞄准（将从触摸板脚本中调用）
    //This method stops touch aiming (will be called from the Touchpad script)
    public void StopTouchAim()
    {
        //我们不再碰瞄准
        //We are no longer touch aiming
        isTouchAiming = false;
    }
}
