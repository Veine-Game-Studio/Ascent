using UnityEngine;
using Ascent.HSM;

namespace Ascent.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(InputReader))]
    [RequireComponent(typeof(PlayerAbilities))]
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D RB { get; private set; }
        public InputReader InputReader { get; private set; }
        public StateMachine StateMachine { get; private set; }
        public PlayerAbilities Abilities { get; private set; }

        [Header("Movement Settings")]
        public float WalkSpeed = 6f;
        public float AirSpeedMultiplier = 0.5f;

        [Header("Dash Settings")]
        public float DashSpeed = 15f;
        public float DashDuration = 0.2f;
        public float DashCooldown = 1f;
        
        [Header("Ground Pound Settings")]
        public float GroundPoundFallSpeed = -25f;
        public float GroundPoundPauseDuration = 0.15f;
        public float GroundPoundImpactDuration = 0.2f;
        
        [Header("Jump Settings")]
        public float JumpForce = 7f;

        [Header("Wall Slide/Jump Settings")]
        public float WallSlideSpeed = -2f;
        public float WallClingDuration = 0.5f;
        public float WallJumpForceY = 6f;
        public float WallJumpForceX = 5f;

        [Header("Collision Checks")]
        public Transform GroundCheck;
        public Vector2 GroundCheckSize = new Vector2(0.8f, 0.1f);
        public LayerMask GroundLayer;

        [Header("Ledge Detect")]
        public Transform WallCheck;
        public float WallCheckDistance = 0.5f;
        public Transform LedgeCheck;
        public float LedgeCheckDistance = 0.5f;
        public float LedgeClimbDuration = 0.5f;
        public Vector2 LedgeClimbOffsetTop = new Vector2(0.5f, 1f);

        public float LastDashTime { get; set; } = -100f;
        public bool HasAirDashed { get; set; } = false;

        public float OriginalGravity { get; private set; }

        private void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
            OriginalGravity = RB.gravityScale;
            InputReader = GetComponent<InputReader>();
            Abilities = GetComponent<PlayerAbilities>();
            StateMachine = new StateMachine();
        }

        public bool CanDash()
        {
            return Abilities != null && Abilities.CanDash && Time.time >= LastDashTime + DashCooldown;
        }

        private void Start()
        {
            StateMachine.ChangeState(new PlayerGroundedState(this));
        }

        private void Update()
        {
            StateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick(Time.fixedDeltaTime);
        }

        [Space]
        [Header("Debug Info")]
        [SerializeField] private bool _isGroundedDebug;

        public bool IsGrounded()
        {
            if (GroundCheck == null) 
            {
                _isGroundedDebug = false;
                return false;
            }
            _isGroundedDebug = Physics2D.OverlapBox(GroundCheck.position, GroundCheckSize, 0f, GroundLayer);
            return _isGroundedDebug;
        }

        public bool IsTouchingWall()
        {
            if (WallCheck == null) return false;
            return Physics2D.Raycast(WallCheck.position, transform.right, WallCheckDistance, GroundLayer);
        }

        public bool CheckForLedge(out Vector2 ledgeCorner)
        {
            ledgeCorner = Vector2.zero;

            if (LedgeCheck == null || WallCheck == null) return false;

            // We are touching a wall, but not touching the ledge check -> we found a ledge!
            bool touchingWall = Physics2D.Raycast(WallCheck.position, transform.right, WallCheckDistance, GroundLayer);
            bool touchingLedge = Physics2D.Raycast(LedgeCheck.position, transform.right, LedgeCheckDistance, GroundLayer);

            if (touchingWall && !touchingLedge)
            {
                // Simple calculation of ledge corner:
                // From the LedgeCheck position, shoot down to find the exact top surface of the wall.
                RaycastHit2D hit = Physics2D.Raycast(LedgeCheck.position + transform.right * WallCheckDistance, Vector2.down, LedgeCheck.localPosition.y - WallCheck.localPosition.y + 0.5f, GroundLayer);
                
                if (hit.collider != null)
                {
                    // For perfect corner, cast from hitting point towards the wall check direction
                    RaycastHit2D wallHit = Physics2D.Raycast(new Vector2(transform.position.x, hit.point.y - 0.1f), transform.right, WallCheckDistance + 1f, GroundLayer);
                    
                    if (wallHit.collider != null)
                    {
                        ledgeCorner = new Vector2(wallHit.point.x, hit.point.y);
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnDrawGizmosSelected()
        {
            if (GroundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(GroundCheck.position, GroundCheckSize);
            }

            if (WallCheck != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(WallCheck.position, WallCheck.position + transform.right * WallCheckDistance);
            }

            if (LedgeCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(LedgeCheck.position, LedgeCheck.position + transform.right * LedgeCheckDistance);
            }
        }

        public void FlipPlayerPivot(float horizontalInput)
        {
            if (horizontalInput > 0 && transform.eulerAngles.y != 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (horizontalInput < 0 && transform.eulerAngles.y != 180)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }
}
