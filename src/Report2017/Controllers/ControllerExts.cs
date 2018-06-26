﻿using Microsoft.AspNetCore.Mvc;
using Report2017.Helpers;

namespace Report2017.Controllers
{
    public static class ControllerExts
    {
        public static TheUser GetUser(this Controller ctx){
            return ctx.User.Identity.GetUser();
        }

        public static string GetUserEmail(this Controller ctx){
            return  ctx.User.Identity.GetUserEmail();
        }
    }
}
