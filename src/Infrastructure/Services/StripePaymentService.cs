﻿using Application.Common.Interfaces.Services;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Infrastructure.Persistence.Specification.BookingItemSpec;
using Infrastructure.Persistence.Repositories;

using AutoMapper;
using Infrastructure.Utility.Extensions;
using Stripe;
using Application.Common.Interfaces.Repositories;
using Domain.Enums;
using Infrastructure.Factories;

namespace Infrastructure.Services
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IConfiguration configuration;

        public StripePaymentService(IConfiguration configuration)
        {
            this.configuration=configuration;
        }

        //public Task<bool> CancelPayment(string paymentID)
        //{
        //    return Task.FromResult(false);

        //}

        public Task<bool> RefundPayment(string paymentID)
        {

            var paymentIntentService = new PaymentIntentService();

            var payment = paymentIntentService.Get(paymentID);


            var options = new RefundCreateOptions
            {
                Charge = payment.LatestChargeId,
            };
            var service = new RefundService();

            //TODO: handle exception for charge not found
            // TODO: handle Stripe.StripeException: Charge ch_3NL2cjJSb1KZM6uY1LEycZmT has already been refunded.

            var refund =  service.Create(options);



            if (refund.Status == "succeeded")
                return Task.FromResult(true);

            return Task.FromResult(false);

        }

        public async Task<string> MakePayment(IPayemntTransactionRepository paymentTransactionRepository, IBookingItemRepo bookingItemRepo, decimal amount, int bookingID)
        {
         
            // TODO: read all showable items, sum all material in one item, sum all items like cars in one item.
            // TODO: Case when booking items counts is 0


            var bookingItems = bookingItemRepo.GetAllBooksItemsByBookingId(bookingID);


            ClientBooking clientBooking = null;
            
            if (bookingItems.Count <= 0) {
                // TODO: add payment exception
                throw new InvalidOperationException("no items in the bookings");
            }

            clientBooking = bookingItems[0].ClientBooking;

            if (clientBooking.Status == BookingStatus.Confirmed)
                throw new InvalidOperationException("This booking already paid and confirmed.");

            var sessionLineItemOptions = bookingItems.ToSessionLineItemOptionsObject();

            var metadata = new Dictionary<string, string>();
            metadata.Add("bookingID", bookingID.ToString());

            string clientReferenceId = bookingItems[0].ClientBooking.UserId;
            //DateTime futureTime = DateTime.UtcNow.AddMinutes(5); // Add 5 minutes to the current UTC time
            //long unixTimestamp = ((DateTimeOffset)futureTime).ToUnixTimeSeconds();

            var options = new SessionCreateOptions
            {
                SuccessUrl = configuration["Stripe:SuccessUrl"],
                CancelUrl = configuration["Stripe:SuccessUrl"],
                LineItems = sessionLineItemOptions,
                Mode = "payment",
                ExpiresAt =  DateTime.Now.AddMinutes(30),
                ClientReferenceId = clientReferenceId,
               // PaymentIntentData = new() { ReceiptEmail = bookingItems[0].ClientBooking.User.Email },

                // TODO: complete data for invoice
               // InvoiceCreation = new() { Enabled = true, InvoiceData = new() { Footer = "Swift Reserve Invoice"} },
                Metadata = metadata,

            };

            var service = new SessionService();
            
            var session = service.Create(options);


            var payementTransactions = await paymentTransactionRepository.FindAsync(e => e.ClientBookingId == bookingID);



            var payementTransaction = payementTransactions.FirstOrDefault();

            if (payementTransaction == null)
            {
                 payementTransaction = new PaymentTransaction()
                {
                    ClientBookingId = bookingID,
                    Amount = clientBooking.TotalCost,
                    UserId = clientBooking.UserId,
                    PaymentMethodId = (int)PaymentMethodType.Card,
                    Status = PaymentStatus.Pending,
                    SessionId = session.Id
                };

                await paymentTransactionRepository.AddAsync(payementTransaction);
            }
            else
            {
                // TODO: expire old stripe session
                payementTransaction.SessionId = session.Id;
            }

            

            return await Task.FromResult(session.Url);
        }
    }
}
