namespace Dywham.Fabric.Providers.Network.Email
{
    public class SenderDetails
    {
        public string EmailProviderFrom { get; set; }

        public string EmailProviderFromDisplay { get; set; }

        public string EmailProviderHost { get; set; }

        public int EmailProviderPort { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public bool EmailProviderUseSsl { get; set; }

        public string EmailProviderUsername { get; set; }

        public string EmailProviderPassword { get; set; }
    }
}