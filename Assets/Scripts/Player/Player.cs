using UnityEngine;

public class Player : MonoBehaviour
{
    public InputController Input;
    public Rigidbody Rigid;

    private void Awake()
    {
        Input = GetComponent<InputController>();
        Rigid = GetComponent<Rigidbody>();
    }
}