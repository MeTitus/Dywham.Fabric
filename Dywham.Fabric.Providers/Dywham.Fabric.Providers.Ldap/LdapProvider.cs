using System.DirectoryServices;

namespace Dywham.Fabric.Providers.Ldap
{
#pragma warning disable CA1416 // Validate platform compatibility
    public class LdapProvider : ILdapProvider
    {
        public bool TryValidateUser(string ldapServer, string username, string password, out LdapUserInfo userInfo)
        {
            var directoryEntry = new DirectoryEntry(ldapServer, username, password);
            var searcher = new DirectorySearcher(directoryEntry)
            {
                Filter = $"(&(objectCategory=User)(objectClass=person)(samaccountname={username}))"
            };

            try
            {
                userInfo = BuildLdapUserInfo(searcher.FindOne());

                return true;
            }
            catch
            {
                userInfo = null;

                return false;
            }
        }

        public LdapUserInfo GetUserInfoByEmployeeId(string ldapServer, string id)
        {
            var directoryEntry = new DirectoryEntry(ldapServer);
            var searcher = new DirectorySearcher(directoryEntry)
            {
                Filter = $"(&(objectCategory=User)(objectClass=person)(employeeid={id}))"
            };
            var searchResult = searcher.FindOne();

            return BuildLdapUserInfo(searchResult);
        }

        public LdapUserInfo GetUserInfoByUsername(string ldapServer, string username)
        {
            var directoryEntry = new DirectoryEntry(ldapServer);
            var searcher = new DirectorySearcher(directoryEntry)
            {
                Filter = $"(&(objectCategory=User)(objectClass=person)(samaccountname={username}))"
            };
            var searchResult = searcher.FindOne();

            return BuildLdapUserInfo(searchResult);
        }

        public LdapUserInfo GetUserInfoByMailNickname(string ldapServer, string mailNickName)
        {
            var directoryEntry = new DirectoryEntry(ldapServer);
            var searcher = new DirectorySearcher(directoryEntry)
            {
                Filter = $"(&(objectCategory=User)(objectClass=person)(mailnickname={mailNickName}))"
            };
            var searchResult = searcher.FindOne();

            return BuildLdapUserInfo(searchResult);
        }

        public LdapUserInfo GetUserInfoByEmployeeEmail(string ldapServer, string email)
        {
            var directoryEntry = new DirectoryEntry(ldapServer);
            var searcher = new DirectorySearcher(directoryEntry)
            {
                Filter = $"(&(objectCategory=User)(objectClass=person)(mail={email}))"
            };
            var searchResult = searcher.FindOne();

            return BuildLdapUserInfo(searchResult);
        }

        private static LdapUserInfo BuildLdapUserInfo(SearchResult searchResult)
        {
            return new LdapUserInfo
            {
                Email = GetPropertyValue(searchResult, "mail"),
                EmployeeId = GetPropertyValue(searchResult, "employeeid"),
                FullName = GetPropertyValue(searchResult, "cn"),
                FirstName = GetPropertyValue(searchResult, "givenName"),
                LastName = GetPropertyValue(searchResult, "sn"),
            };
        }

        private static string GetPropertyValue(SearchResult searchResult, string name)
        {
            var valueCollection = searchResult.Properties[name];

            foreach (var item in valueCollection)
            {
                return item.ToString();
            }

            return null;
        }
    }
#pragma warning restore CA1416 // Validate platform compatibility
}