using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assignment_5___Jackson_vdw.Infrastructure;
using Assignment_5___Jackson_vdw.Models;
//######################################################################################

namespace Assignment_5___Jackson_vdw.Pages
{
    //this model will bring in the repository
    public class PurchaseModel : PageModel
    {
        private IBookRepository repository;

        //constructor
        public PurchaseModel(IBookRepository repo)
        {
            repository = repo;
        }

        //properties
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }

        //methods
        //on a get - set the returnURl equal to whatever was passed in
        public void OnGet(string returnUrl)
        {
            //if nothing was passed in, set it equal to "/"
            ReturnUrl = returnUrl ?? "/";
            //if nothing was in the cart, get a new cart
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        }

        //
        public IActionResult OnPost(long bookId, string returnUrl)
        {
            //look at first or default
            Book book = repository.Books.FirstOrDefault(b => b.BookId == bookId);

            //get the cart or add a new cart
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();

            //add an item to the cart of qty 1
            Cart.AddItem(book, 1);

            //convert the cart into Json
            HttpContext.Session.SetJson("cart", Cart);

            //send to a new page using the returnUrl
            return RedirectToPage(new { returnUrl = returnUrl });
        }
    }
}