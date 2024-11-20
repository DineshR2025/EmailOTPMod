using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmailOTPMod
{
    public class EmailOTPModule
    {
        private int? currentOtp;
        private DateTime? otpStartTime;
        private int otpAttempts;

        private const int MaxAttempts = 10;              // Maximum number of OTP entry attempts
        private const int OtpDuration = 60;              // OTP validity period in seconds
        private const string AllowedDomain = ".dso.org.sg";  // Authorized email domain for OTP generation

        // Status codes for module operations
        public const string StatusEmailOk = "Email Sent";
        public const string StatusEmailFail = "Email Failed";
        public const string StatusEmail = "Email Sent";
        public const string StatusEmailInvalid = "Invalid Email";
        public const string StatusOtpOk = "OTP is valid and checked";
        public const string StatusOtpFail = "OTP is wrong after 10 tries. OTP Failed.";
        public const string StatusOtpTimeout = "OTP Timeout";
        public List<string> ExistingEmailID = new List<string> { 
            "dinesh.rajavel@dso.org.sg", "Admin@dso.org.sg", "User1@dso.org.sg" };


        /// <summary>
        /// Initializes or resets OTP-related variables at the start of a session.
        /// </summary>
        public void Start()
        {
            ResetOtp();
        }

        /// <summary>
        /// Releases resources, clearing OTP data at the end of a session.
        /// </summary>
        public void Close()
        {
            ResetOtp();
        }

        /// <summary>
        /// Generates a 6-digit OTP and sends it to the user’s email if the email is valid.
        /// </summary>
        /// <param name="userEmail">Email address provided by the user.</param>
        /// <returns>Status code indicating success or failure of the email operation.</returns>
        public string GenerateOtpEmail(string userEmail)
        {
            if (ValidateEmail(userEmail) == 1)
                return StatusEmailInvalid;
            else if(ValidateEmail(userEmail) == 2)
                return StatusEmailFail;

            var random = new Random();
            currentOtp = random.Next(100000, 999999);
            otpStartTime = DateTime.Now;

            string emailBody = $"Your OTP Code is {currentOtp}. The code is valid for 1 minute.";
            try
            {
                SendEmail(userEmail, emailBody);
                return StatusEmailOk;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending email: {e.Message}");
                return StatusEmailFail;
            }
        }

        /// <summary>
        /// Verifies user input against the generated OTP, with timeouts and a limited number of attempts.
        /// </summary>
        /// <param name="inputStream">Function to read the user's OTP input from the console.</param>
        /// <returns>Status code indicating the result of OTP validation.</returns>
        public string CheckOtp(Func<string> inputStream)
        {
            if (currentOtp == null || otpStartTime == null)
                return StatusOtpTimeout;

            while (otpAttempts < MaxAttempts)
            {
                if ((DateTime.Now - otpStartTime.Value).TotalSeconds > OtpDuration)
                {
                    ResetOtp();
                    return StatusOtpTimeout;
                }

                string userOtpInput = inputStream();
                if (userOtpInput == currentOtp.ToString())
                {
                    ResetOtp();
                    return StatusOtpOk;
                }
                else
                {
                    otpAttempts++;
                    Console.WriteLine("Invalid OTP. Try again.");
                }
            }
            return StatusOtpFail;
        }

        /// <summary>
        /// Validates the email format and domain.
        /// </summary>
        /// <param name="email">Email address to validate.</param>
        /// <returns>True if the email is valid and from the allowed domain, false otherwise.</returns>
        private int ValidateEmail(string email)
        {
            if(!Regex.IsMatch(email, @"^[a-zA-Z0-9_.+-]+@dso\.org\.sg$"))
            {
                return 1;
            }
            bool isEmailexists = ExistingEmailID.Any(x => x.Equals(email));
            return isEmailexists ? 3 : 2;
        }

        /// <summary>
        /// Simulates sending an email by printing the message to the console.
        /// </summary>
        /// <param name="emailAddress">Recipient email address.</param>
        /// <param name="emailBody">Content of the email.</param>
        private void SendEmail(string emailAddress, string emailBody)
        {
            Console.WriteLine($"Sending email to {emailAddress} with body: {emailBody}");
        }

        /// <summary>
        /// Resets OTP-related variables, clearing OTP data and attempt count.
        /// </summary>
        private void ResetOtp()
        {
            currentOtp = null;
            otpStartTime = null;
            otpAttempts = 0;
        }
    }

}
