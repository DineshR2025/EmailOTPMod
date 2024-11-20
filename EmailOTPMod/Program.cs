/*
 * EmailOTPModule.cs
 * 
 * Purpose: This module provides functionality to generate and validate a One-Time Password (OTP) sent to a user’s email.
 * The OTP is valid for 1 minute, and users have 10 attempts to enter the correct OTP.
 * This is designed to secure access to enterprise applications by validating authorized users from the specified email domain.
 * 
 * Author: Dinesh Rajavel
 * Version: 1.0.0
 * Date: 2024-11-09
 */

using System;
using System.Text.RegularExpressions;
using EmailOTPMod;

class Program
{
    static void Main(string[] args)
    {
        // Create an instance of the EmailOTPModule class
        var otpModule = new EmailOTPModule();
        otpModule.Start();  // Initialize the OTP module

        // Step 1: Ask the user to enter their email address
        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine();

        // Step 2: Generate and send OTP email
        string emailStatus = otpModule.GenerateOtpEmail(email);
        Console.WriteLine(emailStatus);  // Output the result of email generation

        if (emailStatus == EmailOTPModule.StatusEmailOk)
        {
            // Step 3: If the email was successfully sent, prompt the user to enter the OTP
            Console.WriteLine("Please enter the OTP sent to your email (you have 1 minute and 10 attempts):");

            // Step 4: Check OTP entered by the user
            string otpStatus = otpModule.CheckOtp(() => Console.ReadLine());
            Console.WriteLine(otpStatus);  // Output OTP validation result
        }
        else if (emailStatus == EmailOTPModule.StatusEmailInvalid)
        {
            // Handle invalid email
            Console.WriteLine("The provided email address is invalid. Please use a valid .dso.org.sg email.");
        }
        else if (emailStatus == EmailOTPModule.StatusEmailFail)
        {   // Handle Non-existing email
            Console.WriteLine("The provided email address is not exist or sending to the email has failed.");
        }
        else
        {
            // Handle other failure scenarios (email sending failed, etc.)
            Console.WriteLine("Failed to send OTP. Please try again later.");
        }

        otpModule.Close();  // Clean up and close the OTP module
    }
}
