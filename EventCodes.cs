using System;

// Token: 0x0200000A RID: 10
internal enum EventCodes : byte
{
	// Token: 0x04000017 RID: 23
	VoiceData = 1,
	// Token: 0x04000018 RID: 24
	ServerMessage,
	// Token: 0x04000019 RID: 25
	MasterClientSync,
	// Token: 0x0400001A RID: 26
	CachedEvent,
	// Token: 0x0400001B RID: 27
	MasterClientSyncFinished,
	// Token: 0x0400001C RID: 28
	VRChatRPC,
	// Token: 0x0400001D RID: 29
	SerializedData,
	// Token: 0x0400001E RID: 30
	InterestManagement,
	// Token: 0x0400001F RID: 31
	SerializedDataReliable,
	// Token: 0x04000020 RID: 32
	Unknown10,
	// Token: 0x04000021 RID: 33
	Unknown11,
	// Token: 0x04000022 RID: 34
	SerializedDataUnreliable,
	// Token: 0x04000023 RID: 35
	ReliableFlatbuffer,
	// Token: 0x04000024 RID: 36
	ObjectSync = 16,
	// Token: 0x04000025 RID: 37
	Unknown17,
	// Token: 0x04000026 RID: 38
	JoinWorld = 20,
	// Token: 0x04000027 RID: 39
	ObjectOwnership = 22,
	// Token: 0x04000028 RID: 40
	Moderations = 33,
	// Token: 0x04000029 RID: 41
	PhotonEventLimits,
	// Token: 0x0400002A RID: 42
	PhotonHeartbeat,
	// Token: 0x0400002B RID: 43
	AvatarRefresh = 40,
	// Token: 0x0400002C RID: 44
	SetPlayerData = 42,
	// Token: 0x0400002D RID: 45
	Unknown43,
	// Token: 0x0400002E RID: 46
	OwnChatbox,
	// Token: 0x0400002F RID: 47
	OnLoaded = 51,
	// Token: 0x04000030 RID: 48
	PlayerReady = 53,
	// Token: 0x04000031 RID: 49
	PhysBonesPermissions = 60,
	// Token: 0x04000032 RID: 50
	EACHeartBeat = 66,
	// Token: 0x04000033 RID: 51
	Portal = 70,
	// Token: 0x04000034 RID: 52
	EmojiSend,
	// Token: 0x04000035 RID: 53
	PhotonRPC = 200,
	// Token: 0x04000036 RID: 54
	SendSerialize,
	// Token: 0x04000037 RID: 55
	Instantiate,
	// Token: 0x04000038 RID: 56
	CloseConnection,
	// Token: 0x04000039 RID: 57
	Destroy,
	// Token: 0x0400003A RID: 58
	RemoveCachedRPCs,
	// Token: 0x0400003B RID: 59
	SendSerializeReliable,
	// Token: 0x0400003C RID: 60
	DestroyPlayer,
	// Token: 0x0400003D RID: 61
	AssignMasterClient,
	// Token: 0x0400003E RID: 62
	OwnershipRequest,
	// Token: 0x0400003F RID: 63
	OwnershipTransfer,
	// Token: 0x04000040 RID: 64
	VacantViewIds,
	// Token: 0x04000041 RID: 65
	LevelReload,
	// Token: 0x04000042 RID: 66
	PhotonAppId = 220,
	// Token: 0x04000043 RID: 67
	AuthEvent = 223,
	// Token: 0x04000044 RID: 68
	LobbyStats,
	// Token: 0x04000045 RID: 69
	AppStats = 226,
	// Token: 0x04000046 RID: 70
	Match,
	// Token: 0x04000047 RID: 71
	QueueState,
	// Token: 0x04000048 RID: 72
	GameListUpdate,
	// Token: 0x04000049 RID: 73
	GameList,
	// Token: 0x0400004A RID: 74
	CacheSliceChanged = 250,
	// Token: 0x0400004B RID: 75
	ErrorInfo,
	// Token: 0x0400004C RID: 76
	SetProperties = 253,
	// Token: 0x0400004D RID: 77
	LeavePhoton,
	// Token: 0x0400004E RID: 78
	JoinPhoton
}
