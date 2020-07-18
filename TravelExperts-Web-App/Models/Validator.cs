using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace TravelExperts_Web_App.Models
{
    /// <summary>
    /// Collection of validation methods
    /// </summary>
  
    public static class Validator
    {
        // valid canadian postal code regular expression
        // @author - Vassillis Petroulias http://regexlib.com/REDetails.aspx?regexp_id=1293
        //      modified with start and end anchor, as well as to allow for any white space or a hyhen in middle 
        private static string postalRegex = @"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ][\s-]?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$";

        // valid canadian phone number regular expression
        // @author - Yuri Khenokh http://regexlib.com/Search.aspx?k=canadian+phone+number&c=-1&m=-1&ps=20
        // private static string phoneRegex = @"^(?:(?:\+?1[\s])|(?:\+?1(?=(?:\()|(?:\d{10})))|(?:\+?1[\-](?=\d)))?(?:\([2-9]\d{2}\)\ ?|[2-9]\d{2}(?:\-?|\ ?))[2-9]\d{2}[- ]?\d{4}$";
        
        // a simple phone regex - only allow provincial area code and number
        private static string noBrack = @"^[1-9](\d{9})$";

        /// <summary>
        /// Check for a valid postal code
        /// </summary>
        /// <param name="postal">String to test</param>
        /// <param name="error">Return an error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsCanadianPostal(string postal, out string error)
        {
            // initialize regex options - case insensitive
            RegexOptions options = RegexOptions.IgnoreCase;

            // initialize regex object
            Regex regex = new Regex(postalRegex, options);

            if (regex.IsMatch(postal)) // regex matched
            {
                error = "";
                return true;
            }

            error = "Invalid Postal Code";
            return false;
        }

        /// <summary>
        /// Check for a valid phone number
        /// </summary>
        /// <param name="phone">String to test</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsCanadianPhoneNumber(string phone, out string error)
        {
            // initialize regex object
            Regex regex = new Regex(noBrack);

            if (regex.IsMatch(phone))
            {
                error = "";
                return true;
            }

            error = "Invalid phone number";
            return false;
        }
    }
}