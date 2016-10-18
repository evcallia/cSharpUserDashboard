using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using userDashboard.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using CryptoHelper;

namespace userDashboard.Controllers
{
    public class HomeController : Controller
    {
        private UserDashboardContext _context;

        public HomeController(UserDashboardContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("welcome");
        }

        [HttpGet]
        [Route("welcome")]
        public IActionResult Welcome()
        {
            return View("welcome");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            if(HttpContext.Session.GetInt32("id") != null){
                return RedirectToAction("Dashboard");
            }
            return View("login", new User());
        }

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            if(HttpContext.Session.GetInt32("id") != null){
                return RedirectToAction("Dashboard");
            }
            return View("register", new UserReg());
        }

        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("id") == null){
                return RedirectToAction("Login");
            }
            User current = _context.users.Single(u => u.id == HttpContext.Session.GetInt32("id"));
            ViewBag.user_level = current.user_level;
            ViewBag.id = HttpContext.Session.GetInt32("id");
            return View("dashboard", _context.users.ToList());
        }

        [HttpGet]
        [Route("add-user")]
        public IActionResult AddUser()
        {
            if(HttpContext.Session.GetInt32("id") == null){
                return RedirectToAction("Login");
            }
            User current = _context.users.Single(u => u.id == HttpContext.Session.GetInt32("id"));
            if(current.user_level != "admin"){
                return RedirectToAction("Dashboard");
            }
            return View("addUser", new UserReg());
        }

        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            if(HttpContext.Session.GetInt32("id") == null){
                return RedirectToAction("Login");
            }
            if(id == HttpContext.Session.GetInt32("id")){
                ViewBag.self = true;
            }else{
                ViewBag.self = false;
            }
            User user = _context.users.SingleOrDefault(u => u.id == id);
            if(user.user_level != "admin" && user.id != id){
                return RedirectToAction("Dashboard");
            }
            UserReg userReg = new UserReg().Transfer(user);
            User current = _context.users.SingleOrDefault(u => u.id == HttpContext.Session.GetInt32("id"));
            if(current.user_level == "admin"){
                ViewBag.admin = true;
            }else{
                ViewBag.admin = false;
            }
            return View("edit", userReg);
        }

        [HttpGet]
        [Route("show-user/{id}")]
        public IActionResult ShowUser(int id)
        {
            if(HttpContext.Session.GetInt32("id") == null){
                return RedirectToAction("Login");
            }
            var messages = (from message in _context.messages
                            where message.userId == id
                            select message).ToList();
            foreach(var message in messages){
            var comments = (from comment in _context.comments
                            where comment.messageId == message.id
                            select comment).ToList();
            if(comments == null){
                comments = new List<Comment>();
            }
            message.comments = comments;
            }
            User user = _context.users.SingleOrDefault(u => u.id == id);
            user.messagesTo = messages;
            ViewBag.posterId = HttpContext.Session.GetInt32("id");
            return View("showUser", user);
        }

        [HttpGet]
        [Route("remove/{id}")]
        public IActionResult DeleteUser(int id){
            if(HttpContext.Session.GetInt32("id") == null){
                return RedirectToAction("Login");
            }
            User current = _context.users.Single(u => u.id == HttpContext.Session.GetInt32("id"));
            if(current.user_level != "admin"){
                return RedirectToAction("Dashboard");
            }
            User userToRemove = _context.users.SingleOrDefault(u => u.id == id);
            _context.Remove(userToRemove);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [Route("register")]
        public IActionResult ProcessRegister(UserReg newUser)
        {   
            if(ModelState.IsValid){
                if(_context.users.ToList().Count > 0 && _context.users.SingleOrDefault(u => u.email == newUser.email) != null){
                    ModelState.AddModelError("email", "Email is already in use.");
                }else{
                    User user = new User().Transfer(newUser);
                    if(_context.users.ToList().Count == 0){
                        user.user_level = "admin";
                    }
                    user.password = Crypto.HashPassword(user.password);
                    _context.users.Add(user);
                    _context.SaveChanges();
                    if(HttpContext.Session.GetInt32("id") == null){
                        user = _context.users.Single(u => u.email == newUser.email);
                        HttpContext.Session.SetInt32("id", user.id);
                    }
                    return RedirectToAction("Dashboard");
                }
            }
            if(HttpContext.Session.GetInt32("id") != null){
                return View("addUser", newUser);
            }
            return View("register", newUser);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult ProcessLogin(User newUser)
        {   
            if(_context.users.ToList().Count > 0){
                User existing_user = _context.users.Single(u => u.email == newUser.email);
                if(existing_user != null){
                    if(Crypto.VerifyHashedPassword(existing_user.password, newUser.password)){
                        HttpContext.Session.SetInt32("id", existing_user.id);
                        return RedirectToAction("Dashboard");
                    }
                }
            }else{
                ModelState.AddModelError("email", "Email does not exist.");
            }
            return View("login", newUser);
        }

        [HttpPost]
        [Route("edit/{id}")]
        public IActionResult Update(UserReg updatedUser, string type, int id, string user_level)
        {
            if(id == HttpContext.Session.GetInt32("id")){
                ViewBag.self = true;
            }else{
                ViewBag.self = false;
            }
            if(ModelState.IsValid){
                User user = _context.users.SingleOrDefault(u => u.id == updatedUser.id);
                if(type == "password"){
                    user.password = CryptoHelper.Crypto.HashPassword(updatedUser.password);
                }else if(type == "info"){
                    User emailTest = _context.users.SingleOrDefault(u => u.email == updatedUser.email);
                    if (emailTest == null || emailTest.id == user.id){
                        user = user.Transfer(updatedUser);
                        user.description = updatedUser.description;
                        if(user_level != null){
                            user.user_level = user_level;
                        }
                    }else{
                        ModelState.AddModelError("email", "Email is already in use");
                        return View("edit", updatedUser);
                    }
                }else{
                    user.description = updatedUser.description;
                }
                _context.SaveChanges();                
                return RedirectToAction("Dashboard");
            }
            return View("edit", updatedUser);
        }

        [HttpPost]
        [Route("message")]
        public IActionResult Message(int posterId, string message, int userId)
        {
            //VALIDATIONS ON MESSAGE FIELDS
            Message newMessage = new Message{
                message = message,
                posterId = posterId,
                userId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.messages.Add(newMessage);
            _context.SaveChanges();
            return RedirectToAction("ShowUser", new { id = userId});
        }

        [HttpPost]
        [Route("comment")]
        public IActionResult Comment(int messageId, string comment, int commenterId, int userId)
        {
            //VALIDATIONS ON MESSAGE FIELDS
            Comment newComment = new Comment{
                comment = comment,
                messageId = messageId,
                commenterId = commenterId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _context.comments.Add(newComment);
            _context.SaveChanges();
            return RedirectToAction("ShowUser", new{ id = userId});
        }

        [HttpGet]
        [Route("log-off")]
        public IActionResult LogOff(){
            HttpContext.Session.Clear();
            return RedirectToAction("Welcome");
        }
    }
}
