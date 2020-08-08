using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace EMaintanance.Utils
{
    public class CustomUtils
    {
        private IConfiguration _configuration;

        public CustomUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public static void HandleException(SqlException sqlException)
        {
            if (sqlException.Number == 2601 || sqlException.Number == 2627)
            {
                if (sqlException.Message.Contains("UNIQUE KEY constraint"))
                {
                    var match = Regex.Matches(sqlException.Message, @"\b\w*_\w*\b").Cast<Match>().FirstOrDefault();
                    string PlaceHolder = match != null ? match.Value.Replace("UC_", "") : null;
                    if (PlaceHolder != null)
                    {
                        throw new CustomException("Duplicate", PlaceHolder + " already Exists.", "Error", true, sqlException);
                    }
                    else
                    {
                        throw new CustomException("Duplicate", "Record already Exists, Click view more details.", "Error", true, sqlException);
                    }

                }
            }
        }

        public static string EncodeToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public static string DecodeFromBase64(string encodedData)
        {
            string result = null;
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(encodedData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                result = new String(decoded_char);
            }
            catch (Exception ex)
            {
                result = "Encoded Input is not Valid, Please Check the Input Value - " + encodedData;
            }
            return result;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            //const string chars = "!@#$%^&*abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            //return new string(Enumerable.Repeat(chars, length)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());
            string charsAll = "!@#$%^&*abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var randomIns = new Random();
            int N = 10;
            var rndChars = Enumerable.Range(0, N)
                            .Select(_ => charsAll[randomIns.Next(charsAll.Length)])
                            .ToArray();
            rndChars[randomIns.Next(rndChars.Length)] = "0123456789"[randomIns.Next(10)];
            rndChars[randomIns.Next(rndChars.Length)] = "!@#$%^&*"[randomIns.Next(8)];

            return new String(rndChars);

        }

    }
}