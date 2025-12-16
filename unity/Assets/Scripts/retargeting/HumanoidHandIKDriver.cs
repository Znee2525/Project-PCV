using UnityEngine;

public class HumanoidHandIKDriver : MonoBehaviour
{
    Animator animator;

    Transform leftShoulder;
    Transform rightShoulder;

    float leftArmLength;
    float rightArmLength;

    Vector3 smoothLeftDir;
    Vector3 smoothRightDir;

    Vector3 smoothLeftPos;
    Vector3 smoothRightPos;

    Quaternion smoothLeftRot = Quaternion.identity;
    Quaternion smoothRightRot = Quaternion.identity;

    [Header("Smoothing")]
    public float directionSmooth = 15f;
    public float positionSmooth = 18f;
    public float rotationSmooth = 20f;

    void Start()
    {
        animator = GetComponent<Animator>();

        leftShoulder = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

        Transform lElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        Transform lHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);

        Transform rElbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        Transform rHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        leftArmLength =
            Vector3.Distance(leftShoulder.position, lElbow.position) +
            Vector3.Distance(lElbow.position, lHand.position);

        rightArmLength =
            Vector3.Distance(rightShoulder.position, rElbow.position) +
            Vector3.Distance(rElbow.position, rHand.position);
    }

    void OnAnimatorIK(int layerIndex)
    {
        var p = UDPReceiver.latestPose;
        if (p == null) return;

        ApplyArmIK(
            AvatarIKGoal.LeftHand,
            leftShoulder,
            p.left_upper_arm,
            leftArmLength,
            false,
            ref smoothLeftDir,
            ref smoothLeftPos,
            ref smoothLeftRot
        );

        ApplyArmIK(
            AvatarIKGoal.RightHand,
            rightShoulder,
            p.right_upper_arm,
            rightArmLength,
            false,
            ref smoothRightDir,
            ref smoothRightPos,
            ref smoothRightRot
        );
    }

    void ApplyArmIK(
        AvatarIKGoal goal,
        Transform shoulder,
        float[] dir,
        float armLength,
        bool mirror,
        ref Vector3 smoothDir,
        ref Vector3 smoothPos,
        ref Quaternion smoothRot
    )
    {
        if (dir == null || dir.Length != 3) return;

        Vector3 d = new Vector3(
            dir[0],
           -dir[1],
            dir[2]
        );

        if (mirror) d.x *= -1f;
        if (d.sqrMagnitude < 0.0001f) return;

        d.Normalize();

        if (smoothDir == Vector3.zero)
            smoothDir = d;

        smoothDir = Vector3.Lerp(
            smoothDir,
            d,
            Time.deltaTime * directionSmooth
        );

        Vector3 targetPos =
            shoulder.position + smoothDir * armLength * 0.85f;

        if (smoothPos == Vector3.zero)
            smoothPos = targetPos;

        smoothPos = Vector3.Lerp(
            smoothPos,
            targetPos,
            Time.deltaTime * positionSmooth
        );

        Quaternion targetRot =
            Quaternion.LookRotation(smoothDir, Vector3.up);

        if (smoothRot == Quaternion.identity)
            smoothRot = targetRot;

        smoothRot = Quaternion.Slerp(
            smoothRot,
            targetRot,
            Time.deltaTime * rotationSmooth
        );

        animator.SetIKPositionWeight(goal, 1f);
        animator.SetIKRotationWeight(goal, 0.8f);

        animator.SetIKPosition(goal, smoothPos);
        animator.SetIKRotation(goal, smoothRot);
    }
}
