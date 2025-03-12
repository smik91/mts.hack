namespace Midiot.Common.Helpers;

public static class CodeCreator
{
    public static string GenerateCode()
    {
        Random random = new Random();
        int code = random.Next(100000, 1000000);
        return code.ToString();
    }
}
