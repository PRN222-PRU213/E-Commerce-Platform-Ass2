namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class EKycResult
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public string CCCDNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public double FaceMatchScore { get; set; }

        public bool Liveness { get; set; }

        public static EKycResult Fail(string msg)
            => new EKycResult { IsSuccess = false, Message = msg };

        public static EKycResult Success()
            => new EKycResult { IsSuccess = true };
    }
}

