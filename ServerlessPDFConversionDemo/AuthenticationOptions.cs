namespace ServerlessPDFConversionDemo
{
    public class AuthenticationOptions
    {
        public string Endpoint { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
        public string Resource { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
