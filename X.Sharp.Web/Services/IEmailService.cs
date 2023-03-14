// Copyright (c) Ax0ne.  All Rights Reserved

namespace X.Sharp.Web.Services
{
    public interface IEmailService
    {
        void Send(EmailMessage email);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
