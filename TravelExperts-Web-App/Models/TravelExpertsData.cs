using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TravelExperts_Web_App.Models
{
    /// <summary>
    /// operations on Travel experts database
    /// </summary>
    
    public static class TravelExpertsData
    {
        /// <summary>
        /// See if a customer exists in Customer table of Travel Experts Database
        ///     check against business phone number (assume unique)
        /// </summary>
        /// <param name="customer">Customer object to check</param>
        /// <returns>True if customer exists, false otherwise</returns>
        public static bool CustomerExists(Customer customer)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // find customer by phone number
                var result = db.Customers.SingleOrDefault(cust => cust.CustBusPhone == customer.CustBusPhone);

                return result != null;
            }
        }

        /// <summary>
        /// See if a customer that's in the database, a login account
        ///     check against phone number in Customer table AND email in AspNetUsers table
        /// </summary>
        /// <param name="customer">Customer object to check</param>
        /// <returns>True if account exists, false otherwise</returns>
        public static bool AccountExists(Customer customer)
        {
            if(CustomerExists(customer)) // customer exists in Customer table - if customer is not in customer table, they can't be in accounts table
            {
                using (AccountEntities accntDB = new AccountEntities())
                {
                    // find account by email
                    var accntResult = accntDB.AspNetUsers.SingleOrDefault(accnt => accnt.Email == customer.CustEmail);

                    return accntResult != null; // customer exists in customer table and has a login account in AspNetUsers table
                }
            }
            return false;
        }

        /// <summary>
        /// Insert a customer into the Customer table of Travel Experts database
        /// </summary>
        /// <param name="customer"></param>
        public static void InsertCustomer(Customer customer)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                db.Customers.Add(customer);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Update a customer's user name in AspNetUsers table AND Customers table
        /// </summary>
        /// <param name="newCustomer">Customer object to update in database</param>
        /// <returns>True on success, false otherwise</returns>
        public static bool UpdateAccountUserName(Customer newCustomer)
        {
            // update accounts table
            using (AccountEntities db = new AccountEntities())
            {
                // get account from AspNetUsers table by email
                var account = db.AspNetUsers.SingleOrDefault(accnt => accnt.Email == newCustomer.CustEmail);
                if (account != null) // found account
                {
                    account.UserName = newCustomer.UserName;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Update a customer's email in Customers table
        /// </summary>
        /// <param name="customer">Customer to update</param>
        /// <returns>True on success, false otherwise</returns>
        public static bool UpdateCustomerUserName (Customer newCustomer) 
        {
            // update customer table
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer from Customer table by phone number
                var customer = db.Customers.SingleOrDefault(cust => cust.CustBusPhone == newCustomer.CustBusPhone);
                if(customer != null) // found customer
                {
                    customer.UserName = newCustomer.UserName;
                    db.SaveChanges();
                    return true;
                }
                return false; // one or both failed
            }
        }

        /// <summary>
        /// Update a customer's email in AspNetUsers table
        /// </summary>
        /// <param name="customer">Customer to update</param>
        /// <returns>True on success, false otherwise</returns>
        public static bool UpdateAccountEmail(Customer customer)
        {
            // update accounts table
            using (AccountEntities db = new AccountEntities())
            {
                // get account from AspNetUsers table by email
                var account = db.AspNetUsers.SingleOrDefault(accnt => accnt.Email == customer.CustEmail);
                if (account != null) // found account
                {
                    account.Email = customer.CustEmail;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public static bool UpdateCustomerEmail(Customer customer) 
        { 
            // update customers table
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer from Customer table by phone number
                var cust = db.Customers.SingleOrDefault(c => c.CustBusPhone == customer.CustBusPhone);
                if (customer != null) // found customer
                {
                    cust.CustEmail = customer.CustEmail;
                    db.SaveChanges();
                    return true;
                }
                return false; // one or both failed
            }
        }

        /// <summary>
        /// Get all bookings linked to a customer
        /// </summary>
        /// <param name="customerId">Customer object</param>
        /// <returns>List of Bookings</returns>
        public static List<Booking> GetBookings(Customer customer)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer and their bookings 
                Customer customerAndBookings = db.Customers.Include("Bookings")
                                        .Where(c => c.CustomerId == customer.CustomerId)
                                        .SingleOrDefault();
                return customerAndBookings.Bookings.ToList();
            }
        }

        /// <summary>
        /// Get Booking details of a booking
        /// </summary>
        /// <param name="bookingNo">Booking number</param>
        /// <param name="customer">Customer</param>
        /// <returns>List of booing details</returns>
        public static List<BookingDetail> GetBookingDetails(Customer customer, string bookingNo)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities()) 
            {
                // get customer with booking details loaded
                var withDetails = db.Customers.Include("Bookings.BookingDetails")
                                              .Where(c => c.CustomerId == customer.CustomerId)
                                              .SingleOrDefault();
                // get booking
                Booking booking = withDetails.Bookings.Where(b => b.BookingNo == bookingNo).SingleOrDefault();

                return booking.BookingDetails.ToList();
            }
        }

        /// <summary>
        /// Get fees associated to a booking detail
        /// </summary>
        /// <param name="detailId">booking detail identification</param>
        /// <param name="customer">Customer object</param>
        /// <returns>FEe object</returns>
        public static Fee GetFee(Customer customer, int detailId)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities()) 
            {
                // get customer with fee loaded
                var detail = db.BookingDetails.Include("Fee")
                                .Where(d => d.BookingDetailId == detailId)
                                .SingleOrDefault();

                return detail.Fee;
            }
        }

        /// <summary>
        /// See if user name is free to use
        ///     case insensitive
        /// </summary>
        /// <param name="userName">user name to check</param>
        /// <returns>True if free to use, otherwise false</returns>
        public static bool IsUniqueUserName(string userName)
        {
            using (AccountEntities db = new AccountEntities())
            {
                var taken = db.AspNetUsers.SingleOrDefault(accnt => accnt.UserName.ToLower() == userName.ToLower());
                return taken == null;
            }

        }

        /// <summary>
        /// See if there's already an account linked to this email, case insensitive
        /// </summary>
        /// <param name="custEmail"></param>
        /// <returns>true if no account is linked with email, false otherwise</returns>
        public static bool IsUniqueEmail(string custEmail, out string error)
        {
            using (AccountEntities db = new AccountEntities())
            {
                var taken = db.AspNetUsers.SingleOrDefault(accnt => accnt.Email.ToLower() == custEmail.ToLower());
                if (taken == null)
                {
                    error = "";
                    return true;
                }
                error = "An account already exists with this email.";
                return false;
            }
        }

        /// <summary>
        /// Find an account by user id 
        /// </summary>
        /// <param name="userId">identification for account</param>
        /// <returns>The email of the account</returns>
        public static string GetEmailInAccount(string userId)
        {
            using (AccountEntities db = new AccountEntities())
            {
                return db.AspNetUsers.SingleOrDefault(accnt => accnt.Id == userId).Email;
            }
        }

        /// <summary>
        /// Retrieve customer by email
        /// </summary>
        /// <param name="email">email of customer</param>
        /// <returns>Customer object</returns>
        public static Customer GetCustomer(string email)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                return db.Customers.SingleOrDefault(cust => cust.CustEmail == email);
            }
        }

        /// <summary>
        /// See if phone number is unique in Accounts table
        /// </summary>
        /// <param name="custPhone"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool IsUniquePhone(string custPhone, out string error)
        {
            error = "";
            using (AccountEntities db = new AccountEntities())
            {
                var taken = db.AspNetUsers.SingleOrDefault(cust => cust.PhoneNumber == custPhone);

                if (taken == null)
                    return true;

                error = "An account is already linked to this phone number.";
                return false;
            }
        }
    }
}