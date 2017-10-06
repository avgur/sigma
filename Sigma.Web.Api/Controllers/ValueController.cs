namespace Sigma.Web.Api.Controllers
{
    using Sigma.Web.Frontend.Core.Filters;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Web.Http;


    [RoutePrefix("api/value")]
    public class ValueController : ApiController
    {
        
        public class Divide
        {
            public int a { get; set; }
            
            [Range(1, int.MaxValue)]
            public int b { get; set; }
        }


        [AllowAnonymous]
        [Route("login")]
        [HttpGet]
        public object Login(string role = "divider")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "user"),
                new Claim(ClaimTypes.Email, "user@gmail.com"),
                new Claim(ClaimTypes.Role, role)
            };
            var id = new ClaimsIdentity(claims, Registry.ApplicationCookie);

            var ctx = this.Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(id);

            return new
            {
                Success = true
            };
        }

        [ApiAuthorize]
        [Route("logout")]
        [HttpGet]
        public object Logout()
        {
            var ctx = this.Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            return new
            {
                Success = true
            };
        }

        [ApiAuthorize(Roles = "divider")]
        [Route("div")]
        [HttpGet]
        public int Div([FromUri][Required(ErrorMessage = "The model is required.")]Divide divide)
        {
            return divide.a / divide.b;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" }; 
        }

        [Route("{id}")]
        [HttpGet]
        public string Get(int id)
        {
            throw new Exception("aaa");
            return "value";
        }

        [Route("")]
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [Route("{id}")]
        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
        }

        [Route("{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}
