﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Assignment_5___Jackson_vdw.Models;
using Assignment_5___Jackson_vdw.Models.ViewModels;

namespace Assignment_5___Jackson_vdw.Controllers
{
    public class HomeController : Controller
    {
        //set up private and public variables for the repository
        private readonly ILogger<HomeController> _logger;
        private IBookRepository _repository;
        public int PageSize = 5;

        public HomeController(ILogger<HomeController> logger, IBookRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        //Controller for the index page
        public IActionResult Index(string category, int pageNum = 1)
        {
            //add in information for pagination
            return View(new BookListViewModel
            {
                Books = _repository.Books
                        .Where(b => category == null || b.Category == category)
                        .OrderBy(b => b.BookId)
                        .Skip((pageNum - 1) * PageSize)
                        .Take(PageSize)
                    ,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = PageSize,
                    //the next line will create page numbers automatically using an if statement that uses a where and count to create the correct number of pages
                    TotalNumItems = category == null ? _repository.Books.Count() : _repository.Books.Where(x => x.Category == category).Count()
                },
                Category = category
            });
        }

        // not being used right not
        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
