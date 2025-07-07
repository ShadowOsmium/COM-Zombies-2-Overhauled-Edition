public class PlayerID : System.IEquatable<PlayerID>
{
    public AvatarType avatar_type = AvatarType.None;
    public AvatarData.AvatarState avatar_state = AvatarData.AvatarState.Normal;
    public string player_name = string.Empty;
    public int tnet_id = -1;

    public PlayerID(AvatarType type, AvatarData.AvatarState state, string name, int id)
    {
        avatar_type = type;
        avatar_state = state;
        player_name = name;
        tnet_id = id;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is PlayerID))
            return false;
        return Equals((PlayerID)obj);
    }

    public bool Equals(PlayerID other)
    {
        if (other == null)
            return false;
        return avatar_type == other.avatar_type &&
               avatar_state == other.avatar_state &&
               player_name == other.player_name &&
               tnet_id == other.tnet_id;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + avatar_type.GetHashCode();
            hash = hash * 23 + avatar_state.GetHashCode();
            hash = hash * 23 + (player_name != null ? player_name.GetHashCode() : 0);
            hash = hash * 23 + tnet_id.GetHashCode();
            return hash;
        }
    }
}