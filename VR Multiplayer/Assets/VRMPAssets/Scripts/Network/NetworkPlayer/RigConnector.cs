using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigConnector : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] TwoBoneIKConstraint leftArmIK, rightArmIK;

//Attention!
//Because of the template structure's in Unity, You'll see that most of the scripts are in different location
//Because of this specific reason, If you would try to refer to this script in XRINetworkPlayer, you would get error with namespaces
//No time to check right now whether it is important to modify the VRMP.asmdef file to include Assembly-CSharp but at least
//change the script location to the same place if you must 

    public void Setup(Transform leftHandSrc, Transform rightHandSrc, Transform headSrc)
    {
        
        Debug.Log("hallo?");

        //We can use the avatar's animator to add the bone structure to two bone IK constraint
        //Also, DO NOT add the FBX file in the project and make a new prefab out of it, if you will do this
        //Unity will break the link between the avatar and the bonestructure (I presume?) and you will not havve a fun time
       //because while it initially seems everything should be working, it simply doesn't :))
        var leftRoot = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        var leftMid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        var leftTip = animator.GetBoneTransform(HumanBodyBones.LeftHand);

        var rightRoot = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        var rightMid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        var rightTip = animator.GetBoneTransform(HumanBodyBones.RightHand);

        // 2. Wire the IK constraints
        leftArmIK.data.root = leftRoot;
        leftArmIK.data.mid = leftMid;
        leftArmIK.data.tip = leftTip;
        //For the target we'll use the XR origin controllers we get from XRINetworkPlayer
        leftArmIK.data.target = leftHandSrc;
        leftArmIK.weight = 1f;
        leftArmIK.gameObject.SetActive(true);

        //The hint would add some guidance? for where the elbow would be? doesn't seem necessary for now

        rightArmIK.data.root = rightRoot;
        rightArmIK.data.mid = rightMid;
        rightArmIK.data.tip = rightTip;
        rightArmIK.data.target = rightHandSrc;
        rightArmIK.weight = 1f;
        rightArmIK.gameObject.SetActive(true);

        //needs to be done or the rig would not be initiated upon entering the network
        //Offline games you don't have to worry about this but for whatever reason this is not the case
        //With joining on a network
        rigBuilder.Build();

        
    }
}