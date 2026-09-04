using ProtoBuf;

namespace LevelUP;

[ProtoContract]
public class ServerMessage
{
    [ProtoMember(1)]
    public string message;
}
