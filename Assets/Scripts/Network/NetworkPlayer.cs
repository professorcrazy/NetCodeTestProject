using NUnit.Framework.Constraints;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EffectHandler))]
public class NetworkPlayer : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float maxSpeed = 6f;


    [Header("Jumping")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private Transform groundCheckPoint;

    private EffectHandler effectHandler;
    private WeaponHandler weaponHandler;

    private InputSystem_Actions input;
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isSprinting;
    private bool isJumping;


    //network testing elements
    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private Transform spawnPrefab;

    void Awake()
    {
        input = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        effectHandler = GetComponent<EffectHandler>();
        weaponHandler = GetComponent<WeaponHandler>();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += _ => moveInput = Vector2.zero;

        input.Player.Sprint.performed += _ => isSprinting = true;
        input.Player.Sprint.canceled += _ => isSprinting = false;

        input.Player.Jump.performed += _ => isJumping = true;
        input.Player.Attack.performed += _ => weaponHandler?.UseWeapon();
        input.Player.Drop.performed += _ => weaponHandler?.DropCurrentWeapon();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void FixedUpdate() {
        ApplyMovement();
        ApplyJump();
    }

    private void ApplyMovement() {
        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        float boostedSpeed = effectHandler != null ? effectHandler.GetSpeed() : speed;
        float speedMultiplier = isSprinting ? sprintMultiplier : 1f;
        Vector3 force = transform.TransformDirection(inputDir) * (boostedSpeed * speedMultiplier);

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude < maxSpeed * speedMultiplier) {
            rb.AddForce(force, ForceMode.Force);
        }
    }

    private void ApplyJump() {
        if (!isJumping) {
            return;
        }
        isJumping = false;
        if (IsGrounded()) {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded() {
        return Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    public override void OnNetworkSpawn() {
        randomNumber.OnValueChanged += (int previousValue, int newValue) => {
            Debug.Log(OwnerClientId + " : #" + randomNumber.Value);
        };
    }
    /*
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            randomNumber.Value = Random.Range(0, 10);
            Transform spawnedObj = Instantiate(spawnPrefab);
            spawnedObj.transform.position = transform.position;
            spawnedObj.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId,true);            
            Destroy(spawnedObj.gameObject, 2f);
        }
    
    }
    */
    /*
        public void Died() {
            var dropped = Instantiate(weaponPickupPrefab, transform.position, Quaternion.identity);
            dropped.Initialize(currentWeapon.Data, currentWeapon.CurrentAmmo);

        }
    */


    private void OnTriggerEnter(Collider other) {
        WeaponPickup pickup = other.GetComponent<WeaponPickup>();
        if (pickup != null) {
            IWeapon weapon = pickup.CreateWeapon();
            weaponHandler.EquipWeapon(weapon);
            Destroy(pickup.gameObject);
        }
    }
}
