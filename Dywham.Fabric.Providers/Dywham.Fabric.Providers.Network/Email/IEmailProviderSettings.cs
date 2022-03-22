namespace Dywham.Fabric.Providers.Network.Email
{
    public interface IEmailProviderSettings
    {
        string EmailProviderFrom { get; set; }

        string EmailProviderFromDisplay { get; set; }

        string EmailProviderHost { get; set; }

        int EmailProviderPort { get; set; }

        bool UseDefaultCredentials { get; set; }

        bool EmailProviderUseSsl { get; set; }

        string EmailProviderUsername { get; set; }

        string EmailProviderPassword { get; set; }
    }
}
