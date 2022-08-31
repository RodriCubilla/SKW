// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

using System;
using UnityEngine;
using System.Collections;

namespace TarodevController {
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IPlayerController {
        [SerializeField] private ScriptableStats _stats;

        private FrameInput _frameInput;
        private Rigidbody2D _rb;
        private CapsuleCollider2D[] _cols; // Standing and crouching colliders
        private CapsuleCollider2D _col; // Current collider
        private PlayerInput _input;
        private Animator animacion;
        [SerializeField] private CombateCaC golpes;

        private Vector2 _speed;
        private bool _jumpToConsume;
        private bool _endedJumpEarly;
        private int _fixedFrame;
        private bool _coyoteUsed;
        private bool _coyoteUsable;
        private bool _doubleJumpUsable;
        private bool _bufferedJumpUsable;
        private bool _crouching;
        private bool _grounded;
        private Vector2 _groundNormal;
        private int _frameLeftGrounded = int.MinValue;
        private int _lastJumpPressed = int.MinValue;
        private int _frameLastAttacked = int.MinValue;
        private bool _attackToConsume;
        private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
        private readonly RaycastHit2D[] _ceilingHits = new RaycastHit2D[1];
        private int _groundHitCount;
        private Vector2 _currentExternalVelocity;
        private bool _dashToConsume;
        private bool _canDash;
        private Vector2 _dashVel;
        private bool _dashing;
        private int _startedDashing;
        
        #region External

        public Vector2 Speed => _speed;
        public bool Crouching => _crouching;
        public Vector2 GroundNormal => _groundNormal;
        public ScriptableStats PlayerStats => _stats;
        public Vector2 Input => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action<bool, Vector2> DashingChanged;
        public event Action Jumped;
        public event Action DoubleJumped;
        public event Action Attacked;

        public virtual void ApplyVelocity(Vector2 vel, PlayerForce forceType) {
            if (forceType == PlayerForce.Burst) _speed += vel;
            else _currentExternalVelocity += vel;
        }

        #endregion

        #region Movimientos

        [Header("MOVIMIENTOS")]
        [SerializeField] private LayerMask queEsSuelo;

        [SerializeField] private Transform controladorPared;
        [SerializeField] private Vector3 dimensionesCajaPared;
        private bool enPared;

        [SerializeField] private Transform controladorSuelo;
        [SerializeField] private Vector3 dimensionesCajaSuelo;
        [SerializeField] private Transform controladorTecho;
        private bool esTecho;
        [SerializeField] private bool enSuelo;
        private bool deslizando;
        [SerializeField] private float velocidadDeslizar;
        [SerializeField] private float fuerzaSaltoX;
        [SerializeField] private float fuerzaSaltoY;
        [SerializeField] private float tiempoSaltoPared;
        private bool saltandoDePared;

        /*[Header("Climb")]
        [SerializeField] private float velocidadEscalar;
        private CapsuleCollider2D capsuleCollider2D;
        private float gravedadInicial;
        private bool escalando;*/

        #endregion

        protected virtual void Awake() {
            Physics2D.queriesStartInColliders = false; // I'll remove this in a future update
            
            _rb = GetComponent<Rigidbody2D>();
            _cols = GetComponents<CapsuleCollider2D>();
            _input = GetComponent<PlayerInput>();
            animacion = GetComponent<Animator>();
            //capsuleCollider2D = GetComponent<CapsuleCollider2D>();
            //gravedadInicial = _rb.gravityScale;

            golpes = FindObjectOfType<CombateCaC>();

            SetCrouching(false);
        }

        protected virtual void Update() {
            GatherInput();
            //Climb();
            animacion.SetBool("Crouch", _crouching);
            animacion.SetBool("Deslizando", deslizando);
            animacion.SetBool("EnSuelo", enSuelo);

            if(!_grounded && enPared && _frameInput.Move.x != 0)
            {
                deslizando = true;
            }
            else
            {
                deslizando = false;
            }

            if(golpes.ataca == true)
            {
                _frameInput.Move.x = 0;
            }

        }

        protected virtual void GatherInput() {
            _frameInput = _input.FrameInput;

            if (_frameInput.JumpDown) {
                _jumpToConsume = true;
                _lastJumpPressed = _fixedFrame;
            }

            if (_frameInput.DashDown) _dashToConsume = true;
            if (_frameInput.AttackDown) _attackToConsume = true;
        }


        protected virtual void FixedUpdate() {
            _fixedFrame++;
            _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);

            enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCajaSuelo, 0f, queEsSuelo);

            enPared = Physics2D.OverlapBox(controladorPared.position, dimensionesCajaPared, 0f, queEsSuelo);


            CheckCollisions();
            HandleAttacking();
            HandleCrouching();
            HandleHorizontal();
            HandleJump();
            HandleDash();
            HandleFall();

            ApplyVelocity();

            if(deslizando)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -velocidadDeslizar, float.MaxValue));
            }
        }

        #region Collisions

        protected virtual void CheckCollisions() {
            var offset = (Vector2)transform.position + _col.offset;

            _groundHitCount = Physics2D.CapsuleCastNonAlloc(offset, _col.size, _col.direction, 0, Vector2.down, _groundHits, _stats.GrounderDistance);
            var ceilingHits = Physics2D.CapsuleCastNonAlloc(offset, _col.size, _col.direction, 0, Vector2.up, _ceilingHits, _stats.GrounderDistance);

            if (ceilingHits > 0 && _speed.y > 0) _speed.y = 0;

            if (!_grounded && _groundHitCount > 0) {
                _grounded = true;
                _coyoteUsable = true;
                _doubleJumpUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                _canDash = true;
                GroundedChanged?.Invoke(true, Mathf.Abs(_speed.y));
            }
            else if (_grounded && _groundHitCount == 0) {
                _grounded = false;
                _frameLeftGrounded = _fixedFrame;
                GroundedChanged?.Invoke(false, 0);
            }
        }

        #endregion

        #region Attacking

        protected virtual void HandleAttacking() {
            if (!_attackToConsume) return;

            if (_frameLastAttacked + _stats.AttackFrameCooldown < _fixedFrame) {
                _frameLastAttacked = _fixedFrame;
                Attacked?.Invoke();
            }

            _attackToConsume = false;
        }

        #endregion

        #region Crouching

        protected virtual void HandleCrouching() {
            var crouchCheck = _frameInput.Move.y <= _stats.CrouchInputThreshold;
            if (crouchCheck != _crouching) SetCrouching(crouchCheck);
        }

        protected virtual void SetCrouching(bool active) {
            _crouching = active;
            _col = _cols[active ? 1 : 0];
            _cols[0].enabled = !active;
            _cols[1].enabled = active;
        }


        #endregion

       /*#region Climb

        private void Climb()
        {
            if((_frameInput.Move.y != 0 || escalando) && (capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Escalera"))))
            {
                Vector2 velocidadSubida = new Vector2(_rb.velocity.x, _frameInput.Move.y * velocidadEscalar);
                _rb.gravityScale = 0;
                escalando = true;
            }
            else
            {
                _rb.gravityScale = gravedadInicial;
                escalando = false;
            }
            if(enSuelo)
            {
                escalando = false;
            }
        }
        #endregion*/

        #region Horizontal

        protected virtual void HandleHorizontal() {
            if (_frameInput.Move.x != 0 && !saltandoDePared) {
                if (_crouching && _grounded) {
                    var penaltySpeed = _stats.MaxSpeed * _stats.CrouchSpeedPenalty;
                    _speed.x = Mathf.MoveTowards(_speed.x, penaltySpeed * _frameInput.Move.x, _stats.Deceleration * Time.fixedDeltaTime);
                    if(_frameInput.Move.x < 0 && _grounded && _crouching)
                    {
                        this.transform.localScale = new Vector2(-1,1);
                        animacion.SetBool("CrouchWalk ",  true);
                    }
                    else if(_frameInput.Move.x > 0 && _grounded && _crouching)
                    {
                        this.transform.localScale = new Vector2(1,1);
                        animacion.SetBool("CrouchWalk ",  true);
                    }
                }
                else {

                    if (_stats.AllowCreeping) _speed.x = Mathf.MoveTowards(_speed.x, _stats.MaxSpeed * _frameInput.Move.x, _stats.Acceleration * Time.fixedDeltaTime);
                    else _speed.x += _frameInput.Move.x * _stats.Acceleration * Time.fixedDeltaTime;

                    if(_frameInput.Move.x < 0 && _grounded && !saltandoDePared)
                    {
                        this.transform.localScale = new Vector2(-1,1);
                        animacion.SetBool("Walk", true);
                    }
                    else if(_frameInput.Move.x > 0 && _grounded && !saltandoDePared)
                    {
                        this.transform.localScale = new Vector2(1,1);
                        animacion.SetBool("Walk", true);
                    }
                }
            }
            else {
                _speed.x = Mathf.MoveTowards(_speed.x, 0, _stats.Deceleration * (_grounded ? 1 : _stats.AirDecelerationPenalty) * Time.fixedDeltaTime);
                animacion.SetBool("Walk", false);
                animacion.SetBool("CrouchWalk ",  false);
            }

            _speed.x = Mathf.Clamp(_speed.x, -_stats.MaxSpeed, _stats.MaxSpeed);
        }

        #endregion

        #region Jump

        private bool CanUseCoyote => _coyoteUsable && !_grounded && _frameLeftGrounded + _stats.CoyoteFrames > _fixedFrame;
        private bool HasBufferedJump => _grounded && _bufferedJumpUsable && _lastJumpPressed + _stats.JumpBufferFrames > _fixedFrame;
        private bool CanDoubleJump => _stats.AllowDoubleJump && _doubleJumpUsable && !_coyoteUsable;

        protected virtual void HandleJump() {
            if (_jumpToConsume && CanDoubleJump && !deslizando) {
                _speed.y = _stats.JumpPower;
                _doubleJumpUsable = false;
                _endedJumpEarly = false;
                _jumpToConsume = false;
                DoubleJumped?.Invoke();
                animacion.SetTrigger("Jump");
            }


            if ((_jumpToConsume && CanUseCoyote && !deslizando) || HasBufferedJump) {
                _coyoteUsable = false;
                _bufferedJumpUsable = false;
                _speed.y = _stats.JumpPower;
                Jumped?.Invoke();
                animacion.SetTrigger("Jump");
            }

            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
            
            if(_jumpToConsume && enPared && deslizando)
            {
                SaltoPared();
            }
        }

        private void SaltoPared()
        {
            enPared = false;
            _rb.velocity = new Vector2(fuerzaSaltoX * -_frameInput.Move.x, fuerzaSaltoY);
            StartCoroutine(CambioSaltoPared());
        }

        IEnumerator CambioSaltoPared()
        {
            saltandoDePared = true;
            yield return new WaitForSeconds(tiempoSaltoPared);
            saltandoDePared = false;
        }

        #endregion

        #region Dash

        protected virtual void HandleDash() {
            if (!_stats.AllowDash) return;
            if (_dashToConsume && _canDash && !_crouching) {
                var dir = new Vector2(_frameInput.Move.x, _grounded && _frameInput.Move.y < 0 ? 0 : _frameInput.Move.y).normalized;
                if (dir == Vector2.zero) {
                    _dashToConsume = false;
                    return;
                }

                _dashVel = dir * _stats.DashVelocity;
                _dashing = true;
                DashingChanged?.Invoke(true, dir);
                _canDash = false;
                _startedDashing = _fixedFrame;

                // Strip external buildup
                _currentExternalVelocity = Vector2.zero;
            }

            if (_dashing) {
                _speed = _dashVel;
                // Cancel when the time is out or we've reached our max safety distance
                if (_startedDashing + _stats.DashDurationFrames < _fixedFrame) {
                    _dashing = false;
                    DashingChanged?.Invoke(false, Vector2.zero);
                    if (_speed.y > 0) _speed.y = 0;
                    _speed.x *= _stats.DashEndHorizontalMultiplier;
                    if (_grounded) _canDash = true;
                }
            }

            _dashToConsume = false;
        }

        #endregion

        #region Falling

        protected virtual void HandleFall() {
            if (_dashing) return;
            if (_grounded && _speed.y <= 0) {
                // Slopes
                _speed.y = _stats.GroundingForce;
                _groundNormal = Vector2.zero;
                for (var i = 0; i < _groundHitCount; i++) {
                    var hit = _groundHits[i];
                    if (hit.collider.isTrigger) continue;
                    _groundNormal = hit.normal;

                    var slopePerp = Vector2.Perpendicular(_groundNormal).normalized;
                    var slopeAngle = Vector2.Angle(_groundNormal, Vector2.up);

                    if (slopeAngle != 0) {
                        if (_speed.x == 0) {
                            _speed.y = 0;
                            
                        }
                        else {
                            _speed.y = _speed.x * -slopePerp.y;
                            _speed.y += _stats.GroundingForce;
                            //animacion.SetTrigger("Fall");
                        }

                        break;
                    }
                }
            }
            else {
                var fallSpeed = _endedJumpEarly && _speed.y > 0 ? -_stats.FallSpeed * _stats.JumpEndEarlyGravityModifier : -_stats.FallSpeed;
                _speed.y += fallSpeed * Time.fixedDeltaTime;
                if (_speed.y < -_stats.MaxFallSpeed) _speed.y = -_stats.MaxFallSpeed;
            }
        }

        #endregion

        protected virtual void ApplyVelocity() {
            _rb.velocity = _speed + _currentExternalVelocity;
            _jumpToConsume = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCajaSuelo);
            Gizmos.DrawWireCube(controladorPared.position, dimensionesCajaPared);
        }
    }

    public interface IPlayerController {
        public Vector2 Input { get; }
        public Vector2 Speed { get; }
        public bool Crouching { get; }
        public Vector2 GroundNormal { get; }
        public ScriptableStats PlayerStats { get; }

        public event Action<bool, float> GroundedChanged; // Grounded - Impact force
        public event Action<bool, Vector2> DashingChanged; // Dashing - Dir
        public event Action Jumped, DoubleJumped;
        public event Action Attacked;

        public void ApplyVelocity(Vector2 vel, PlayerForce forceType);
    }

    public enum PlayerForce {
        /// <summary>
        /// Added directly to the players movement speed, to be controlled by the standard deceleration
        /// </summary>
        Burst,

        /// <summary>
        /// An additive force handled by the decay system
        /// </summary>
        Decay
    }
}