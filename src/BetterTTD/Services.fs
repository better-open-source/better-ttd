module BetterTTD.ServicesModule

open BetterTTD.ActorMessagesModule
open BetterTTD.PacketTransformers

type IServerSubscriber =
    abstract OnPacketReceived : PacketMessage     -> unit

type IServerNotifier =
    abstract SendConnect      : ConnectMessage    -> unit
    abstract SendPollClient   : PollClientMessage -> unit