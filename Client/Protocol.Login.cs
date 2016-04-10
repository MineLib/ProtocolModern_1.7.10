using System.Threading.Tasks;

namespace ProtocolModern.Client
{
    public sealed partial class Protocol
    {
        private string AccessToken { get; set; }
        private string ClientToken { get; set; }
        private string SelectedProfile { get; set; }


        public override async Task<bool> Login(string login, string password)
        {
            var result = await Yggdrasil.Authenticate(login, password);

            switch (result.Status)
            {
                case YggdrasilResponse.Success:
                    AccessToken                 = result.Info.AccessToken;
                    ClientToken                 = result.Info.ClientToken;
                    SelectedProfile             = result.Info.Profile.ID;
                    Client.ClientUsername       = result.Info.Profile.Name;
                    return true;

                default:
                    AccessToken                 = "None";
                    ClientToken                 = "None";
                    SelectedProfile             = "None";
                    Client.ClientUsername       = "None";
                    return false;
            }
        }

        /// <summary>
        /// Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        public async Task<bool> RefreshSession()
        {
            if (!UseLogin)
                return false;

            var result = await Yggdrasil.RefreshSession(AccessToken, ClientToken);

            switch (result.Status)
            {
                case YggdrasilResponse.Success:
                    AccessToken = result.Info.AccessToken;
                    ClientToken = result.Info.ClientToken;
                    return true;

                default:
                    return false;
            }
        }

        public async Task<bool> VerifySession()
        {
            if (!UseLogin)
                return false;

            return (await Yggdrasil.VerifySession(AccessToken)).Response;
        }

        public async Task<bool> Invalidate()
        {
            if (!UseLogin)
                return false;

            return (await Yggdrasil.Invalidate(AccessToken, ClientToken)).Response;
        }

        public override async Task<bool> Logout()
        {
            if (!UseLogin)
                return false;

            return (await Yggdrasil.Logout(Client.ClientLogin, Client.ClientPassword)).Response;
        }
    }
}
