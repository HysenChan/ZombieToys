//此脚本控制盟友。盟友是一个简单的实体，一旦生成，便会移动到指定位置
//This script controls the allies. An ally is a simple entity which, once spawned, simply moves to a designated position
using UnityEngine;

public class Ally : MonoBehaviour
{
    //盟军冷却多长时间
    public float Duration;              //How long the ally stays spawned

    //引用盟友的导航代理
    [SerializeField] UnityEngine.AI.NavMeshAgent navMeshAgent;    //A reference to the ally's navmesh agent

    void Reset()
    {
        //获取对navmesh代理的引用
        //Get a reference to the navmesh agent
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void Move(Vector3 point)
    {
        //告诉navmesh代理移动到指定点
        //Tell the navmesh agent to move to the designated point
        navMeshAgent.SetDestination(point);
    }
}
