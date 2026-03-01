using UnityEngine;

public class PCPlayerInputHandler
{
    private const string HorizontalAxisName = "Horizontal";
    private const string VerticalAxisName = "Vertical";

    public Vector3 GetMovementDirection()
    {
        return new Vector3(Input.GetAxis(HorizontalAxisName), Input.GetAxis(VerticalAxisName));
    }

    public Vector3 GetRotationDirection()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;

        return mouseWorldPosition;
    }

    public bool IsShootKeyPressing()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }
}
