using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

class PacketHandler
{
	#region Town

	public static void S_EnterHandler(PacketSession session, IMessage packet)
	{
        S_Enter enterPacket = packet as S_Enter;
        if (enterPacket == null)
	        return;
		TownManager.Instance.Spawn(enterPacket.Player);
	}

	public static void S_LeaveHandler(PacketSession session, IMessage packet) { }

	// 본인 화면에 다른 유저 모습 보이게 만들기
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		if (spawnPacket == null)
			return;

		// 플레이어 리스트를 가져옵니다.(어떤 형식인지는 아직 잘 모르겠습니다.)
		var playerList = spawnPacket.Players;
		foreach (var playerInfo in playerList)
		{
			// 플레이어 리스트를 반복 순회하면서
			var tr = playerInfo.Transform;
			// 플레이어(해당 유저)의 좌표, 회전 값 담기
			var player = TownManager.Instance.CreatePlayer(playerInfo, new Vector3(tr.PosX, tr.PosY, tr.PosZ));
			// 마을에 있는 타인에게 본인 그려주기
			player.SetIsMine(false);
			// 나인가 유무 = 아니다.
		}
	}

	public static void S_Reward(PacketSession session, IMessage packet)
	{
		// 아직 S_Reward가 없기 때문에
		// S_Chat이 S_Reward라는 가정하에 코드를 작성하고 있습니다.
		// S_Reward로 바꾸기 아직
		S_Chat rewardPacket = packet as S_Chat;
		if(rewardPacket == null)
			return;

		//아래 주석 코드는 안 쓰기로 결정난 코드
		//var player = TownManager.Instance.GetPlayerAvatarById(rewardPacket.PlayerId);

		// if(player)
		// {
		 	// rewardPacket으로 부터 받은 money 돈을
		 	// setGold로 서버가 제공하는 골드를 가져오기
		 	// player.RecvGold(rewardPacket.gold);
		// }
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		// 서버와의 접속을 끊을 클라이언트, 혹은 던전 입장할 경우

		S_Despawn despawnPacket = packet as S_Despawn;
		//IMessage로 온 코드를 S_Despawn으로 정의

		if (despawnPacket == null)
			return;
		// 없을 경우 null 반환

		foreach (var playerId in despawnPacket.PlayerIds)
		{
			TownManager.Instance.ReleasePlayer(playerId);
			// Release 뜻 중 하나는 "방출하다"라서
			// 방출할 플레이어를 방출해준다.
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		// 케릭터 이동

		S_Move movePacket = packet as S_Move;

		if (movePacket == null)
			return;
		// packet이 없으면 종료


		var tr = movePacket.Transform;
		Vector3 move = new Vector3(tr.PosX, tr.PosY, tr.PosZ);
		Vector3 eRot = new Vector3(0, tr.Rot, 0);
		// 위치 회전값 가져오기

		var player = TownManager.Instance.GetPlayerAvatarById(movePacket.PlayerId);
		// 해당 플레이어의 Id를 바탕으로 유저 정보를 가져온다.
		if (player)
		{	// 해당 유저가 있으면, null, undefined가 아니므로
			// true이기 때문에 if문 동작
			player.Move(move, Quaternion.Euler(eRot));
			// Move에 move : 좌표
			// Quaternion.Euler(eRot) : 회전값을 제공합니다.
			// Quaternion.Euler를 사용하는 이유는 짐벌락 문제를 해결하기 위해서 사용한다.
		}
	}

	public static void S_AnimationHandler(PacketSession session, IMessage packet)
	{
		S_Animation animationPacket = packet as S_Animation;
		if (animationPacket == null)
			return;

		var animCode = animationPacket.AnimCode;
		// Hash(int) 형태의 실행시킬 AnimationCode를 가져온다.

		var player = TownManager.Instance.GetPlayerAvatarById(animationPacket.PlayerId);
		// 해당 플레이어의 Id를 바탕으로 유저 정보를 가져온다.

		if (player)
		{
			player.Animation(animCode);
			// 실행시킬 Animation 코드를 함수에 제공(함수 내 설명이 있어요)
		}
	}

	public static void S_ChangeCostumeHandler(PacketSession session, IMessage packet) { }

	public static void S_ChatHandler(PacketSession session, IMessage packet)
	{
		S_Chat chatPacket = packet as S_Chat;
		if (chatPacket == null)
			return;
		// chatPacket이 없으면 return(종료)

		var msg = chatPacket.ChatMsg;

		var player = TownManager.Instance.GetPlayerAvatarById(chatPacket.PlayerId);
		// id에 따른 유저의 모든 정보를 가져온다.

		if (player)
		{
			player.RecvMessage(msg);
			//ReceptionMessage 즉, 서버에서 온 메시지 수신한다.
		}
	}

	#endregion


	#region Battle


	public static void S_EnterDungeonHandler(PacketSession session, IMessage packet)
	{
		S_EnterDungeon pkt = packet as S_EnterDungeon;
		if (pkt == null)
			return;

		Scene scene = SceneManager.GetActiveScene();

		if (scene.name == GameManager.BattleScene)
		{
			BattleManager.Instance.Set(pkt);
		}
		else
		{
			GameManager.Instance.Pkt = pkt;
			SceneManager.LoadScene(GameManager.BattleScene);
		}
	}

	public static void S_LeaveDungeonHandler(PacketSession session, IMessage packet)
	{
		S_LeaveDungeon pkt = packet as S_LeaveDungeon;
		if (pkt == null)
			return;

		SceneManager.LoadScene(GameManager.TownScene);
	}

	public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
	{
		S_ScreenText pkt = packet as S_ScreenText;
		if (pkt == null)
			return;

		if (pkt.ScreenText != null)
		{
			var uiScreen = BattleManager.Instance.UiScreen;
			uiScreen.Set(pkt.ScreenText);
		}
	}

	public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
	{
		S_ScreenDone pkt = packet as S_ScreenDone;
		if (pkt == null)
			return;

		var uiScreen = BattleManager.Instance.UiScreen;
		uiScreen.gameObject.SetActive(false);
	}

	public static void S_BattleLogHandler(PacketSession session, IMessage packet)
	{
		S_BattleLog pkt = packet as S_BattleLog;
		if (pkt == null)
			return;

		if (pkt.BattleLog != null)
		{
			var uiBattleLog = BattleManager.Instance.UiBattleLog;
			uiBattleLog.Set(pkt.BattleLog);
		}
	}

	public static void S_SetPlayerHpHandler(PacketSession session, IMessage packet)
	{
		S_SetPlayerHp pkt = packet as S_SetPlayerHp;
		if (pkt == null)
			return;

		var uiPlayer = BattleManager.Instance.UiPlayerInformation;
		uiPlayer.SetCurHP(pkt.Hp);
	}

	public static void S_SetPlayerMpHandler(PacketSession session, IMessage packet)
	{
		S_SetPlayerMp pkt = packet as S_SetPlayerMp;
		if (pkt == null)
			return;

		var uiPlayer = BattleManager.Instance.UiPlayerInformation;
		uiPlayer.SetCurMP(pkt.Mp);
	}

	public static void S_SetMonsterHpHandler(PacketSession session, IMessage packet)
	{
		S_SetMonsterHp pkt = packet as S_SetMonsterHp;
		if (pkt == null)
			return;

		BattleManager.Instance.SetMonsterHp(pkt.MonsterIdx, pkt.Hp);
	}

	public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
	{
		S_PlayerAction pkt = packet as S_PlayerAction;
		if (pkt == null)
			return;

		Monster monster = BattleManager.Instance.GetMonster(pkt.TargetMonsterIdx);
		monster.Hit();

		BattleManager.Instance.PlayerAnim(pkt.ActionSet.AnimCode);
		EffectManager.Instance.SetEffectToMonster(pkt.TargetMonsterIdx, pkt.ActionSet.EffectCode);
	}

	public static void S_MonsterActionHandler(PacketSession session, IMessage packet)
	{
		S_MonsterAction pkt = packet as S_MonsterAction;
		if (pkt == null)
			return;

		Monster monster = BattleManager.Instance.GetMonster(pkt.ActionMonsterIdx);
		if(monster)monster.SetAnim(pkt.ActionSet.AnimCode);

		if(pkt.ActionSet.AnimCode != 4) BattleManager.Instance.PlayerHit();
		EffectManager.Instance.SetEffectToPlayer(pkt.ActionSet.EffectCode);
	}
	#endregion


	#region Pvp

	public static void S_PlayerMatchHandler(PacketSession session, IMessage packet)
	{
		S_PlayerMatch matchPacket = packet as S_PlayerMatch;
		if(matchPacket == null)
			return;
		//TownManager.Instance
	}

	public static void S_PlayerMatchNotificationHandler(PacketSession session, IMessage packet)
	{
		S_PlayerMatchNotification matchPacket = packet as S_PlayerMatchNotification;
		if(matchPacket == null)
			return;
		Scene scene = SceneManager.GetActiveScene();

		if(scene.name == GameManager.PvpScene)
		{
			Debug.Log("if 경우");
			PvpBattleManager.Instance.Set(matchPacket);
		}
		else
		{
			Debug.Log("그 밖으로 빠지는 경우");
			GameManager.Instance.Pvp = matchPacket;
			SceneManager.LoadScene(GameManager.PvpScene);
		}
	}

	public static void S_HitAnimationNotificationHandler(PacketSession session, IMessage packet)
	{
		S_HitAnimationNotification hitPacket = packet as S_HitAnimationNotification;
		Debug.Log("동작 확인 : "+hitPacket);
		if(hitPacket == null)
			return;
		PvpBattleManager.Instance.HitAnimation(hitPacket);
	}

	public static void S_BeatenAnimationNotificationHandler(PacketSession session, IMessage packet)
	{
		S_BeatenAnimationNotification beatenPacket = packet as S_BeatenAnimationNotification;

		if(beatenPacket == null)
			return;
		PvpBattleManager.Instance.BeatenAnimation(beatenPacket);
	}

	// public static void S_PlayerStrikeFirstNotificationHandler(PacketSession session, IMessage packet)
	// {
	// 	S_PlayerStrikeFirstNotification firstPacket = packet as S_PlayerStrikeFirstNotification;

	// 	if(firstPacket == null)
	// 		return;

	// 	PvpBattleManager.Instance.StrikeFirstNotification(firstPacket);
	// }
	public static void S_UserTurnHandler(PacketSession session, IMessage packet)
	{
		S_UserTurn turnPacket = packet as S_UserTurn;
		if(turnPacket == null)
			return;
		Debug.Log("턴 유무 확인 :"+turnPacket.UserTurn);
		PvpBattleManager.Instance.CheckUserTurn(turnPacket.UserTurn);
	}

	public static void S_PvpBattleLogHandler(PacketSession session, IMessage packet)
	{
		S_PvpBattleLog battleLogPacket =  packet as S_PvpBattleLog;

		if(battleLogPacket == null)
			return;

		if(battleLogPacket.BattleLog != null)
		{
			var pvpUiBattleLog = PvpBattleManager.Instance.PvpUiBattleLog;
			pvpUiBattleLog.Set(battleLogPacket.BattleLog);
		}
	}

	/*
	    _onRecv.Add((ushort)MsgId.SPlayerCurrencyNotification, MakePacket<S_PlayerCurrencyNotification>);
        _handler.Add((ushort)MsgId.SPlayerCurrencyNotification, PacketHandler.S_PlayerCurrencyNotificationHandler);


        _onRecv.Add((ushort)MsgId.SEnemyActionNotification, MakePacket<S_EnemyActionNotification>);
        _handler.Add((ushort)MsgId.SEnemyActionNotification, PacketHandler.S_EnemyActionNotificationHandler);
	*/

	// public static void S_EnemyActionNotificationHandler(PacketSession session, IMessage packet)
	// {
	// 	S_EnemyActionNotification enemyActionPacket = packet as S_EnemyActionNotification;

	// 	if(enemyActionPacket == null)
	// 		return;
		
	// }

	#endregion
}

