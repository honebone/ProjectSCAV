using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : EntityModel, IMovable, ILookable, ILoadoutable
{
    private readonly IMover _mover;
    private readonly IInputGetter _inputGetter;
    private readonly ILooker _looker;

    private StatValue _jetpackPower;

    private bool _isJumping;
    public bool IsJumping => _isJumping;

    private Vector2 _lootAt;
    public Vector2 LookAt => _lootAt;

    private readonly LoadoutModel _loadout;
    public LoadoutModel Loadout => _loadout;

    private readonly InventoryModel _inventory;
    public InventoryModel Inventory => _inventory;

    public PlayerModel(
        PlayerData data,
        float jetpackPower,
        IProjectileSpawner projectileSpawner,
        IMover mover,
        IInputGetter inputGetter,
        ILooker looker) : base(data, projectileSpawner)
    {
        _jetpackPower = new StatValue(jetpackPower);
        _mover = mover;
        _inputGetter = inputGetter;
        _looker = looker;

        _loadout = new LoadoutModel(data.GunSlot, data.GearSlot, data.ImplantSlot);
        _inventory = new InventoryModel(Constants.Instance.InventorySlotsPerRow, data.InventorySize);
    }

    public override void Tick(float deltaTime, Vector2 position)
    {
        base.Tick(deltaTime, position);

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
                _mover.SetMoveY(_jetpackPower.Value);
            }
        }

        if (_inputGetter.JumpUp)
        {
            _isJumping = false;
        }

        _lootAt = _inputGetter.MousePos;
        Look(_lootAt, Stats.FOVAngle.Value, Stats.SightRange.Value);

        if (_inputGetter.SwapItem_Next)
        {
            _loadout.SwitchNext();
        }
        else if (_inputGetter.SwapItem_Back)
        {
            _loadout.SwitchBack();
        }

        if (_inputGetter.UseDown)
        {
            _loadout.HoldingItem?.Use(this);
        }
    }

    public void Walk(float walk)
    {
        _mover.SetMoveX(walk);
    }

    public void Jump(Vector2 jump)
    {
        _mover.SetMoveY(jump.y);
    }

    public void Look(Vector2 lookAt, float angle, float range)
    {
        _looker.Look(lookAt, angle, range);
    }
}