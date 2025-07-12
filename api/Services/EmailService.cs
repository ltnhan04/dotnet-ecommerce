using System;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Utils;

namespace api.Services
{
    public class EmailService
    {
        private readonly SendEmail _emailSender;
        public EmailService()
        {
            _emailSender = new SendEmail();
        }

        public async Task SendVerificationEmail(string email, string verificationCode, string htmlTemplate)
        {
            string htmlBody = htmlTemplate.Replace("{verification}", verificationCode);
            await _emailSender.SendEmailAsync(email, "Verification Code", htmlBody);
        }
        public async Task SendPasswordResetRequestEmail(string email, string resetUrl, string htmlTemplate)
        {
            string htmlBody = htmlTemplate.Replace("{resetUrl}", resetUrl);
            await _emailSender.SendEmailAsync(email, "Password Reset Request", htmlBody);
        }
        public async Task SendPasswordResetSuccessEmail(string email, string htmlTemplate)
        {
            await _emailSender.SendEmailAsync(email, "Password Reset Successful", htmlTemplate);
        }
        public async Task SendOrderConfirmationEmail(
           string email,
           string htmlTemplate,
           Dictionary<string, string> placeholders)
        {
            string htmlBody = htmlTemplate;

            foreach (var pair in placeholders)
            {
                htmlBody = htmlBody.Replace($"{{{pair.Key}}}", pair.Value);
            }

            await _emailSender.SendEmailAsync(email, "Order Confirmation", htmlBody);
        }
        public async Task SendMilestoneReminderEmail(string email, string name, int currentPoints, int milestone)
        {
            string subject = $"🎯 Gần đủ điểm nhận voucher mốc {milestone}!";
            string htmlBody = $@"
        <p>Xin chào {name},</p>
        <p>Bạn hiện có <strong>{currentPoints}</strong> điểm tích lũy.</p>
        <p>Chỉ còn <strong>{milestone - currentPoints}</strong> điểm nữa là bạn có thể đổi voucher mốc {milestone} điểm rồi đấy 🎁</p>
        <p>Hãy tiếp tục mua sắm để nhận ưu đãi nhé!</p>
        <p><i>iTribe Shop</i></p>";

            await _emailSender.SendEmailAsync(email, subject, htmlBody);
        }

    }
}