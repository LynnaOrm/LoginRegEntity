﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using LoginRegEntity.Models;
using Microsoft.AspNetCore.Identity;

namespace LoginRegEntity.Controllers
{
    public class HomeController : Controller
    {
        private LogRegContext _context;

        public HomeController(LogRegContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterUser newUser)
        {
            System.Console.WriteLine("IM HERE!");

            if (_context.users.Where(u => u.Email == newUser.Email).SingleOrDefault() != null)
                ModelState.AddModelError("Email", "Email already in use");

            if (ModelState.IsValid)
            {
                PasswordHasher<RegisterUser> hasher = new PasswordHasher<RegisterUser>();
                // insert user into DB
                User User = new User
                {
                    FirstName = newUser.FirstName,
                    LastName = newUser.LastName,
                    Email = newUser.Email,
                    Password = hasher.HashPassword(newUser, newUser.Password),
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                User theUser = _context.Add(User).Entity;
                _context.SaveChanges();
                
                System.Console.WriteLine("weeeeeeeeeeeee");

                HttpContext.Session.SetInt32("id", theUser.Id);
                return RedirectToAction("Success");
            }
            return View("Index");
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(LoginUser logUser)
        {
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            
            User userToLog = _context.users.Where(u => u.Email == logUser.LogEmail).SingleOrDefault();
            // User userToLog = _context.users.SingleOrDefault(u => u.Email == logUser.LogEmail);
            if(userToLog == null)
                ModelState.AddModelError("LogEmail", "Cannot find Email");
            else if( hasher.VerifyHashedPassword(logUser, userToLog.Password, logUser.LogPassword) == 0)
            {
                ModelState.AddModelError("LogPassword", "Wrong Password");
            }
            if(!ModelState.IsValid)
                return View("Index");
            HttpContext.Session.SetInt32("id", userToLog.Id);
            return RedirectToAction("Success");
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        private User ActiveUser
        {
            get{ return _context.users.Where(u => u.Id == HttpContext.Session.GetInt32("id")).FirstOrDefault();}
        }

        [HttpGet]
        [Route("Success")]
        public IActionResult Success()
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            User user = _context.users.Where(u => u.Id == HttpContext.Session.GetInt32("id")).FirstOrDefault();
            ViewBag.UserInfo = user;
            return View();
        }
    }
}