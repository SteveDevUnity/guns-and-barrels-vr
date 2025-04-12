using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;

public class Colt_Manager : MonoBehaviour, IDataPersistance
{
    [SerializeField] Animator _animator;
    [SerializeField] AudioSource _audioSource;
    [SerializeField] ParticleSystem _coltMuzzleFlash;
    [SerializeField] GameObject _firePoint;
    [SerializeField] GameObject _beltAttachPoint;

    public GameObject BulletPrefab;
    public LayerMask TargetLayer;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _interactable;
    private float _bulletSpeed = 20.0f;
    private Joint _fixedJoint;
    private bool _tutorialIsCompleted;
    
    private BoxCollider _boxCollider;

    public static event Action OnWeaponGrabbedFirstTime;

    private void OnEnable()
    {
        
        TutorialManager.OnGripTutorialConfirmed += ReleaseWeapon;
    }

    private void OnDisable()
    {
        TutorialManager.OnGripTutorialConfirmed -= ReleaseWeapon;
    }

    private void Awake()
    {
        _fixedJoint = GetComponent<Joint>();
        _boxCollider = GetComponent<BoxCollider>();
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void IDataPersistance.LoadGameData(GameData gameData)
    {
        _tutorialIsCompleted = gameData.TutorialIsCompleted;

        if (_tutorialIsCompleted)
        {
            Destroy(_fixedJoint);
        }

        else
        {
            _interactable.enabled = false;
        }
    }

    void IDataPersistance.SaveGameData(ref GameData gameData)
    {
        if(_tutorialIsCompleted)
        {
            gameData.WeaponPosition = transform.position;
            gameData.WeaponRotation = new float[] {transform.rotation.x, transform.rotation.y,
              transform.rotation.z, transform.rotation.w};
        }

        
    }

    private void Start()
    {
        if (_tutorialIsCompleted)
        {            
            transform.SetPositionAndRotation(_beltAttachPoint.transform.position,
                _beltAttachPoint.transform.rotation);
        }
    }

    // Called from XRGrabInteractable Component Event Activated
    public void ShootBullet()
    {
        PlayShootAnimation();

        GameObject Bullet = Instantiate(BulletPrefab, _firePoint.transform.position, Quaternion.identity);

        Rigidbody rb_bullet = Bullet.GetComponent<Rigidbody>();

        rb_bullet.velocity = _firePoint.transform.forward * _bulletSpeed;

        Destroy(Bullet, 5.0f);

    }

    private void PlayShootAnimation()
    {
        _animator.SetTrigger("IsShooting");
        _audioSource.Play();
        _coltMuzzleFlash.Play();
    }


    // If Player loses or throws Weapon away
    // Called from XRGrabInteractable Component Event Select Exited
    public void SnapWeaponToHolster()
    {
        transform.position = new Vector3(_beltAttachPoint.transform.position.x, 
            _beltAttachPoint.transform.position.y, _beltAttachPoint.transform.position.z);
    }

    public void DestroyJoint(SelectEnterEventArgs args)
    {
    
        if (_fixedJoint != null && args.interactableObject.transform.gameObject.CompareTag("Weapon"))
        {
            Destroy(_fixedJoint);

            

            //Notify: TutorialManager.cs to show TriggerTutorial UI 
            OnWeaponGrabbedFirstTime.Invoke();
        }

        else
        {
            return;
        }
    }
    

    private void ReleaseWeapon()
    {
        _interactable.enabled = true;
    }

        
    


    
}
