using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppFeatures.Models;

namespace WebAppFeatures.ServicesRepository
{
    public interface IVerificationRepository
    {
        void SendRegistrationVerificationToken(string userId, string verficationToken);
        RegisterVerification GetRegistrationGeneratedToken(string userId);
        ResetPasswordVerification GetResetGeneratedToken(string userId);
        bool UpdateRegisterVerification(string userId);
        bool CheckIsAlreadyVerifiedRegistration(string userId);
        void SendResetVerificationToken(string userId, string verficationToken);
        bool UpdateResetVerification(string userId);
        bool CheckIsAlreadyVerifiedResetPassword(string userId, string hashtoken);
    }
}
