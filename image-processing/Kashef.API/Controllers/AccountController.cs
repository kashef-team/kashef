//using System;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.ModelBinding;
//using Microsoft.Owin.Security;
//using Kashef.API.Filters;
//using Kashef.API.Results;
//using System.Net;  
//using Newtonsoft.Json.Linq;  
//using Kashef.API.Models; 

//namespace Kashef.API.Controllers
//{
//    [Authorize]
//    [RoutePrefix("api/v1/account")]
//    public class AccountController : ApiController
//    {
//        #region Properties

//        private IAccountManagmentService _accountManagmentService = null;

//        #endregion

//        #region Constructors

//        public AccountController(IAccountManagmentService accountManagementService)
//        {
//            _accountManagmentService = accountManagementService;
//        }

//        #endregion

//        #region Action Methods

//        // POST api/Account/Register
//        [AllowAnonymous]
//        [HttpPost]
//        [Route("Register")]
//        [ModelValidationFilter]
//        public IHttpActionResult Register(RegisterModel model)
//        {
//            //Precondition check...
//            if (null == model)
//            {
//                return new HttpActionResult((HttpStatusCode)422, ErrorCodes.MODEL_IS_NULL);
//            }

//            //Register New User...
//            if (_accountManagmentService.RegisterNewUser(model.FirstName, model.LastName, model.Email, model.Password, model.PhoneNumber, 1,1, model.ProfilePicture))
//            {
//                return new HttpActionResult(System.Net.HttpStatusCode.Created);
//            }
//            else
//            {
//                return BadRequest(ErrorCodes.NEWUSER_REGISTERTION_FAILURE);
//            }
//        } 
//        [HttpPost]
//        [Route("SendVerficationCode")]
//        [AllowAnonymous]
//        public IHttpActionResult SendVerficationCode(VerficationModle verficationModel)
//        { 
//            //Precondition check...
//            if (null != verficationModel && !string.IsNullOrEmpty(verficationModel.EmailAddress))
//            {
//                //Get User entity
//                User loggedInUser = _accountManagmentService.GetUserEntity(verficationModel.EmailAddress); 
                
//                //Precondition checks...
//                if (null != loggedInUser)
//                {
//                    //Generate Verification Code...
//                     loggedInUser.VerficationCode = StringExtensions.RandomString(8);
//                     loggedInUser.VerificationDate =  DateTime.Now;

//                    //Update User Entity....
//                    _accountManagmentService.UpdateUser(loggedInUser);

//                    //Send Email    
//                    Task.Factory.StartNew(() =>
//                    { 
//                        SMTPExtension.SendVerificationNumber(loggedInUser.VerficationCode, loggedInUser.FullName, loggedInUser.EmailAddress);
//                    });

//                    return Ok();
//                }
//            }
//            return BadRequest();
//        }

//        //TODO : check that verfication datetime not elpapsed 30 mins
//        [HttpPost]
//        [AllowAnonymous]
//        [Route("ResetPassword")]
//        [ModelValidationFilter]
//        public IHttpActionResult ResetPassword(ResetPasswordModel model)
//        {
//            if (null != model)
//            {
//                //Get User entity
//                User loggedInUser = _accountManagmentService.GetUserEntity(model.EmailAddress);

//                //Check that email address already exists...
//                if (null == loggedInUser)
//                {
//                    ModelState.AddModelError("EmailAddress", "The Email Address is not valid");
//                    throw new HttpResponseException(ControllerContext.Request.CreateErrorResponse((HttpStatusCode)422, ModelState));
//                }

//                ////Check that email address login method is normal login..
//                if (loggedInUser.LoginMethodId != 1)
//                {
//                    ModelState.AddModelError("EmailAddress", "The Email Address is not allowed to password reset");
//                    throw new HttpResponseException(ControllerContext.Request.CreateErrorResponse((HttpStatusCode)422, ModelState));
//                }

//                //Check Verification code
//                if (string.IsNullOrEmpty(loggedInUser.VerficationCode))
//                {
//                    ModelState.AddModelError("VerficationCode", "The verification code doesnot exist or expired");
//                    throw new HttpResponseException(ControllerContext.Request.CreateErrorResponse((HttpStatusCode)422, ModelState));
//                }

//                //Match the user verfication code with the database record
//                if (loggedInUser.VerficationCode.ToLower() != model.VerificationCode.ToLower())
//                {
//                    ModelState.AddModelError("VerficationCode", "The verification code not valid");
//                    throw new HttpResponseException(ControllerContext.Request.CreateErrorResponse((HttpStatusCode)422, ModelState));
//                }

//                //Reset Verfication code and Change the password....
//                loggedInUser.VerficationCode = null;
//                loggedInUser.VerificationDate = null;
//                loggedInUser.Hash = Hashing.CreateHashedPassword(model.NewPassword);

//                //Update User Entity....
//                _accountManagmentService.UpdateUser(loggedInUser);

//                return Ok();
//            }

//            return BadRequest();
//        }
         
//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);
//        }

//        #endregion

//        #region Private Methods

//        #endregion

//    }
//}