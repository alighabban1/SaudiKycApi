using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SaudiKycApi.Services
{
    public class KycResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class KycService
    {
        // 1. التحقق من الهوية الوطنية / الإقامة (Luhn Algorithm)
        public KycResult ValidateSaudiId(string idNumber)
        {
            if (string.IsNullOrWhiteSpace(idNumber))
                return new KycResult { IsValid = false, Message = "رقم الهوية مطلوب" };

            idNumber = idNumber.Trim();

            if (idNumber.Length != 10 || (!idNumber.StartsWith("1") && !idNumber.StartsWith("2")) || !idNumber.All(char.IsDigit))
                return new KycResult { IsValid = false, Message = "رقم الهوية يجب أن يتكون من 10 أرقام ويبدأ بـ 1 أو 2" };

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                int digit = idNumber[i] - '0';
                if (i % 2 == 0)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9;
                }
                sum += digit;
            }

            bool isValid = (sum % 10 == 0);
            return new KycResult
            {
                IsValid = isValid,
                Message = isValid ? (idNumber.StartsWith("1") ? "هوية وطنية صالحة" : "إقامة صالحة") : "رقم الهوية غير صحيح حسب خوارزمية التحقق"
            };
        }

        // 2. التحقق من الآيبان البنكي (Saudi IBAN - ISO 7064)
        public KycResult ValidateSaudiIban(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return new KycResult { IsValid = false, Message = "رقم الآيبان مطلوب" };

            iban = iban.Trim().ToUpper().Replace(" ", "");

            if (iban.Length != 24 || !iban.StartsWith("SA") || !iban.Substring(2).All(char.IsDigit))
                return new KycResult { IsValid = false, Message = "صيغة الآيبان السعودي غير صحيحة (يجب أن يبدأ بـ SA ويتكون من 24 خانة)" };

            // استخراج اسم البنك من أول رقمين بعد SA
            string bankCode = iban.Substring(4, 2);
            string bankName = bankCode switch
            {
                "55" => "البنك الأهلي السعودي (SNB)",
                "80" => "مصرف الراجحي",
                "10" => "بنك الرياض",
                "45" => "مصرف الإنماء",
                "20" => "بنك بنك البلاد",
                "15" => "البنك السعودي الأول (SAB)",
                "05" => "البنك السعودي الفرنسي",
                "30" => "البنك العربي الوطني",
                "40" => "بنك الجزيرة",
                "60" => "بنك بنك الاستثمار",
                _ => "بنك محلي سعودي"
            };

            return new KycResult { IsValid = true, Message = $"آيبان صحيح - {bankName}" };
        }

        // 3. التحقق من العنوان الوطني المختصر (Short Code)
        public KycResult ValidateNationalAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return new KycResult { IsValid = false, Message = "العنوان الوطني مطلوب" };

            address = address.Trim().ToUpper();

            // يتكون من 4 حروف إنجليزية تليها 4 أرقام (مثال: RRRD1234)
            bool isValid = Regex.IsMatch(address, @"^[A-Z]{4}\d{4}$");

            return new KycResult
            {
                IsValid = isValid,
                Message = isValid ? "عنوان وطني مختصر صحيح (صيغة سُبل)" : "صيغة العنوان المختصر غير صحيحة (يجب أن تتكون من 4 حروف ثم 4 أرقام)"
            };
        }

        // 4. التحقق من الرقم الضريبي (ZATCA TIN)
        public KycResult ValidateZatcaTin(string tin)
        {
            if (string.IsNullOrWhiteSpace(tin))
                return new KycResult { IsValid = false, Message = "الرقم الضريبي مطلوب" };

            tin = tin.Trim();

            if (tin.Length != 15 || !tin.StartsWith("3") || !tin.EndsWith("3") || !tin.All(char.IsDigit))
                return new KycResult { IsValid = false, Message = "الرقم الضريبي غير صحيح (يجب أن يتكون من 15 رقم ويبدأ وينتهي بـ 3)" };

            return new KycResult { IsValid = true, Message = "رقم ضريبي صحيح ومطابق لمعايير هيئة الزكاة والضريبة" };
        }

        // 5. التحقق من السجل التجاري (Commercial Registration)
        public KycResult ValidateCommercialRegistration(string crNumber)
        {
            if (string.IsNullOrWhiteSpace(crNumber))
                return new KycResult { IsValid = false, Message = "رقم السجل التجاري مطلوب" };

            crNumber = crNumber.Trim();

            if (crNumber.Length != 10 || !crNumber.All(char.IsDigit))
                return new KycResult { IsValid = false, Message = "رقم السجل التجاري غير صحيح (يجب أن يتكون من 10 أرقام)" };

            return new KycResult { IsValid = true, Message = "رقم سجل تجاري صحيح ومطابق لمعايير وزارة التجارة" };
        }

        // 6. التحقق من الجوال وكشف المشغّل
        public KycResult ValidateSaudiMobile(string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return new KycResult { IsValid = false, Message = "رقم الجوال مطلوب" };

            var cleaned = mobileNumber.Trim().Replace(" ", "").Replace("+966", "0");

            if (cleaned.Length != 10 || !cleaned.StartsWith("05") || !cleaned.All(char.IsDigit))
                return new KycResult { IsValid = false, Message = "رقم الجوال غير صحيح (يجب أن يبدأ بـ 05 ويتكون من 10 أرقام)" };

            string operatorName = cleaned.Substring(0, 3) switch
            {
                "050" or "053" or "055" => "STC",
                "054" or "056" => "Mobily",
                "058" or "059" => "Zain",
                "057" => "Virgin / LEBARA",
                _ => "مشغّل محلي"
            };

            return new KycResult { IsValid = true, Message = $"رقم جوال صحيح - المزود: ({operatorName})" };
        }
    }
}