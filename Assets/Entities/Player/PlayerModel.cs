using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerModel : EntityModel, IMovable, ILookable, ILoadoutable
{
    private readonly IMover _mover;
    private readonly IInputGetter _inputGetter;
    private readonly ILooker _looker;

    private bool _isJumping;
    private bool _isUsingItem;
    private Vector2 _lootAt;
    private readonly LoadoutModel _loadout;
    private readonly InventoryModel _inventory;

    public bool IsJumping => _isJumping;
    public bool IsMoving => _inputGetter.MoveAxis != 0 || !_mover.IsGrounded;

    public Vector2 LookAt => _lootAt;

    public LoadoutModel Loadout => _loadout;

    public InventoryModel Inventory => _inventory;

    public event Action OnJetpack;

    public PlayerModel(
        PlayerData data,
        IProjectileSpawner projectileSpawner,
        IMover mover,
        IInputGetter inputGetter,
        ILooker looker) : base(data, projectileSpawner)
    {
        _mover = mover;
        _inputGetter = inputGetter;
        _looker = looker;

        _loadout = new LoadoutModel(data.GunSlot, data.GearSlot, data.ImplantSlot, this);
        _inventory = new InventoryModel(Constants.Instance.InventorySlotsPerRow, data.InventorySize);
    }

    public override void Tick(float deltaTime, Vector2 position)
    {
        base.Tick(deltaTime, position);

        _loadout.Tick(deltaTime, this);

        Walk(_inputGetter.MoveAxis * Stats.MoveSpeed.Value);

        if (_mover.IsGrounded)
        {
            if (_inputGetter.JumpDown)
            {
                Jump(new Vector2(0, Stats.JumpHeight.Value));
                _isJumping = true;
            }
        }
        else
        {
            if (_inputGetter.JetpackHold && !_isJumping)
            {
                OnJetpack?.Invoke();
                _mover.SetMoveY(Stats.JetpackPower.Value);
            }
        }

        if (_inputGetter.JumpUp)
        {
            _isJumping = false;
        }

        //�}�E�X�J�[�\���̕�������
        _lootAt = _inputGetter.MousePos;
        Look(_lootAt, Stats.FOVAngle.Value, Stats.SightRange.Value);


        //�莝���A�C�e���؂�ւ�(�A�C�e���g�p���͕s��)
        if (!_isUsingItem)
        {
            if (_inputGetter.SwapItem_Next)
            {
                _loadout.SwitchNext();
            }
            else if (_inputGetter.SwapItem_Back)
            {
                _loadout.SwitchBack();
            }
        }
        

        //�A�C�e���g�p/�g�p���~
        if (_inputGetter.UseDown)
        {
            _isUsingItem = true;
            _loadout.HoldingItem?.Use(this);
        }

        if(_inputGetter.UseUp)
        {
            if (_isUsingItem)
            {
                _loadout.HoldingItem?.StopUsing(this);
                _isUsingItem = false;
            }
        }

        if(_inputGetter.Reload) _loadout.HoldingItem?.Reload(this);
    }

    public void Walk(float walk)
    {
        _mover.Walk(walk);
    }

    public void Jump(Vector2 jump)
    {
        _mover.Jump(jump);
    }

    public void Look(Vector2 lookAt, float angle, float range)
    {
        _looker.Look(lookAt, angle, range);
    }
}