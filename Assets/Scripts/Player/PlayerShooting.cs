using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Gun _gun;
    private Player _player;
    private bool _canFire = true;

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
        if (!fire || !_canFire || !_gun.CanFire) return;

        //# 애니메이션 동작 및 발사 완료 후 _canFire는 true가 됨
        _canFire = false;
        _player.Ani.SetFire();
    }

    private void GunShoot()
    {
        _gun.Fire();
        _canFire = true;
    }
}