using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dookki_Web.Models.Map
{
    public class mapAccount
    {
        DOOKKIEntities db = new DOOKKIEntities();
        public ACCOUNT find(string username, string password)
        {
            var user = db.ACCOUNTs.Where(m=>m.UserName == username & m.Password == password).ToList();
            if(user.Count > 0)
            {
                return user[0];
            }
            else
            {
                return null;
            }
        }
    }
}