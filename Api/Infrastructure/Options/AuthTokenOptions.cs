namespace Api.Infrastructure.Options
{
    /// <summary>
    /// Authentication token options
    /// </summary>
    public class AuthTokenOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Token duration
        /// </summary>
        public int TokenDuration { get; set; }
    }
}
