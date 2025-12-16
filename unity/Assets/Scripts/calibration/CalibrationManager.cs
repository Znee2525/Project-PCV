using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CalibrationManager : MonoBehaviour
{
    private Dictionary<HumanBodyBones, Quaternion> offsets =
        new Dictionary<HumanBodyBones, Quaternion>();

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (!animator || !animator.isHuman)
        {
            Debug.LogError("❌ Animator not found or avatar is not Humanoid");
        }
    }

    void Update()
    {
        // New Input System (press C)
        if (Keyboard.current != null &&
            Keyboard.current.cKey.wasPressedThisFrame)
        {
            Calibrate();
        }
    }

    public void Calibrate()
    {
        if (!animator) return;

        offsets.Clear();

        foreach (HumanBodyBones b in System.Enum.GetValues(typeof(HumanBodyBones)))
        {
            if (b == HumanBodyBones.LastBone) continue;

            Transform t = animator.GetBoneTransform(b);
            if (!t) continue;

            offsets[b] = Quaternion.Inverse(t.localRotation);
        }

        Debug.Log("✅ Calibration completed (" + offsets.Count + " bones)");
    }

    public Quaternion ApplyOffset(HumanBodyBones bone, Quaternion incomingRot)
    {
        if (!offsets.ContainsKey(bone))
            return incomingRot;

        // incoming * offset = corrected rotation
        return incomingRot * offsets[bone];
    }

    // Optional helper
    public bool IsCalibrated()
    {
        return offsets.Count > 0;
    }
}
