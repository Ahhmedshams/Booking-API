using PayPal.Api;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utility.Extensions
{
    public static class BookingItemExtension
    {
        public static List<SessionLineItemOptions> ToSessionLineItemOptionsObject(this List<BookingItem> bookingItems)
        {

            List<SessionLineItemOptions> sessionLineItemOptions = new List<SessionLineItemOptions>();

            foreach (var bookingItem in  bookingItems)
            {
                var lineItem = new SessionLineItemOptions() { Quantity = 1, PriceData = new SessionLineItemPriceDataOptions() { Currency = "egp",
                UnitAmount = (int) (bookingItem.Price * 100), ProductData = new() { Name = bookingItem?.Resource?.Name ?? "any thing name"}}
               };

                sessionLineItemOptions.Add(lineItem);

            }

            return sessionLineItemOptions;
        }

        public static ItemList ToPaypalItemsList(this List<BookingItem> bookingItems)
        {
            List<Item> items = new List<Item>();

            foreach(var bookingItem in bookingItems)
            {
                var item = new Item() { currency = "USD", name = bookingItem.Resource.Name, price = bookingItem.Price.ToString(), quantity = "1"};

                items.Add(item);
            }

            return new ItemList() { items = items};
        }
    }
}
