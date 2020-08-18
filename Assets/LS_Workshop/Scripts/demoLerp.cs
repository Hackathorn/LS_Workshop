using UnityEngine;

[ExecuteInEditMode]
public class demoLerp : MonoBehaviour
{
    public Transform start;
    public Transform end;
    [Range(0f, 1f)]
    public float lerpPct = 0.5f;

    private void Update() 
    {
        transform.position = Vector3.LerpUnclamped(start.position, end.position, lerpPct);
        transform.rotation = Quaternion.LerpUnclamped(start.rotation, end.rotation, lerpPct);
    }
}
