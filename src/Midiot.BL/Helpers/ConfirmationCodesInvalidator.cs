using Midiot.Infrastructure.Entities;

namespace Midiot.BL.Helpers;

public static class ConfirmationCodesInvalidator
{
    public static void InvalidatePreviousConfirmationCodes(IEnumerable<ConfirmationCodeEntity> codes)
    {
        var codesToInvalidate = codes.Where(code => !code.IsUsed || code.Expiration > DateTime.UtcNow).ToList();
        codesToInvalidate.ForEach(code => code.IsUsed = true);
    }
}
