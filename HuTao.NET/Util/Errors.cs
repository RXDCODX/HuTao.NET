﻿namespace HuTao.NET.GI.Util;

public class Errors
{
    /// <summary>
    /// 有効な原神アカウントが見つからなかった際にスローされる例外
    /// </summary>
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException()
            : base() { }

        public AccountNotFoundException(string message)
            : base(message) { }

        public AccountNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// HoyoLabAPIから無効な結果が返ってきた際にスローされる例外
    /// </summary>
    public class HoyoLabApiBadRequestException : Exception
    {
        private readonly int retcode;

        public HoyoLabApiBadRequestException()
            : base() { }

        public HoyoLabApiBadRequestException(string message, int retcode)
            : base(message)
        {
            this.retcode = retcode;
        }

        public HoyoLabApiBadRequestException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// APIがキャプチャ認証でブロックされた際にスローされる例外
    /// </summary>
    public class HoyoLabCaptchaBlockException : Exception
    {
        public HoyoLabCaptchaBlockException()
            : base() { }

        public HoyoLabCaptchaBlockException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// ログイン報酬を既に受け取っている際にスローされる例外
    /// </summary>
    public class DailyRewardAlreadyReceivedException : Exception
    {
        public DailyRewardAlreadyReceivedException()
            : base() { }

        public DailyRewardAlreadyReceivedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
