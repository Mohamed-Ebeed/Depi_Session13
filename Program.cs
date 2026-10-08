using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ASSLINQ.ListGenerators;

namespace ASSLINQ
{
    // Custom comparer for Grouping Q3: words made of the same characters are "equal"
    class AnagramComparer : IEqualityComparer<string>
    {
        private static string Normalize(string s) => new string(s.Trim().OrderBy(c => c).ToArray());
        public bool Equals(string x, string y) => Normalize(x) == Normalize(y);
        public int GetHashCode(string obj) => Normalize(obj).GetHashCode();
    }

    internal class Program
    {
        static void Title(string t) => Console.WriteLine($"\n===== {t} =====");

        static void Main(string[] args)
        {
            int[] Arr = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
            string[] words = File.ReadAllLines("dictionary_english.txt");

            #region LINQ - Element Operators
            Title("Element 1: first product out of stock");
            var outOfStock = ProductList.FirstOrDefault(p => p.UnitsInStock == 0);
            Console.WriteLine(outOfStock);

            Title("Element 2: first product with price > 1000 (or null)");
            var expensive = ProductList.FirstOrDefault(p => p.UnitPrice > 1000);
            Console.WriteLine(expensive?.ToString() ?? "null");

            Title("Element 3: second number greater than 5");
            var second = Arr.Where(n => n > 5).Skip(1).First();   // or .ElementAt(1)
            Console.WriteLine(second);
            #endregion

            #region LINQ - Aggregate Operators
            Title("Aggregate 1: count of odd numbers");
            Console.WriteLine(Arr.Count(n => n % 2 != 0));

            Title("Aggregate 2: customers and their orders count");
            var custOrders = from c in CustomerList
                             select new { c.CustomerName, OrdersCount = c.Orders.Count() };
            foreach (var c in custOrders) Console.WriteLine(c);

            Title("Aggregate 3: categories and products count");
            var catCount = from p in ProductList
                           group p by p.Category into g
                           select new { Category = g.Key, ProductsCount = g.Count() };
            foreach (var c in catCount) Console.WriteLine(c);

            Title("Aggregate 4: total of numbers");
            Console.WriteLine(Arr.Sum());

            Title("Aggregate 5: total characters of all words");
            Console.WriteLine(words.Sum(w => (long)w.Length));

            Title("Aggregate 6: shortest word length");
            Console.WriteLine(words.Min(w => w.Length));

            Title("Aggregate 7: longest word length");
            Console.WriteLine(words.Max(w => w.Length));

            Title("Aggregate 8: average word length");
            Console.WriteLine(words.Average(w => w.Length));

            Title("Aggregate 9: total units in stock per category");
            var unitsPerCat = from p in ProductList
                              group p by p.Category into g
                              select new { Category = g.Key, TotalUnits = g.Sum(p => p.UnitsInStock) };
            foreach (var c in unitsPerCat) Console.WriteLine(c);

            Title("Aggregate 10: cheapest price per category");
            var cheapest = from p in ProductList
                           group p by p.Category into g
                           select new { Category = g.Key, CheapestPrice = g.Min(p => p.UnitPrice) };
            foreach (var c in cheapest) Console.WriteLine(c);

            Title("Aggregate 11: products with lowest price per category (LET)");
            var lowest = from p in ProductList
                         group p by p.Category into g
                         let minPrice = g.Min(p => p.UnitPrice)
                         select new { Category = g.Key, Products = g.Where(p => p.UnitPrice == minPrice) };
            foreach (var g in lowest)
            {
                Console.WriteLine(g.Category);
                foreach (var p in g.Products) Console.WriteLine("   " + p);
            }

            Title("Aggregate 12: highest price per category");
            var highest = from p in ProductList
                          group p by p.Category into g
                          select new { Category = g.Key, HighestPrice = g.Max(p => p.UnitPrice) };
            foreach (var c in highest) Console.WriteLine(c);

            Title("Aggregate 13: products with highest price per category");
            var highestProducts = from p in ProductList
                                  group p by p.Category into g
                                  let maxPrice = g.Max(p => p.UnitPrice)
                                  select new { Category = g.Key, Products = g.Where(p => p.UnitPrice == maxPrice) };
            foreach (var g in highestProducts)
            {
                Console.WriteLine(g.Category);
                foreach (var p in g.Products) Console.WriteLine("   " + p);
            }

            Title("Aggregate 14: average price per category");
            var avgPrice = from p in ProductList
                           group p by p.Category into g
                           select new { Category = g.Key, AveragePrice = g.Average(p => p.UnitPrice) };
            foreach (var c in avgPrice) Console.WriteLine(c);
            #endregion

            #region LINQ - Set Operators
            Title("Set 1: unique category names");
            foreach (var c in ProductList.Select(p => p.Category).Distinct()) Console.WriteLine(c);

            var productLetters = ProductList.Select(p => p.ProductName[0]);
            var customerLetters = CustomerList.Select(c => c.CustomerName[0]);

            Title("Set 2: unique first letters from product and customer names");
            Console.WriteLine(string.Join(", ", productLetters.Union(customerLetters)));

            Title("Set 3: common first letters");
            Console.WriteLine(string.Join(", ", productLetters.Intersect(customerLetters)));

            Title("Set 4: product first letters that are not customer first letters");
            Console.WriteLine(string.Join(", ", productLetters.Except(customerLetters)));

            Title("Set 5: last three characters of all customer & product names (with duplicates)");
            var last3 = CustomerList.Select(c => c.CustomerName)
                        .Concat(ProductList.Select(p => p.ProductName))
                        .Select(n => n.Length >= 3 ? n.Substring(n.Length - 3) : n);
            foreach (var s in last3) Console.WriteLine(s);
            #endregion

            #region LINQ - Partitioning Operators
            var waOrders = CustomerList.Where(c => c.Region == "WA").SelectMany(c => c.Orders);

            Title("Partitioning 1: first 3 orders from Washington customers");
            foreach (var o in waOrders.Take(3)) Console.WriteLine(o);

            Title("Partitioning 2: all but first 2 orders from Washington customers");
            foreach (var o in waOrders.Skip(2)) Console.WriteLine(o);

            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            Title("Partitioning 3: from start until a number < its position");
            Console.WriteLine(string.Join(", ", numbers.TakeWhile((n, i) => n >= i)));

            Title("Partitioning 4: starting from first element divisible by 3");
            Console.WriteLine(string.Join(", ", numbers.SkipWhile(n => n % 3 != 0)));

            Title("Partitioning 5: starting from first element less than its position");
            Console.WriteLine(string.Join(", ", numbers.SkipWhile((n, i) => n >= i)));
            #endregion

            #region LINQ - Quantifiers
            Title("Quantifier 1: any word contains 'ei'");
            Console.WriteLine(words.Any(w => w.Contains("ei")));

            Title("Quantifier 2: categories with at least one out-of-stock product");
            var someOut = ProductList.GroupBy(p => p.Category)
                                     .Where(g => g.Any(p => p.UnitsInStock == 0));
            foreach (var g in someOut)
            {
                Console.WriteLine(g.Key);
                foreach (var p in g) Console.WriteLine("   " + p);
            }

            Title("Quantifier 3: categories with all products in stock");
            var allIn = ProductList.GroupBy(p => p.Category)
                                   .Where(g => g.All(p => p.UnitsInStock > 0));
            foreach (var g in allIn)
            {
                Console.WriteLine(g.Key);
                foreach (var p in g) Console.WriteLine("   " + p);
            }
            #endregion

            #region LINQ - Grouping Operators
            Title("Grouping 1: numbers by remainder when divided by 5");
            List<int> nums = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            foreach (var g in nums.GroupBy(n => n % 5).OrderBy(g => g.Key))
            {
                Console.WriteLine($"Numbers with a remainder of {g.Key} when divided by 5:");
                foreach (var n in g) Console.WriteLine(n);
            }

            Title("Grouping 2: words by first letter (dictionary) - showing count + first 5 words");
            var byLetter = words.Where(w => w.Length > 0)
                                .GroupBy(w => char.ToUpper(w[0]))
                                .OrderBy(g => g.Key);
            foreach (var g in byLetter)
                Console.WriteLine($"{g.Key}: {g.Count()} words, e.g. {string.Join(", ", g.Take(5))}");

            Title("Grouping 3: anagram groups with custom comparer");
            string[] arr = { "from", "salt", "earn", " last", "near", "form" };
            foreach (var g in arr.GroupBy(w => w, new AnagramComparer()))
            {
                Console.WriteLine("...");
                foreach (var w in g) Console.WriteLine(w);
            }
            #endregion
        }
    }
}
