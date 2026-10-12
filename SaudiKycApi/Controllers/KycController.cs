using Microsoft.AspNetCore.Mvc;
using SaudiKycApi.Services;

namespace SaudiKycApi.Controllers
{
    [ApiController]
    [Route("api/kyc")]
    public class KycController : ControllerBase
    {
        private readonly KycService _kycService;

        public KycController(KycService kycService)
        {
            _kycService = kycService;
        }

        // 1. فحص الهوية
        [HttpGet("validate-id")]
        public IActionResult ValidateId([FromQuery] string idNumber)
        {
            var result = _kycService.ValidateSaudiId(idNumber);
            return Ok(result);
        }

        // 2. فحص الآيبان
        [HttpGet("validate-iban")]
        public IActionResult ValidateIban([FromQuery] string iban)
        {
            var result = _kycService.ValidateSaudiIban(iban);
            return Ok(result);
        }

        // 3. فحص العنوان الوطني
        [HttpGet("validate-address")]
        public IActionResult ValidateAddress([FromQuery] string address)
        {
            var result = _kycService.ValidateNationalAddress(address);
            return Ok(result);
        }

        // 4. فحص الرقم الضريبي
        [HttpGet("validate-tin")]
        public IActionResult ValidateTin([FromQuery] string tin)
        {
            var result = _kycService.ValidateZatcaTin(tin);
            return Ok(result);
        }

        // 5. فحص السجل التجاري
        [HttpGet("validate-cr")]
        public IActionResult ValidateCr([FromQuery] string crNumber)
        {
            var result = _kycService.ValidateCommercialRegistration(crNumber);
            return Ok(result);
        }

        // 6. فحص الجوال + المشغّل
        [HttpGet("validate-mobile")]
        public IActionResult ValidateMobile([FromQuery] string mobileNumber)
        {
            var result = _kycService.ValidateSaudiMobile(mobileNumber);
            return Ok(result);
        }
    }
}