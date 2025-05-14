
using System.Collections;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using XRMultiplayer;

public class RigConnector : NetworkBehaviour
{
    protected Transform m_LeftHandOrigin, m_RightHandOrigin;
    [SerializeField] TwoBoneIKConstraint leftTBIKC;
    [SerializeField] TwoBoneIKConstraint rightTBIKC;
    [SerializeField] TwoBoneIKConstraint leftArmIK;
    [SerializeField] TwoBoneIKConstraint rightArmIK;
    [SerializeField] Animator animator;
    [SerializeField] RigBuilder rigBuilder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) { return; }
        animator = GetComponentInParent<Animator>();
        leftTBIKC.data.target = XRINetworkPlayer.LocalPlayer.m_LeftHandOrigin;
        rightTBIKC.data.target = XRINetworkPlayer.LocalPlayer.m_RightHandOrigin;



        StartCoroutine(StartRig());
    }

    private IEnumerator StartRig()
    {
        yield return new WaitForSeconds(1f);

        yield return new WaitUntil(() => animator.isInitialized && animator.avatar != null && animator.avatar.isHuman);

        Debug.Log("should be built");


        if (animator != null)
        {
            var leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);

            leftTBIKC.data.root = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            leftTBIKC.data.mid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            leftTBIKC.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);

            rightTBIKC.data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            rightTBIKC.data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            rightTBIKC.data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);


        }
        else
        {
            Debug.Log("uh oh");
        }
        rigBuilder.Build();
    }

}