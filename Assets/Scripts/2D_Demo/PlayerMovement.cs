using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    #region Variables
    enum PlayerStates { Idle, Run, Airborne, Sprint, WallJump, WallSlide, Dash, Kick}
    PlayerStates _state;
    bool _stateComplete;
    private bool _animComplete;
    public bool AnimComplete { get => _animComplete ; private set => _animComplete = value; }

    [Header("Reference")]
    [SerializeField] private PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;
    [SerializeField] private Animator _animator;

    private Rigidbody2D _rb;

    //movement
    public float HorizontalVelocity { get; private set; }
    private bool _isFacingRight;
    public bool IsFacingRight { get => _isFacingRight; set => _isFacingRight = value;  }


    //collision check
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _lastWallHit;
    private bool _isGrounded;
    private bool _bumpedHead;
    private bool _isTouchingWall;

    //jump
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastFalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    //apex
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //jump buffer
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    //coyote time
    private float _coyoteTimer;

    //wall slide
    private bool _isWallSliding;
    private bool _isWallSlideFalling;

    //wall jump
    private bool _useWallJumpMoveStats;
    private bool _isWallJumping;
    private float _wallJumpTime;
    private bool _isWallJumpFastFalling;
    private bool _isWallJumpFalling;
    private float _wallJumpFastFallTime;
    private float _wallJumpFastFallReleaseSpeed;

    private float _wallJumpPostBufferTimer;
    
    private float _wallJumpApexPoint;
    private float _timerPastWallJumpApexThreshold;
    private bool _isPastWallJumpApexThreshold;

    //dash
    private bool _isDashing;
    private bool _isAirDashing;
    private float _dashTimer;
    private float _dashOnGroundTimer;
    private int _numberOfDashesUsed;
    private Vector2 _dashDirection;
    private bool _isDashFastFalling;
    private float _dashFastFallTime;
    private float _dashFastFallReleaseSpeed;

    private Coroutine _dashEffectCoroutine;

    [SerializeField] private CharacterDataBase _characterDB;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int _selectedOption = 0;

    private KnockbackPhysicPlayer _knockbackPhysic;

    [SerializeField] private AudioClip _dashClip;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _knockbackPhysic = GetComponent<KnockbackPhysicPlayer>();
    }

    private void Start()
    {
        Turn(true);
        if (!PlayerPrefs.HasKey("SelectedOption"))
        {
            _selectedOption = 0;
        }
        else
        {
            Load();
        }
        UpdateCharacter(_selectedOption);
    }

    private void FixedUpdate()
    {
        CollisionCheck();
        Jump();
        Fall();
        WallSlide();
        WallJump();
        Dash();
        if (_isGrounded)
        {
            if (_knockbackPhysic != null && _knockbackPhysic.InHitStun)
                return;
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            //wall jump
            if(_useWallJumpMoveStats)
            {
                if (_knockbackPhysic != null && _knockbackPhysic.InHitStun)
                    return;
                Move(MoveStats.WallJumpMoveAcceleration, MoveStats.WallJumpMoveDeceleration, InputManager.Movement);
            }
            else //airborne
            {
                if (_knockbackPhysic != null && _knockbackPhysic.InHitStun)
                    return;
                Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
            }
        }

        ApplyVelocity();
    }

    private void Update()
    {
        CountTimers();
        JumpChecks();
        LandCheck();
        WallSlideCheck();
        WallJumpCheck();
        DashCheck();

        if(_stateComplete)
        {
            SelectState();
        }
        UpdateState();
    }

    private void UpdateCharacter(int selectedOption)
    {
        Character character = _characterDB.GetCharacter(selectedOption);
        _spriteRenderer.sprite = character.CharacterSprite;
        _spriteRenderer.color = character.Color;
        MoveStats = character.PlayerMovementStats;
    }

    private void Load()
    {
        _selectedOption = PlayerPrefs.GetInt("SelectedOption");
    }

    #region Movement
    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (!_isDashing)
        {
            if (Mathf.Abs(moveInput.x) >= MoveStats.MoveThreshold)
            {
                TurnCheck(moveInput);

                float targetVelocity = 0f;
                if (InputManager.RunIsHeld)
                {
                    targetVelocity = moveInput.x * MoveStats.MaxRunSpeed;
                }
                else { targetVelocity = moveInput.x * MoveStats.MaxWalkSpeed; }
                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }
            else if (Mathf.Abs(moveInput.x) < MoveStats.MoveThreshold)
            {
                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
            }
        }
        
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if(_isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if(!_isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if(turnRight)
        {
            _isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    private void ApplyVelocity()
    {
        //clamp fall speed 
        if (!_isDashing)
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);
        }
        else
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -50f, 50f);
        }
        _rb.velocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }
    #endregion

    #region Land/Fall

    private void LandCheck()
    {
        //landed
        if ((_isJumping || _isFalling || _isWallJumpFalling || _isWallJumping || _isWallSlideFalling || _isWallSliding || _isDashFastFalling) && _isGrounded && VerticalVelocity <= 0f)
        {
            ResetDashes();
            ResetJumpValues();
            ResetWallJumpValues();
            StopWallSlide();

            _numberOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;

            if(_isDashFastFalling && _isGrounded)
            {
                ResetDashValues();
                return;
            }

            ResetDashValues();
        }
    }

    private void Fall()
    {
        //normal gravity while falling
        if (!_isGrounded && !_isJumping && !_isWallSliding && !_isWallJumping && !_isDashing && !_isDashFastFalling)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }

            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Jump

    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastFalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
    }

    private void JumpChecks()
    {
        //when we press the jump button 
        if (InputManager.JumpWasPressed)
        {
            if(_isWallSlideFalling && _wallJumpPostBufferTimer >= 0f)
            {
                return;
            }
            else if(_isWallSliding || (_isTouchingWall && !_isGrounded))
            {
                return;
            }

            _jumpBufferTimer = MoveStats.JumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }
        //when we release the jump button 
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }
            if (_isJumping && VerticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        //initiate jump with jump buffering and coyote time
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (_jumpReleasedDuringBuffer)
            {
                _isFastFalling = true;
                _fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        //double jump
        else if (_jumpBufferTimer > 0f && (_isJumping || _isWallJumping || _isWallSlideFalling || _isAirDashing || _isDashFastFalling) && !_isTouchingWall && _numberOfJumpsUsed < MoveStats.NumberofJumpsAllowed)
        {
            _isFastFalling = false;
            InitiateJump(1);

            if(_isDashFastFalling)
            {
                _isDashFastFalling = false;
            }
        }
        //air jump after coyote time lapsed
        else if (_jumpBufferTimer > 0f && _isFalling && !_isWallSlideFalling && _numberOfJumpsUsed < MoveStats.NumberofJumpsAllowed - 1)
        {
            InitiateJump(2);
            _isFastFalling = false;
        }
    }

    private void InitiateJump(int numberofJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }

        ResetWallJumpValues();

        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberofJumpsUsed;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        //apply gravity while jumping 
        if (_isJumping)
        {
            //check for head bump
            if (_bumpedHead)
            {
                _isFastFalling = true;
            }

            //gravity on ascending
            if (VerticalVelocity >= 0f)
            {
                //apex controls
                _apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (_apexPoint > MoveStats.ApexThreshHold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else VerticalVelocity = -0.01f;
                    }
                }

                //gravity on ascending but not past apex threshold
                else if(!_isFastFalling)
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }

            //gravity on descending
            else if (!_isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (VerticalVelocity < 0f)
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }

        //jump cut
        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }
    }

    #endregion

    #region Collision check
    
    private void CollisionCheck()
    {
        IsGrounded();
        BumpedHead();
        IsTouchingWall();
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else _isGrounded = false;
        #region Debug Visualisation
        if (MoveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (_isGrounded)
            {
                rayColor = Color.green;
            }
            else rayColor = Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);

        }
        #endregion
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);
        if (_headHit.collider != null)
        {
            _bumpedHead = true;
        }
        else _bumpedHead = false;

        #region Debug Visualization
        if (MoveStats.DebugShowHeadBumpBox)
        {
            float headWitdth = MoveStats.HeadWidth;

            Color rayColor;
            if (_bumpedHead)
            {
                rayColor = Color.green;
            }
            else rayColor = Color.red;

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWitdth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWitdth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWitdth, boxCastOrigin.y + MoveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWitdth, rayColor);
        }

        #endregion
    }

    private void IsTouchingWall()
    {
        float orginalEndPoint = 0f;
        if (_isFacingRight)
        {
            orginalEndPoint = _bodyColl.bounds.max.x;
        }
        else { orginalEndPoint = _bodyColl.bounds.min.x; }

        float adjustedHeight = _bodyColl.bounds.size.y * MoveStats.WallDetectionRayHeightMulltiplier;

        Vector2 boxCastOrigin = new Vector2(orginalEndPoint, _bodyColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(MoveStats.WallDetectionRayLength, adjustedHeight);

        _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, MoveStats.WallDetectionRayLength, MoveStats.GroundLayer);
        if (_wallHit.collider != null)
        {
            _lastWallHit = _wallHit;
            _isTouchingWall = true;
        }
        else { _isTouchingWall = false; }

        #region Debug Visualization
        if (MoveStats.DebugShowWallHitBox)
        {
            Color rayColor;
            if (_isTouchingWall)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
            Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
            Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

            Debug.DrawLine(boxBottomLeft, boxTopRight, rayColor);
            Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopLeft, boxTopRight, rayColor);
        }
        #endregion
    }
        #endregion

    #region Wall Slide

    private void WallSlideCheck()
    {
        if(_isTouchingWall && !_isGrounded && !_isDashing)
        {
            if(VerticalVelocity < 0 && !_isWallSliding)
            {
                ResetDashValues();
                ResetJumpValues();
                ResetWallJumpValues();

                if(MoveStats.ResetDashOnWallSlide)
                {
                    ResetDashes();
                }

                _isWallSlideFalling = false;
                _isWallSliding = true;

                if(MoveStats.ResetJumpOnWallSlide)
                {
                    _numberOfJumpsUsed = 0;
                }
            }
        }
        else if(_isWallSliding && !_isTouchingWall && !_isGrounded && !_isWallSlideFalling)
        {
            _isWallSlideFalling = true;
            StopWallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if(_isWallSliding)
        {
            _numberOfJumpsUsed++;
            _isWallSliding = false;
        }
    }

    private void WallSlide()
    {
        if(_isWallSliding)
        {
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, -MoveStats.WallSlideSpeed, MoveStats.WallSlideDecelerationSpeed * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Wall Jump

    private void ResetWallJumpValues()
    {
        _isWallSlideFalling = false;
        _useWallJumpMoveStats = false;
        _isWallJumping = false;
        _isWallJumpFastFalling = false;
        _isWallJumpFalling = false;
        _isPastWallJumpApexThreshold = false;

        _wallJumpFastFallTime = 0;
        _wallJumpTime = 0;
    }

    private void WallJumpCheck()
    {
        if(ShouldApplyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }

        //wall jump fast falling
        if(InputManager.JumpWasReleased && !_isWallSliding && !_isTouchingWall && _isWallJumping)
        {
            if(VerticalVelocity > 0)
            {
                if(_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallTime = MoveStats.TimeForUpwardsCancel;

                    VerticalVelocity = 0f;
                }
                else
                {
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //actual jump with post wall jump buffer time
        if(InputManager.JumpWasPressed && _wallJumpPostBufferTimer > 0f)
        {
            InitiateWallJump();    
        }
    }

    private void InitiateWallJump()
    {
        if(!_isWallJumping)
        {
            _isWallJumping = true;
            _useWallJumpMoveStats = true;
        }

        StopWallSlide();
        ResetJumpValues();
        _wallJumpTime = 0f;

        VerticalVelocity = MoveStats.InitialWallJumpVelocity;

        int dirMultiplier = 0;
        Vector2 hitPoint = _lastWallHit.collider.ClosestPoint(_bodyColl.bounds.center);

        if(hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        else
        {
            dirMultiplier = 1;
        }

        HorizontalVelocity = Mathf.Abs(MoveStats.WallJumpDirection.x) * dirMultiplier;
    }

    private void WallJump()
    {
        //apply wall jump gravity
        if(_isWallJumping)
        {
            //time to take over movement controls while wall jumping
            _wallJumpTime += Time.fixedDeltaTime;
            if(_wallJumpTime >= MoveStats.TimeTillJumpApex)
            {
                _useWallJumpMoveStats = false;
            }

            //hit head
            if(_bumpedHead)
            {
                _isWallJumpFastFalling = true;
                _useWallJumpMoveStats = false;
            }

            //gravity in ascending
            if(VerticalVelocity >= 0f)
            {
                //apex controls
                _wallJumpApexPoint = Mathf.InverseLerp(MoveStats.WallJumpDirection.y, 0f, VerticalVelocity);

                if(_wallJumpApexPoint > MoveStats.ApexThreshHold)
                {
                    if(!_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = true;
                        _timerPastWallJumpApexThreshold = 0f;
                    }

                    if(_isPastWallJumpApexThreshold)
                    {
                        _timerPastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if(_timerPastWallJumpApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }

                //gravity in ascending but not past apex threshold
                else if(!_isWallJumpFalling)
                {
                    VerticalVelocity += MoveStats.WallJumpGravity * Time.fixedDeltaTime;

                    if(_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = false;
                    }
                }
            }

            //gravity on descending
            else if(!_isWallJumpFalling)
            {
                VerticalVelocity += MoveStats.WallJumpGravity * Time.fixedDeltaTime;
            }
            else if(VerticalVelocity < 0f)
            {
                if(!_isWallJumpFalling)
                {
                    _isWallJumpFalling = true;
                }
            }

            //handle wall jump cut time
            if(_isWallJumpFastFalling)
            {
                if(_wallJumpFastFallTime >= MoveStats.TimeForUpwardsCancel)
                {
                    VerticalVelocity += MoveStats.WallJumpGravity * MoveStats.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }
                else if(_wallJumpFastFallTime < MoveStats.TimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallTime / MoveStats.TimeForUpwardsCancel));
                }

                _wallJumpFastFallTime += Time.fixedDeltaTime;
            }
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if(!_isGrounded && (_isTouchingWall || _isWallSliding))
        {
            return true;
        }
        else return false;
    }

    #endregion

    #region Dash

    private void ResetDashValues()
    {
        _isDashFastFalling = false;
        _dashOnGroundTimer = -0.01f;
    }

    private void ResetDashes()
    {
        _numberOfDashesUsed = 0;
    }

    private void DashCheck()
    {
        if(InputManager.DashWasPressed)
        {
            //ground dash 
            if(_isGrounded && _dashOnGroundTimer < 0 && !_isDashing)
            {
                InitiateDash();
                StartDashEffect();
            }

            //air dash
            else if(!_isGrounded && !_isDashing && _numberOfDashesUsed < MoveStats.NumberOfDashes)
            {
                _isAirDashing = true;
                InitiateDash();
                StartDashEffect();

                //you left a wallslide but dashed withing the wall jump post buffer timer
                if(_wallJumpPostBufferTimer > 0f)
                {
                    _numberOfJumpsUsed--;
                    if(_numberOfJumpsUsed < 0f)
                    {
                        _numberOfJumpsUsed = 0;
                    }
                }
            }
        }
    }

    private void InitiateDash()
    {
        SoundEffectManager.Instance.PlaySoundEffect(_dashClip, transform, 1);
        
        _dashDirection = InputManager.Movement;

        Vector2 closesestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[0]);

        for(int i = 0; i < MoveStats.DashDirections.Length; i++)
        {
            //skip if we hit it bang on 
            if(_dashDirection == MoveStats.DashDirections[i])
            {
                closesestDirection = _dashDirection;
                break;
            }

            float distance = Vector2.Distance(_dashDirection, MoveStats.DashDirections[i]);

            //check if this is a diagonal direction and apply bias
            bool isdDiagonal = (Mathf.Abs(MoveStats.DashDirections[i].x) == 1 && Mathf.Abs(MoveStats.DashDirections[i].y) == 1);
            if(isdDiagonal)
            {
                distance -= MoveStats.DashDiagonallyBias;
            }
            else if(distance < minDistance)
            {
                minDistance = distance;
                closesestDirection = MoveStats.DashDirections[i];
            }
        }

        //handle direction with no inout
        if(closesestDirection == Vector2.zero)
        {
            if(_isFacingRight)
            {
                closesestDirection = Vector2.right;
            }
            else
            {
                closesestDirection = Vector2.left;
            }
        }

        _dashDirection = closesestDirection;
        _numberOfDashesUsed++;
        _isDashing = true;
        _dashTimer = 0f;
        _dashOnGroundTimer = MoveStats.TimeBetweenDashesOnGround;

        ResetJumpValues();
        ResetWallJumpValues();
        StopWallSlide();
    }

    private void Dash()
    {
        if (_isDashing)
        {
            //stop the dash after the timer 
            _dashTimer += Time.fixedDeltaTime;
            if(_dashTimer >= MoveStats.DashTime)
            {
                if(_isGrounded)
                {
                    ResetDashes();
                }

                _isAirDashing = false;
                _isDashing = false;

                if(!_isJumping && !_isWallJumping)
                {
                    _dashFastFallTime = 0f;
                    _dashFastFallReleaseSpeed = VerticalVelocity;

                    if(!_isGrounded)
                    {
                        _isDashFastFalling = true;
                    }
                }

                return;
            }

            HorizontalVelocity = MoveStats.DashSpeed * _dashDirection.x;

            if(_dashDirection.x != 0f || _isAirDashing)
            {
                VerticalVelocity = MoveStats.DashSpeed * _dashDirection.y;
            }
        }

        //handle dash cut time 
        else if(_isDashFastFalling)
        {
            if(VerticalVelocity > 0f)
            {
                if(_dashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity = Mathf.Lerp(_dashFastFallReleaseSpeed, 0f, (_dashFastFallTime / MoveStats.DashTimeForUpwardsCancel));
                }
                else if(_dashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    VerticalVelocity += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
                }

                _dashFastFallTime += Time.fixedDeltaTime;
            }

            else
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
        }
    }

    private void StartDashEffect()
    {
        if(DashEffectCoroutine() != null)
        {
            StopCoroutine(DashEffectCoroutine());
        }
        _dashEffectCoroutine = StartCoroutine(DashEffectCoroutine());
    }

    private void StopDashEffect()
    {
        if (DashEffectCoroutine() != null)
        {
            StopCoroutine(DashEffectCoroutine());
        }
    }

    #endregion

    #region Timer

    private void CountTimers()
    {
        //jump buffer
        _jumpBufferTimer -= Time.deltaTime;

        //jump coyote time
        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else _coyoteTimer = MoveStats.JumpCoyoteTime;

        //wall jump buffer timer
        if(!ShouldApplyPostWallJumpBuffer())
        {
            _wallJumpPostBufferTimer -= Time.deltaTime;
        }

        //dash timer
        if(_isGrounded)
        {
            _dashOnGroundTimer -= Time.deltaTime;
        }
    }

#endregion

    #region Jump Visualization

    public static float Map(float value, float min1, float max1, float min2, float max2, bool clamp = false)
    {
        float val = min2 + (max2 - min2) * ((value - min1) / (max1 - min1));
        return clamp ? Mathf.Clamp(val, Mathf.Min(min2, max2), Mathf.Max(min2, max2)) : val;
    }

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPos = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 previousPos = startPos;
        float speed = 0f;
        if (MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }

        Vector2 velocity = new Vector2(speed, MoveStats.InitialJumpVelocity);
        Gizmos.color = gizmoColor;
        float timeStep = 2 * MoveStats.TimeTillJumpApex / MoveStats.ArcResolution;

        for (int i = 0; i < MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;

            if (simulationTime < MoveStats.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveStats.Gravity) * simulationTime * simulationTime;
            }
            else if (simulationTime < MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime)
            {
                float apexTime = simulationTime - MoveStats.TimeTillJumpApex;
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }
            else
            {
                float descendTime = simulationTime - (MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime);
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveStats.ApexHangTime;
                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, MoveStats.Gravity) * descendTime * descendTime;
            }

            drawPoint = startPos + displacement;

            if (MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPos, drawPoint - previousPos, Vector2.Distance(previousPos, drawPoint), MoveStats.GroundLayer);
                if (hit.collider != null)
                {
                    Gizmos.DrawLine(previousPos, hit.point);
                    break;
                }

                Gizmos.DrawLine(previousPos, drawPoint);
                previousPos = drawPoint;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(MoveStats.ShowWalkJumpArc)
        {
            DrawJumpArc(MoveStats.MaxWalkSpeed, Color.white);
        }
        if(MoveStats.ShowRunJumpArc)
        {
            DrawJumpArc(MoveStats.MaxRunSpeed, Color.white);
        }
    }
    #endregion

    #region State Machine

    private void UpdateState()
    {
        switch(_state)
        {
            case PlayerStates.Idle:
                UpdateIdle();
                break;
            case PlayerStates.Run:
                UpdateRun();
                break;
            case PlayerStates.Airborne:
                UpdateAirborne();
                break;
            case PlayerStates.Sprint:
                UpdateSprint();
                break;
            case PlayerStates.WallJump:
                UpdateWallJump();
                break;
            case PlayerStates.WallSlide:
                UpdateWallSlide();
                break;
            case PlayerStates.Dash:
                UpdateDash();
                break;
            /*case PlayerStates.Kick:
                UpdateKick();
                break;*/
        }
    }

    #region UpdateAnim
    private void UpdateIdle()
    {
        if (InputManager.Movement.x != 0 || !_isGrounded) _stateComplete = true;
    }

    private void UpdateRun()
    {
        float floatX = HorizontalVelocity;
        Vector2 velX = new Vector2(floatX, transform.position.y);
        _animator.speed = Mathf.Abs(floatX) / MoveStats.MaxWalkSpeed;
        if (_isGrounded || Mathf.Abs(floatX) < 0.1f) _stateComplete = true;
    }

    private void UpdateAirborne()
    {
        float time = Map(VerticalVelocity, _fastFallReleaseSpeed, -_fastFallReleaseSpeed, 0, 1, true);
        _animator.Play("Player_Jump", 0, time);
        _animator.speed = 0;
        if (_isGrounded || _isTouchingWall || _isWallSliding) _stateComplete = true;
    }

    private void UpdateSprint()
    {
        float floatX = HorizontalVelocity;
        Vector2 velX = new Vector2(floatX, transform.position.y);
        _animator.speed = Mathf.Abs(floatX) / MoveStats.MaxRunSpeed;
        if (_isGrounded || Mathf.Abs(floatX) < 0.1f) _stateComplete = true;
    }

    private void UpdateWallSlide()
    {
        float floatY = VerticalVelocity;
        Vector2 velY = new Vector2(transform.position.x, floatY);
        if(_isGrounded || Mathf.Abs(floatY) < 0.1f || !_isTouchingWall || _isDashing) _stateComplete = true;
    }

    private void UpdateWallJump()
    {
        float time = Map(VerticalVelocity, _wallJumpFastFallReleaseSpeed, -_wallJumpFastFallReleaseSpeed, 0, 1, true);
        _animator.Play("Player_Wall_Jump", 0, time);
        _animator.speed = 0;
        if (_isGrounded || _isTouchingWall || _isWallSliding) _stateComplete = true;
    }

    private void UpdateDash()
    { 
        //_animator.speed = Mathf.Abs(_dashTimer) / MoveStats.DashTime;
        if (!_isDashing || !_isAirDashing) _stateComplete = true;
    }

    /*private void UpdateKick()
    {
        if (AnimComplete) _stateComplete = true;
    }*/
    #endregion

    private void SelectState()
    {
        _stateComplete = false;
        //on the ground
        if(_isGrounded)
        {
            /*if (InputManager.AttackWasPressed)
            {
                Debug.Log("Attack was pressed");
                _state = PlayerStates.Kick;
                StartKick();
            }*/
            if (InputManager.DashWasPressed && !_isDashing && _dashOnGroundTimer < 0) //if dash
            {
                _state = PlayerStates.Dash;
                StartDash();
            }
            if (InputManager.Movement.x == 0) //if idle
            {
                _state = PlayerStates.Idle;
                StartIdle();
            }
            else if (InputManager.RunIsHeld) //if sprint
            {
                _state = PlayerStates.Sprint;
                StartSprint();
            }
            else //if run
            {
                _state = PlayerStates.Run;
                StartRun();
            }
        }
        //not on the ground // !_isGrounded
        else if(InputManager.DashWasPressed && !_isDashing && _numberOfDashesUsed < MoveStats.NumberOfDashes) //if air dash
        {
            _state = PlayerStates.Dash;
            StartDash();
        }
        else if(InputManager.JumpWasReleased && !_isWallSliding && !_isTouchingWall && _isWallJumping) //if wall jump
        {
            _state = PlayerStates.WallJump;
            StartWallJump();
        }
        else if(_isTouchingWall && !_isDashing) //if wall slide
        {
            _state = PlayerStates.WallSlide;
            StartWallSlide();
        }
        else
        {
            _state = PlayerStates.Airborne;
            StartAirborne();
        }
    }

    #region StartAnim
    private void StartIdle()
    {
        _animator.Play("Player_Idle");
    }

    private void StartRun()
    {
        _animator.Play("Player_Run");
    }

    private void StartAirborne()
    {
        _animator.Play("Player_Jump");
    }

    private void StartSprint()
    {
        _animator.Play("Player_Sprint");
    }

    private void StartWallJump()
    {
        _animator.Play("Player_Wall_Jump");
    }

    private void StartWallSlide()
    {
        _animator.Play("Player_Wall_Slide");
    }

    private void StartDash()
    {
        _animator.Play("Player_Dash");
    }

    /*private void StartKick()
    {
        _animator.Play("Player_Kick_1");
    }

    public void StateComplete()
    {
        AnimComplete = true;
        yield return new WaitForSeconds(1);
        AnimComplete = false;
    }*/
    #endregion

    #endregion

    #region Coroutine

    IEnumerator DashEffectCoroutine()
    {
        for (int i = 0; i < Object_Pooling.Instance._amountToPool; i++)
        {
            GameObject ghost = Object_Pooling.Instance.GetPooledObject();
            if (ghost != null)
            {
                ghost.transform.position = this.transform.position;
                ghost.transform.rotation = this.transform.rotation;
                ghost.SetActive(true);

                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                ghost.GetComponent<SpriteRenderer>().sprite = currentSprite;

                SpriteRenderer currentSprite2 = GetComponent<SpriteRenderer>();
                currentSprite2 = ghost.GetComponent <SpriteRenderer>();
                currentSprite2.sortingOrder = -1;
                

                yield return new WaitForSeconds(MoveStats.GhostDelaySeconds);
                ghost.SetActive(false);
            }
        }
    }

    #endregion
}