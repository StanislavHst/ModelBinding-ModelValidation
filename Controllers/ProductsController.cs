using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsValidation.Models;
using ProductsValidation.Services;

namespace ProductsValidation.Controllers
{
    
    public class ProductsController : Controller
    {
        private List<Product> myProducts;

        public ProductsController(Data data)
        {
            myProducts = data.Products;
        }
        
        public IActionResult Index(int filterId, string filtername)
        {
            return View(myProducts);
        }
        
        public IActionResult View(int id)
        {
            Product prod = myProducts.Find(prod => prod.Id == id);
            if (prod != null)
            {
                return View("View", prod);
            }

            return View("NotExists");
        }
        public IActionResult Details(int id)
        {
            return View(id);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product prod = myProducts.Find(prod => prod.Id == id);
            if (prod != null)
            {
                return View(prod);
            }

            return View("NotExists");
        } 
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            CheckDescription(product);
            if (!ModelState.IsValid) return View(product);

            int index = myProducts.FindIndex(prod => prod.Id == product.Id);
            if (index == -1) return View("NotExists");

            myProducts[index] = product;
            return View("View", product);
        }

        
        [HttpPost]
        public IActionResult Create(Product product)
        {
            CheckDescription(product);
            if (!ModelState.IsValid) return View(product);

            myProducts.Add(product);
            return View("View", product);
        }

        public IActionResult Create()
        {
            return View(new Product(){Id = myProducts.Select(product => product.Id).DefaultIfEmpty().Max() + 1});
        }

        public IActionResult Delete(int id)
        {
            myProducts.Remove( myProducts.Find(product => product.Id == id));
            return View("Index", myProducts);
        }

        [HttpGet]
        public IActionResult EditPrices(Product.Category? type)
        {
            if (type == null) return BadRequest();

            ViewBag.Type = type;
            return View(GetPrices(myProducts.FindAll(product => product.Type == type)));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult EditPrices([FromQuery] Product.Category? type, List<PriceEdit> rows)
        {
            if (type == null) return BadRequest();

            ViewBag.Type = type;
            List<Product> products = myProducts.FindAll(product => product.Type == type);

            if (!rows.Select(row => row.Id).SequenceEqual(products.Select(product => product.Id)))
                ModelState.AddModelError("", "Open the price list again.");

            if (!ModelState.IsValid) return View(GetPrices(products));

            for (int i = 0; i < products.Count; i++)
            {
                products[i].Price = rows[i].Price.Value;
            }
            return RedirectToAction("Index");
        }

        private List<PriceEdit> GetPrices(List<Product> products)
        {
            var prices = new List<PriceEdit>();
            foreach (Product product in products)
            {
                prices.Add(new PriceEdit { Id = product.Id, Name = product.Name, Price = product.Price });
            }
            return prices;
        }

        private void CheckDescription(Product product)
        {
            if (string.IsNullOrEmpty(product.Description) || string.IsNullOrEmpty(product.Name)) return;

            if (product.Description == product.Name ||
                !product.Description.StartsWith(product.Name, StringComparison.Ordinal))
            {
                ModelState.AddModelError("Description",
                    "Description must start with the product name and contain additional text.");
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
