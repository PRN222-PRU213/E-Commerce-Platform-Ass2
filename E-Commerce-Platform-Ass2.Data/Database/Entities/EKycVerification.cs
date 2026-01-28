namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    public class EKycVerification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string CccdNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public double FaceMatchScore { get; set; }

        public bool Liveness { get; set; }

        public string Status { get; set; } = string.Empty; // PENDING / VERIFIED / FAILED

        public DateTime CreatedAt { get; set; }
    }
}

