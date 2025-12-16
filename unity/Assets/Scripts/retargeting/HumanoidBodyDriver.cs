using UnityEngine;

public class HumanoidBodyDriver : MonoBehaviour
{
    Animator animator;

    Transform hips;
    Transform spine;
    Transform chest;
    Transform head;

    Quaternion hipsBind;
    Quaternion spineBind;
    Quaternion chestBind;
    Quaternion headBind;

    Quaternion smoothHips;
    Quaternion smoothSpine;
    Quaternion smoothChest;
    Quaternion smoothHead;

    [Header("Body Settings")]
    public float bodyYaw = 25f;
    public float bodyRoll = 15f;
    public float bodySmooth = 6f;

    [Header("Head Settings")]
    public float headYaw = 40f;
    public float headPitch = 30f;
    public float headSmooth = 10f;

    void Start()
    {
        animator = GetComponent<Animator>();

        hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        head = animator.GetBoneTransform(HumanBodyBones.Head);

        hipsBind = hips.localRotation;
        spineBind = spine.localRotation;
        chestBind = chest.localRotation;
        headBind = head.localRotation;

        smoothHips = hipsBind;
        smoothSpine = spineBind;
        smoothChest = chestBind;
        smoothHead = headBind;

        Debug.Log("✅ Body + Head Driver READY");
    }

    // 🔥 WAJIB LateUpdate
    void LateUpdate()
    {
        var p = UDPReceiver.latestPose;
        if (p == null) return;

        ApplyBody(p.spine);
        ApplyHead(p.head);
    }

    void ApplyBody(float[] dir)
    {
        if (dir == null || dir.Length != 3) return;

        Vector3 d = new Vector3(
            dir[0],
           -dir[1],
            0f
        );

        if (d.sqrMagnitude < 0.0001f) return;
        d.Normalize();

        // ===============================
        // DEADZONE AGAR TEGAP
        // ===============================
        float dx = Mathf.Abs(d.x) < 0.1f ? 0f : d.x;
        float dy = Mathf.Abs(d.y) < 0.1f ? 0f : d.y;

        float yaw =
            Mathf.Clamp(dx * bodyYaw, -bodyYaw, bodyYaw);

        float pitch =
            Mathf.Clamp(-dy * 15f, -12f, 12f);

        // ❗ ROLL DIMATIKAN
        Quaternion hipsTarget =
            Quaternion.Euler(pitch * 0.3f, yaw * 0.4f, 0f);

        Quaternion spineTarget =
            Quaternion.Euler(pitch * 0.6f, yaw * 0.6f, 0f);

        Quaternion chestTarget =
            Quaternion.Euler(pitch, yaw, 0f);

        smoothHips = Quaternion.Slerp(
            smoothHips,
            hipsTarget * hipsBind,
            Time.deltaTime * bodySmooth
        );

        smoothSpine = Quaternion.Slerp(
            smoothSpine,
            spineTarget * spineBind,
            Time.deltaTime * bodySmooth * 1.2f
        );

        smoothChest = Quaternion.Slerp(
            smoothChest,
            chestTarget * chestBind,
            Time.deltaTime * bodySmooth * 1.5f
        );

        hips.localRotation = smoothHips;
        spine.localRotation = smoothSpine;
        chest.localRotation = smoothChest;
    }

    void ApplyHead(float[] dir)
    {
        if (dir == null || dir.Length != 3) return;

        // MediaPipe → Unity space
        Vector3 d = new Vector3(
            dir[0],
            dir[1],
            dir[2]
        );

        if (d.sqrMagnitude < 0.0001f) return;
        d.Normalize();

        // 🔑 HEAD ROTATION
        float yaw =
            Mathf.Clamp(d.x * headYaw, -headYaw, headYaw);

        float pitch =
            Mathf.Clamp(d.y * headPitch, -headPitch, headPitch);

        Quaternion target =
            Quaternion.Euler(-pitch, yaw, 0f);

        smoothHead = Quaternion.Slerp(
            smoothHead,
            target * headBind,
            Time.deltaTime * headSmooth
        );

        head.localRotation = smoothHead;
    }

}
