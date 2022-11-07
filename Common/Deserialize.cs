namespace Common;

public class Deserialize
{
    public User UserDesirialx(string recvData)
    {
        Int32 userId = ' ';
        Int32 userValue = ' ';
        bool first = true;
        
        for (int i = 1; i < recvData.Length; ++i)
        {
        
            if (recvData[i] == ':' && first)
            {
                userId = (int)Char.GetNumericValue(recvData[i + 1]);
                first = false;
            }
            else if (recvData[i] == ':')
            {
                userValue = (int)Char.GetNumericValue(recvData[i + 1]);
                break;
            }
        }

        User user = new User()
        {
            UserId = userId,
            Value = userValue
        };

        return user;
    }
}