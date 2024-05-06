using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cafemanagement.Models;  





namespace Cafemanagement.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        SQLTrainingEntities1 db = new SQLTrainingEntities1 ();


        [HttpPost,Route("signup")]
        public HttpResponseMessage Signup([FromBody] UserTable user )
        {
            try /// it will get the data from the body and then find the email that is match with the email 
                // of the database
            {
                UserTable userobj = db.UserTables
                .Where(u => u.email == user.email).FirstOrDefault();
                if(userobj == null)
                {
                    user.role = "user";
                    user.status = "true";  
                    db.UserTables.Add(user);  // here we adding the user.role  and user.status in the Body(json body that is given by the user)
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Signup sucessfully " });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest , new {message = "Email is Already Exist"});
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        } 

        [HttpPost, Route("login")] 
        public HttpResponseMessage Login([FromBody] UserTable user)
        {  
            try
            {
                UserTable userobj = db.UserTables
                .Where(u => (u.email == user.email && u.password == user.password)).FirstOrDefault(); 
                 if(userobj != null)
                {
                    if (userobj.status == "true")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new {token =TokenManager.GenerateToke(userobj.email, userobj.role)});
                    } 
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Waiting for the admin approval " });
                    } 

                } 
                 else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Invalid Login Login Credential" });
                }
            }
            catch (Exception ex) {

              return  Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

        }




    }
}
