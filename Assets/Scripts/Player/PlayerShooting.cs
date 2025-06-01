using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Gun _gun;
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.Input.OnFire -= Fire;
        _player.Input.OnFire += Fire;
    }

    private void OnDisable()
    {
        _player.Input.OnFire -= Fire;
    }

    private void Fire(bool fire)
    {
        _gun.Fire(fire);
    }
}