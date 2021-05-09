using UnityEngine;

public class CursorController : MonoBehaviour
{
    public void changeColor(bool emphasis)
    {
        if (emphasis)
        {
            name = "Enable";
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            name = "Invalid";
            GetComponent<Renderer>().material.color = Color.green;
        }
    }
}
