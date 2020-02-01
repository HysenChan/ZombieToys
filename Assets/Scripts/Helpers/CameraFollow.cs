//此脚本允许摄像机平稳地跟随玩家且不旋转
//This script allows a camera to follow the player smoothly and without rotation

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //要应用于相机运动的平滑量
    [SerializeField] float smoothing = 5f;              //Amount of smoothing to apply to the cameras movement
    //相机与播放器的偏移量（相机应位于播放器的后上方和上方多远）
    [SerializeField] Vector3 offset = new Vector3(-1f, 7f, -10f); //The offset of the camera from the player (how far back and above the player the camera should be)

    // FixedUpdate用于处理基于物理的代码。在此FixedUpdate中不存在物理代码，但是由于玩家的移动代码
    //是在FixedUpdate中处理的，因此我们也在FixedUpdate中移动摄像机，以便它们保持同步
    //FixedUpdate is used to handle physics based code. No physics code exists in this FixedUpdate, but since the player's movement code
    //is handled in FixedUpdate, we are moving the camera in FixedUpdate as well so that they stay in sync
    void FixedUpdate()
    {
        //使用玩家的位置和偏移量来确定相机应该在哪里
        //Use the player's position and offset to determine where the camera should be
        Vector3 targetCamPos = GameManager.Instance.Player.transform.position + offset;
        //使用Lerp平滑地从当前位置移动到所需位置，这对于线性插值来说很短。基本上，它会占用您的位置，想要的位置以及一定的时间，然后告诉您沿该行的位置
        //Smoothly move from the current position to the desired position using a Lerp, which is short
        //for linear interpolation. Basically, it takes where you are, where you want to be, and an amount of time
        //and then tells you where you will be along that line
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}

