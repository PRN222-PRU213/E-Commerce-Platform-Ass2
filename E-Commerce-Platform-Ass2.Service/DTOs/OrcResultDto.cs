using Newtonsoft.Json;

namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class OrcResultDto
    {
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("object")]
        public OrcObject Object { get; set; } = null!;

        public bool Success => Object?.Msg == "OK";
        public string IdNumber => Object?.Id ?? string.Empty;
        public string FullName => Object?.Name ?? string.Empty;
    }

    public class OrcObject
    {
        [JsonProperty("msg")]
        public string Msg { get; set; } = string.Empty;

        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("card_type")]
        public string CardType { get; set; } = string.Empty;

        [JsonProperty("birth_day")]
        public string BirthDay { get; set; } = string.Empty;

        [JsonProperty("recent_location")]
        public string RecentLocation { get; set; } = string.Empty;

        [JsonProperty("issue_date")]
        public string IssueDate { get; set; } = string.Empty;

        [JsonProperty("issue_place")]
        public string IssuePlace { get; set; } = string.Empty;
    }
}

