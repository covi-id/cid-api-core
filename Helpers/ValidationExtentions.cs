using CoviIDApiCore.Exceptions;
using CoviIDApiCore.V1.Constants;
using CoviIDApiCore.V1.DTOs.Credentials;
using System;
using System.IO;

namespace CoviIDApiCore.Helpers
{
    public static class ValidationExtentions
    {
        public static string ValidateLength(this string str)
        {
            if (str.Length >= 2 && str.Length <= 255)
            {
                return str;
            }
            throw new ValidationException(Messages.Val_Length);
        }

        public static long ValidateMobileNumber(this long num)
        {
            var lenght = num.ToString().Length;
            if (lenght > 9 && lenght < 16)
            {
                return num;
            }
            throw new ValidationException(Messages.Val_MobileNumber);
        }

        public static string ValidateIdentification(this string value, IdentificationTypes identificationType)
        {
            if (identificationType == IdentificationTypes.Passport)
            {
                if (value.Length > 6)
                {
                    return value;
                }
            }

            if (value.Length == 13)
            {
                return value;
            }

            throw new ValidationException(Messages.Val_Identification);
        }

        public static DateTime IsInPast(this DateTime date)
        {
            var defaultDate = new DateTime();
            if (date < DateTime.Today && date != defaultDate)
                return date;
            throw new ValidationException(Messages.Val_DateNotInPast);
        }

        public static bool IsAppropriateSize(this string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length <= 5242880) // 5 MB
                return true;
         
            return false;
        }
    }
}
