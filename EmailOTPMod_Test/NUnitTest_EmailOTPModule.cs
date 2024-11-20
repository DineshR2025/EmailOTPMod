namespace EmailOTPMod.Tests
{
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class EmailOTPModuleTests
    {
        private EmailOTPModule _otpModule;

        [SetUp]
        public void Setup()
        {
            _otpModule = new EmailOTPModule();
        }

        [TearDown]
        public void TearDown()
        {
            _otpModule.Close();
        }

        [Test]
        public void Start_ResetsOtpState()
        {
            // Arrange
            _otpModule.GenerateOtpEmail("dinesh.rajavel@dso.org.sg");

            // Act
            _otpModule.Start();

            // Assert
            Assert.AreEqual(0, GetPrivateField<int>(_otpModule, "otpAttempts"));
            Assert.Null(GetPrivateField<int?>(_otpModule, "currentOtp"));
            Assert.IsNull(GetPrivateField<DateTime?>(_otpModule, "otpStartTime"));
        }

        [Test]
        public void GenerateOtpEmail_InvalidEmailFormat_ReturnsInvalidEmailStatus()
        {
            // Act
            var result = _otpModule.GenerateOtpEmail("invalid-email");

            // Assert
            Assert.AreEqual(EmailOTPModule.StatusEmailInvalid, result);
        }

        [Test]
        public void GenerateOtpEmail_ValidEmail_SendsEmailAndReturnsSuccess()
        {
            // Act
            var result = _otpModule.GenerateOtpEmail("dinesh.rajavel@dso.org.sg");

            // Assert
            Assert.AreEqual(EmailOTPModule.StatusEmailOk, result);
        }

        [Test]
        public void CheckOtp_CorrectOtpWithinTime_ReturnsOtpOk()
        {
            // Arrange
            _otpModule.GenerateOtpEmail("dinesh.rajavel@dso.org.sg");
            var expectedOtp = GetPrivateField<int>(_otpModule, "currentOtp");

            // Act
            var result = _otpModule.CheckOtp(() => expectedOtp.ToString());

            // Assert
            Assert.AreEqual(EmailOTPModule.StatusOtpOk, result);
        }

        [Test]
        public void CheckOtp_Timeout_ReturnsOtpTimeout()
        {
            // Arrange
            _otpModule.GenerateOtpEmail("dinesh.rajavel@dso.org.sg");
            System.Threading.Thread.Sleep(61000); // Simulate 61 seconds timeout

            // Act
            var result = _otpModule.CheckOtp(() => "123456");

            // Assert
            Assert.AreEqual(EmailOTPModule.StatusOtpTimeout, result);
        }

        [Test]
        public void CheckOtp_MaxAttemptsReached_ReturnsOtpFail()
        {
            // Arrange
            _otpModule.GenerateOtpEmail("dinesh.rajavel@dso.org.sg");

            // Act
            string result = null;
            for (int i = 0; i < 10; i++)
            {
                result = _otpModule.CheckOtp(() => "123456"); // Wrong OTP
            }

            // Assert
            Assert.AreEqual(EmailOTPModule.StatusOtpFail, result);
        }

        [Test]
        public void ValidateEmail_InvalidEmailDomain_ReturnsInvalidStatus()
        {
            // Act
            var result = CallPrivateMethod<int>(_otpModule, "ValidateEmail", "test@gmail.com");

            // Assert
            Assert.AreEqual(1, result); // 1 represents invalid email
        }

        [Test]
        public void ValidateEmail_ExistingEmail_ReturnsValidStatus()
        {
            // Act
            var result = CallPrivateMethod<int>(_otpModule, "ValidateEmail", "Admin@dso.org.sg");

            // Assert
            Assert.AreEqual(3, result); // 3 represents valid existing email
        }

        [Test]
        public void ValidateEmail_NewEmail_ReturnsFailStatus()
        {
            // Act
            var result = CallPrivateMethod<int>(_otpModule, "ValidateEmail", "newuser@dso.org.sg");

            // Assert
            Assert.AreEqual(2, result); // 2 represents email does not exist
        }

        [Test]
        public void ResetOtp_ClearsOtpState()
        {
            // Arrange
            _otpModule.GenerateOtpEmail("dinesh.rajavel@dso.org.sg");

            // Act
            CallPrivateMethod(_otpModule, "ResetOtp");

            // Assert
            Assert.IsNull(GetPrivateField<int?>(_otpModule, "currentOtp"));
            Assert.IsNull(GetPrivateField<DateTime?>(_otpModule, "otpStartTime"));
            Assert.AreEqual(0, GetPrivateField<int>(_otpModule, "otpAttempts"));
        }

        // Utility Methods for Testing Private Members
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }

        private void CallPrivateMethod(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(obj, parameters);
        }

        private T CallPrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)method.Invoke(obj, parameters);
        }
    }
}
