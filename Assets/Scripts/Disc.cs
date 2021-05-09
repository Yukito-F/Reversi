using UnityEngine;

public class Disc : MonoBehaviour
{
    int state = 1;
    Rigidbody rig;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    public void reload(int remoteState)
    {
        if (state == remoteState)
        {
            return;
        }

        rig.AddForce(0, 20.0f, 0);
        rig.AddTorque(0.8f, 0, 0);
        state = remoteState;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            reload(-1 * state);
        }
    }
}