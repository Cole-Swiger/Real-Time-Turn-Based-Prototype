using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleDrawer : MonoBehaviour
{
    public float radius = 1f;
    public int segments = 50;
    private LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        DrawCircle();
        HideRange();
    }

    void DrawCircle()
    {
        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * (i * 360f / segments);

            // Calculate X and Z positions (Ground Plane)
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;
            float y = 0f; // Keep Y coordinate at zero to ensure it is flat on the ground

            line.SetPosition(i, new Vector3(x, y, z));
        }
    }

    public void SetRadius(float newRadius)
    {
        // Only redraw if the radius has actually changed
        if (Mathf.Abs(radius - newRadius) > 0.001f) 
        {
            radius = newRadius;
            DrawCircle();
        }
    }

    // Call this function when the user selects the attack action
    public void ShowRange()
    {
        line.enabled = true;
    }

    // Call this function when the user deselects the attack action
    public void HideRange()
    {
        line.enabled = false;
    }
}
