using Employee.BAL;
using Employee.DAL;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace EmployeeManagement.Controllers
{
    public class EmployeeController : Controller
    {
        EmployeeBAL empBAL = new EmployeeBAL();
        EmployeeDAL empDAL = new EmployeeDAL();

        string Request = "";
        string Response = "";
        string Exception = "";

        JsonResult returnObj = null;


        // generate web tokens
        /*
                public string GenrateJwtToken(string userName)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisismysupersecretcodeandthisisprotectedbyalogrithm"));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, userName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };
                           var token = new JwtSecurityToken(
                           claims: claims,
                           expires: DateTime.Now.AddMinutes(30),
                           signingCredentials: credentials);

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

        */

        public JsonResult Login(string username, string password)
        {

            if (username == "admin" && password == "admin")
            {
                var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new[]
                {
                     new Claim(ClaimTypes.Name, username)
                 };

                var token = new JwtSecurityToken(
                     claims: claims,
                     expires: DateTime.Now.AddDays(14),
                     signingCredentials: credentials
                );


                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Json(new { username, token = tokenString, message = "Login Successfully" , success = true });
            }

            return Json(new { username, msg = "Bad Credentials" , success = false });
        }



        // validate token 
        public ClaimsPrincipal ValidateJwtToken(string token)
        {

            var secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var clockSkew = TimeSpan.FromMinutes(15);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ClockSkew = clockSkew
                };

                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {

                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }


        public JsonResult ProtectedEndpoint(string token)
        {
            var principal = ValidateJwtToken(token);

            if (principal == null)
            {
                return Json(new { message = "Invalid Token" }, JsonRequestBehavior.AllowGet);
            }

            //            var userName = principal.Identity.Name;


            return Json(new { message = "success" });
        }


        public JsonResult ListEmployee()
        {
            // Retrieve the token from the Authorization header
            var authHeader = HttpContext.Request.Headers["Authorization"];

           
            string token = null;
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer"))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { message = "Token is missing or invalid" }, JsonRequestBehavior.AllowGet);
            }

            var principal = ValidateJwtToken(token);

            if (principal == null)
            {
                return Json(new { message = "Invalid Token" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
                string MethodName = MethodBase.GetCurrentMethod().Name;


                JsonResult returnObj;
                try
                {
                    returnObj = Json(empBAL.SelectEmployee(), JsonRequestBehavior.AllowGet);
                    returnObj.MaxJsonLength = int.MaxValue;
                    // Assuming Response is another property you're managing
                    Response = new JavaScriptSerializer().Serialize(returnObj);
                }
                catch (Exception ex)
                {
                    Exception = ex.ToString();
                    return Json(new { message = "Error occurred", error = ex.Message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { message = "success", data = returnObj });
            }
        }





        public JsonResult AddEmployee(string name, string email, string gender, int age)
        {

            var authHeader = HttpContext.Request.Headers["Authorization"];
            string token = null;
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer"))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { message = "Token is missing or invalid" }, JsonRequestBehavior.AllowGet);
            }

            var principal = ValidateJwtToken(token);

            if (principal == null)
            {
                return Json(new { message = "Invalid Token" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
                string MethodName = MethodBase.GetCurrentMethod().Name;
                string resultMessage = "";
                string msg = "";
                int resultCode = (int)ErrorCode.ErrorType.ERROR;
                try
                {
                    var result = empBAL.AddEmployee(name, email, gender, age);
                    resultMessage = result ? "Record Added Successfully" : "Failed to Add Record";
                    resultCode = result ? (int)ErrorCode.ErrorType.SUCCESS : (int)ErrorCode.ErrorType.ERROR;

                    if (result)
                    {
                        string htmlBody = $@"
                      <html>
                     <body style='font-family: Arial, sans-serif; color: #333;'>
                     <h2 style='color: #2e6c80;'>Design Accent</h2>
                     <p>Dear {name},</p>
                     <br />
                     <h4>Personal Information</h4>
                     <p><b>Name   :</b> {name}</p>
                     <p><b>Email  :</b> {email}</p>
                     <p><b>Gender :</b> {gender}</p>
                     <p><b>Age    :</b> {age}</p>
                     <br />
                     <p style='font-weight: bold;'>Best Regards,</p>
                     <p>Design Accent Team</p>
                     </body>
                     </html>";

                        MailMessage mail = new MailMessage
                        {
                            From = new MailAddress("sanjeevkrpd11@gmail.com"),
                            Subject = "Successfully Registered",
                            Body = htmlBody,  // Assign the HTML body to the MailMessage.Body
                            IsBodyHtml = true
                        };
                        mail.To.Add(email);

                        SmtpClient smtp = new SmtpClient("smtp.gmail.com")
                        {
                            Port = 587,
                            Credentials = new NetworkCredential("mailer@dnaworks.in", "qoisttmdyfyabwgf"),
                            EnableSsl = true,
                        };

                        try
                        {
                            // Send the email
                            smtp.Send(mail);
                            msg = "Email sent successfully!";
                        }
                        catch (Exception ex)
                        {
                            msg = "Failed to send email. Error: " + ex.ToString();
                        }
                    }
                    else
                    {
                        msg = "Unverified";
                    }
                }
                catch (Exception ex)
                {
                    resultMessage = ex.ToString();
                    Exception = ex.ToString();
                }
                finally
                {
                    Response = new JavaScriptSerializer().Serialize(resultMessage);
                }
                return Json(new { Message = resultMessage, Code = resultCode, Info = msg , success = true });

            }




        }


        public JsonResult DeleteEmployee(int id)
        {
            var authHeader = HttpContext.Request.Headers["Authorization"];
            string token = null;
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer"))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { message = "Token is missing or invalid" }, JsonRequestBehavior.AllowGet);
            }

            var principal = ValidateJwtToken(token);

            if (principal == null)
            {
                return Json(new { message = "Invalid Token" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
                string MethodName = MethodBase.GetCurrentMethod().Name;
                string resultMessage = "";

                int resultCode = (int)ErrorCode.ErrorType.ERROR;

                try
                {
                    var result = empBAL.DeleteEmployee(id);
                    resultMessage = result ? "Record deleted successfully" : "Failed to delete record";
                    resultCode = result ? (int)ErrorCode.ErrorType.SUCCESS : (int)ErrorCode.ErrorType.ERROR;

                }
                catch (Exception ex)
                {
                    resultMessage = ex.ToString();
                    Exception = ex.ToString();
                }
                finally
                {
                    Response = new JavaScriptSerializer().Serialize(resultMessage);
                }
                return Json(new { Message = resultMessage, Code = resultCode, success = true });
            }
        }

        public JsonResult UpdateEmployee(int id, string name, string email, string gender, int age)
        {
            string ClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string MethodName = MethodBase.GetCurrentMethod().Name;
            string resultMessage = "";
            string msg = "";
            int resultCode = (int)ErrorCode.ErrorType.ERROR;
            try
            {
                var result = empBAL.UpdateEmployee(id, name, email, gender, age);
                resultMessage = result ? "Record Updated Successfully" : "Failed to Update Record";
                resultCode = result ? (int)ErrorCode.ErrorType.SUCCESS : (int)ErrorCode.ErrorType.ERROR;

                if (result)
                {
                    string htmlBody = $@"
                      <html>
                     <body style='font-family: Arial, sans-serif; color: #333;'>
                     <h2 style='color: #2e6c80;'>Learning How to Send Email from SMTP</h2>
                     <p>Dear {name},</p>
                     <br />
                     <h4>Updated Personal Information </h4>
                     <p><b>Name   :</b> {name}</p>
                     <p><b>Email  :</b> {email}</p>
                     <p><b>Gender :</b> {gender}</p>
                     <p><b>Age    :</b> {age}</p>
                     <br />
                     <p style='font-weight: bold;'>Best Regards,</p>
                     <p>Design Accent Team</p>
                     </body>
                     </html>";

                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress("sanjeevkrpd11@gmail.com"),
                        Subject = "Successfully Updated ",
                        Body = htmlBody,  // Assign the HTML body to the MailMessage.Body
                        IsBodyHtml = true
                    };
                    mail.To.Add(email);

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("mailer@dnaworks.in", "qoisttmdyfyabwgf"),
                        EnableSsl = true,
                    };

                    try
                    {
                        // Send the email
                        smtp.Send(mail);
                        msg = "Email sent successfully!";
                    }
                    catch (Exception ex)
                    {
                        msg = "Failed to send email. Error: " + ex.ToString();
                    }
                }
                else
                {
                    msg = "Unverified";
                }
            }
            catch (Exception ex)
            {
                resultMessage = ex.ToString();
                Exception = ex.ToString();
            }
            finally
            {
                Response = new JavaScriptSerializer().Serialize(resultMessage);
            }
            return Json(new { Message = resultMessage, Code = resultCode, Info = msg });
        }
    }
}
