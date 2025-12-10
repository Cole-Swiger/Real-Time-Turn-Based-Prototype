using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

//Class used to draw range circles around units for movement and actions
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

    //Set the size of the circle
    public void SetRadius(float newRadius)
    {
        // Only redraw if the radius has actually changed
        if (Mathf.Abs(radius - newRadius) > 0.001f) 
        {
            radius = newRadius;
            DrawCircle();
        }
    }

    // Call this function when the user selects the action
    public void ShowRange()
    {
        line.enabled = true;
    }

    // Call this function when the user deselects the action
    public void HideRange()
    {
        line.enabled = false;
    }
}
