using UnityEngine;
using UnityEngine.InputSystem;

public class MyPlayer : Player
{
    private Vector2 moveInput;
    private float sendInterval = 0.05f;
    private float sendTimer;
    private bool wasMoved;

    public bool IsAutoMove { get; set; } //로컬 네트워크라서 테스트용..

    private void Update()
    {
        if (IsAutoMove)
            return;

        HandleInput();
        MovementClientBase();
        HandleFlip(moveInput);
        HandleAnimation(moveInput);
    }

    private void HandleInput()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null)
            return;

        moveInput = Vector2.zero;

        if (kb.wKey.isPressed) moveInput.y += 1f;
        if (kb.sKey.isPressed) moveInput.y -= 1f;
        if (kb.aKey.isPressed) moveInput.x -= 1f;
        if (kb.dKey.isPressed) moveInput.x += 1f;

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();
    }

    private void MovementClientBase()
    {
        Movement(moveInput * (moveSpeed * Time.deltaTime), Space.World);

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        sendTimer += Time.deltaTime;
        if (sendTimer >= sendInterval && isMoving)
        {
            sendTimer = 0f;
            PACKET_REQ_MOVE();
        }

        if (!isMoving && wasMoved)
            PACKET_REQ_MOVE();

        wasMoved = isMoving;
    }

    //Guest 클라 테스트용..
    public void AutoMoveTo(Vector2 direction)
    {
        Vector2 delta = direction.normalized * moveSpeed * Time.deltaTime;
        Movement(delta, Space.World);
        HandleFlip(direction);
        HandleAnimation(direction);

        sendTimer += Time.deltaTime;
        if (sendTimer >= sendInterval)
        {
            sendTimer = 0f;
            moveInput = direction.normalized;
            PACKET_REQ_MOVE();
            moveInput = Vector2.zero;
        }
    }

    private void PACKET_REQ_MOVE()
    {
        REQ_PLAYER_MOVE packet = new();
        packet.AccountId = accountId;
        packet.PosX = transform.position.x;
        packet.PosY = transform.position.y;
        packet.DirX = moveInput.x;
        packet.DirY = moveInput.y;
        packet.MoveSpeed = moveSpeed;
        Managers.Instance.Network.Udp.Send(PacketId.MSG_REQ_MOVE, packet);
    }
}
