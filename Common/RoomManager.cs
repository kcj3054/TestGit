namespace Common;


//수정 
public class RoomManager
{
    public List<Room>? RoomList { get; set; }
}

public class Room
{
    public List<User>? UserList
    {
        get;
        set;
    }

    public int RoomNumber
    {
        get;
        set;
    }
}

public class User
{
    public int? UserId
    {
        get;
        set;
    }

    public int? Value
    {
        get;
        set;
    }
}