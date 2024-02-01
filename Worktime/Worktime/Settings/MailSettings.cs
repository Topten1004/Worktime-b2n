namespace Worktime.Settings
{
    public class MailSettings
    {
        public string Mail { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Smtp { get; set; } = string.Empty;
        public int PortSmtp { get; set; }
        public string Pop { get; set; } = string.Empty;
        public int PortPop { get; set; }
        public string Bcc { get; set; } = string.Empty;
    }
}
