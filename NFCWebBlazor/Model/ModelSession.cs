using Blazored.SessionStorage;
using NFCWebBlazor.Models;

namespace NFCWebBlazor.Model
{
    public class ModelSession
    {
        public void setUserSession(Users userlogin, ISessionStorageService session)
        {

            Users users1 = new Users();
            users1.UsersName = userlogin.UsersName;
            users1.TenUser = userlogin.TenUser;
            users1.GroupUser = userlogin.GroupUser;
            users1.KhuVuc = userlogin.KhuVuc;
            users1.Email = userlogin.Email;
            users1.TypeApp = userlogin.TypeApp;
            users1.PathImg = userlogin.PathImg;
           
            session.SetItemAsync<Users>("login", users1);
        }
        public void saveMenu(string menujson, ISessionStorageService session)
        {
            session.SetItemAsync<string>("menu", menujson);
        }
        public async Task<Users> GetUserAsync(ISessionStorageService session)
        {

            Users users = await session.GetItemAsync<Users>("login");
           
            return users;
           

        }
        public async Task<string> GetMenuAsync(ISessionStorageService session)
        {

            string menujson = await session.GetItemAsync<string>("menu");

            return menujson;


        }

    }
}
