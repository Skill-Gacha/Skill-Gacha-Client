using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.SEnter, MakePacket<S_Enter>);
        _handler.Add((ushort)MsgId.SEnter, PacketHandler.S_EnterHandler);
        _onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);
        _onRecv.Add((ushort)MsgId.SLeave, MakePacket<S_Leave>);
        _handler.Add((ushort)MsgId.SLeave, PacketHandler.S_LeaveHandler);
        _onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
        _handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);
        _onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
        _handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);
        _onRecv.Add((ushort)MsgId.SAnimation, MakePacket<S_Animation>);
        _handler.Add((ushort)MsgId.SAnimation, PacketHandler.S_AnimationHandler);
        _onRecv.Add((ushort)MsgId.SChangeCostume, MakePacket<S_ChangeCostume>);
        _handler.Add((ushort)MsgId.SChangeCostume, PacketHandler.S_ChangeCostumeHandler);
        _onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
        _handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);
        _onRecv.Add((ushort)MsgId.SEnterDungeon, MakePacket<S_EnterDungeon>);
        _handler.Add((ushort)MsgId.SEnterDungeon, PacketHandler.S_EnterDungeonHandler);
        _onRecv.Add((ushort)MsgId.SLeaveDungeon, MakePacket<S_LeaveDungeon>);
        _handler.Add((ushort)MsgId.SLeaveDungeon, PacketHandler.S_LeaveDungeonHandler);
        _onRecv.Add((ushort)MsgId.SScreenText, MakePacket<S_ScreenText>);
        _handler.Add((ushort)MsgId.SScreenText, PacketHandler.S_ScreenTextHandler);
        _onRecv.Add((ushort)MsgId.SScreenDone, MakePacket<S_ScreenDone>);
        _handler.Add((ushort)MsgId.SScreenDone, PacketHandler.S_ScreenDoneHandler);
        _onRecv.Add((ushort)MsgId.SBattleLog, MakePacket<S_BattleLog>);
        _handler.Add((ushort)MsgId.SBattleLog, PacketHandler.S_BattleLogHandler);
        _onRecv.Add((ushort)MsgId.SSetPlayerHp, MakePacket<S_SetPlayerHp>);
        _handler.Add((ushort)MsgId.SSetPlayerHp, PacketHandler.S_SetPlayerHpHandler);
        _onRecv.Add((ushort)MsgId.SSetPlayerMp, MakePacket<S_SetPlayerMp>);
        _handler.Add((ushort)MsgId.SSetPlayerMp, PacketHandler.S_SetPlayerMpHandler);
        _onRecv.Add((ushort)MsgId.SSetMonsterHp, MakePacket<S_SetMonsterHp>);
        _handler.Add((ushort)MsgId.SSetMonsterHp, PacketHandler.S_SetMonsterHpHandler);
        _onRecv.Add((ushort)MsgId.SPlayerAction, MakePacket<S_PlayerAction>);
        _handler.Add((ushort)MsgId.SPlayerAction, PacketHandler.S_PlayerActionHandler);
        _onRecv.Add((ushort)MsgId.SMonsterAction, MakePacket<S_MonsterAction>);
        _handler.Add((ushort)MsgId.SMonsterAction, PacketHandler.S_MonsterActionHandler);

        // 패킷 추가 등록
        // _onRecv.Add((ushort)MsgId.SViewRankPoint, MakePacket<S_ViewRankPoint>);
        // _handler.Add((ushort)MsgId.SViewRankPoint, PacketHandler.S_ViewRankPointHandler);

        // _onRecv.Add((ushort)MsgId.SPlayerCurrencyNotification, MakePacket<S_PlayerCurrencyNotification>);
        // _handler.Add((ushort)MsgId.SPlayerCurrencyNotification, PacketHandler.S_PlayerCurrencyNotificationHandler);

        _onRecv.Add((ushort)MsgId.SPlayerMatch, MakePacket<S_PlayerMatch>);
        _handler.Add((ushort)MsgId.SPlayerMatch, PacketHandler.S_PlayerMatchHandler);

        _onRecv.Add((ushort)MsgId.SPlayerMatchNotification, MakePacket<S_PlayerMatchNotification>);
        _handler.Add((ushort)MsgId.SPlayerMatchNotification, PacketHandler.S_PlayerMatchNotificationHandler);

        _onRecv.Add((ushort)MsgId.SUserTurn, MakePacket<S_UserTurn>);
        _handler.Add((ushort)MsgId.SUserTurn, PacketHandler.S_UserTurnHandler);

        _onRecv.Add((ushort)MsgId.SSetEnemyHp, MakePacket<S_SetPvpEnemyHp>);
        _handler.Add((ushort)MsgId.SSetEnemyHp, PacketHandler.S_SetEnemyHpHandler);

		_onRecv.Add((ushort)MsgId.SPvpBattleLog, MakePacket<S_PvpBattleLog>);
        _handler.Add((ushort)MsgId.SPvpBattleLog, PacketHandler.S_PvpBattleLogHandler);

		_onRecv.Add((ushort)MsgId.SPvpPlayerAction, MakePacket<S_PvpPlayerAction>);
        _handler.Add((ushort)MsgId.SPvpPlayerAction, PacketHandler.S_PvpPlayerActionHandler);

		_onRecv.Add((ushort)MsgId.SPvpEnemyAction, MakePacket<S_PvpEnemyAction>);
        _handler.Add((ushort)MsgId.SPvpEnemyAction, PacketHandler.S_PvpEnemyActionHandler);

        _onRecv.Add((ushort)MsgId.SSetPvpPlayerHp, MakePacket<S_SetPvpPlayerHp>);
        _handler.Add((ushort)MsgId.SSetPvpPlayerHp, PacketHandler.S_SetPvpPlayerHpHandler);

        _onRecv.Add((ushort)MsgId.SSetPvpPlayerMp, MakePacket<S_SetPvpPlayerMp>);
        _handler.Add((ushort)MsgId.SSetPvpPlayerMp, PacketHandler.S_SetPvpPlayerMpHandler);

        _onRecv.Add((ushort)MsgId.SOpenStoreResponse, MakePacket<S_OpenStoreResponse>);
        _handler.Add((ushort)MsgId.SOpenStoreResponse, PacketHandler.S_OpenStoreResponseHandler);

        _onRecv.Add((ushort)MsgId.SBuyItemResponse, MakePacket<S_BuyItemResponse>);
        _handler.Add((ushort)MsgId.SBuyItemResponse, PacketHandler.S_BuyItemResponseHandler);

        _onRecv.Add((ushort)MsgId.SInventoryViewResponse, MakePacket<S_InventoryViewResponse>);
        _handler.Add((ushort)MsgId.SInventoryViewResponse, PacketHandler.S_InventoryViewResponseHandler);

        _onRecv.Add((ushort)MsgId.SViewRankPoint, MakePacket<S_ViewRankPoint>);
        _handler.Add((ushort)MsgId.SViewRankPoint, PacketHandler.S_ViewRankPointHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		// 크기를 4바이트로 읽음
		int size = BitConverter.ToInt32(buffer.Array, buffer.Offset);
		count += 4;

		// 아이디를 1바이트로 읽음
		byte id = buffer.Array[buffer.Offset + count];
		count += 1;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 5, buffer.Count - 5);

		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}