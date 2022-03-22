using Dywham.Fabric.Providers;

namespace Dywham.Fabric.Providers.Ldap
{
    public interface ILdapProvider : IProvider
    {
        bool TryValidateUser(string ldapServer, string username, string password, out LdapUserInfo userInfo);

        LdapUserInfo GetUserInfoByEmployeeId(string ldapServer, string id);

        LdapUserInfo GetUserInfoByEmployeeEmail(string ldapServer, string email);

        LdapUserInfo GetUserInfoByUsername(string ldapServer, string username);
    }
}