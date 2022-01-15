using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppFeatures.Models;

namespace WebAppFeatures.ServicesRepository
{
    public class VerificationRepository: IVerificationRepository
    {
        private readonly AppDbContext context;
        public VerificationRepository(AppDbContext _context)
        {
            context = _context;
        }
        public void SendRegistrationVerificationToken(string userId, string verficationToken)
        {
            RegisterVerification registerVerification = new ()
            {
                Id = 0,
                UserId = userId,
                GeneratedDate = DateTime.Now,
                GeneratedToken = verficationToken,
                Status = true,
                VerificationStatus = false
            };
            context.RegisterVerification.Add(registerVerification);
            context.SaveChanges();
        }
        public RegisterVerification GetRegistrationGeneratedToken(string userId)
        {
            RegisterVerification registerVerification = context.RegisterVerification.Where(a => a.UserId == userId).OrderByDescending(a => a.GeneratedDate).FirstOrDefault();
            return registerVerification;
        }
        public bool UpdateRegisterVerification(string userId)
        {
            RegisterVerification registerVerification = context.RegisterVerification.Where(a => a.UserId == userId).FirstOrDefault();
            if (registerVerification != null)
            {
                registerVerification.VerificationStatus = true;
                registerVerification.VerificationDate = DateTime.Now;
                context.RegisterVerification.Update(registerVerification);
            }
            return context.SaveChanges() > 0;
        }
        public bool CheckIsAlreadyVerifiedRegistration(string userId)
        {
            bool registerVerification = context.RegisterVerification.Where(a => a.UserId == userId
            && a.VerificationStatus == true).Any();
            return registerVerification;
        }
        
        public void SendResetVerificationToken(string userId, string verficationToken)
        {
            ResetPasswordVerification registerVerification = new()
            {
                Id = 0,
                GeneratedDate = DateTime.Now,
                GeneratedToken = verficationToken,
                UserId = userId,
                Status = true,
                VerificationStatus = false
            };
            context.ResetPasswordVerification.Add(registerVerification);
            context.SaveChanges();
        }
        public ResetPasswordVerification GetResetGeneratedToken(string userId)
        {
            ResetPasswordVerification resetPasswordVerification = context.ResetPasswordVerification.Where(a => a.UserId == userId).OrderByDescending(a=>a.GeneratedDate).FirstOrDefault();
            return resetPasswordVerification;
        }
        public bool UpdateResetVerification(string userId)
        {
            ResetPasswordVerification resetVerification = context.ResetPasswordVerification.Where(a => a.UserId == userId).OrderByDescending(a => a.GeneratedDate).FirstOrDefault();
            if (resetVerification != null)
            {
                resetVerification.VerificationStatus = true;
                resetVerification.VerificationDate = DateTime.Now;
                context.ResetPasswordVerification.Update(resetVerification);
            }
            return context.SaveChanges() > 0;
        }
        public bool CheckIsAlreadyVerifiedResetPassword(string userId,string hashtoken)
        {
            bool resetPasswordVerification = context.ResetPasswordVerification.Where(a => a.UserId == userId
            && a.GeneratedToken == hashtoken).Select(a=>a.VerificationStatus).FirstOrDefault();
            return resetPasswordVerification;
        }
    }
}
