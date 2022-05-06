namespace Com.GGIT.Security.JsonWebToken
{
    public class JwtSettings
    {
        private static JwtKeys Key;

        public static JwtKeys GetKey()
        {
            if (Key == null)
            {
                using JwtManager manager = new JwtManager();
                Key = manager.LoadKeys();
            }
            return Key;
        }

        public static void ResetKey()
        {
            Key = null;
        }
    }
}
