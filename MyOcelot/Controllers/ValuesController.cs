using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyOcelot.Authentication.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private TokenBuilder _tokenBuilder = null;
        public ValuesController(TokenBuilder tokenBuilder)
        {
            this._tokenBuilder = tokenBuilder; ;
        }
        /// <summary>
        /// 登录，生成加密token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Login()
        {
            //省略登录逻辑。。。。
            var user = new User(1, "fan", "410577910@qq.com", "admin");
            string signToken = this._tokenBuilder.CreateJwtToken(user);
            return signToken;
        }
        /// <summary>
        /// 创建订单，需要校验token
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult<string> CreateOrder()
        {
            if (base.User.Identity.IsAuthenticated)
            {
                string userName = base.User.Identity.Name;
            }
            //省略创建订单逻辑。。。
            return "生成订单成功";
        }
        /// <summary>
        /// 创建订单，需要校验token
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "system")]
        [HttpPost]
        public ActionResult<string> CreateUser()
        {
            if (base.User.Identity.IsAuthenticated)
            {
                string userName = base.User.Identity.Name;
            }
            //省略创建订单逻辑。。。
            return "CreateUser成功";
        }
    }
}
