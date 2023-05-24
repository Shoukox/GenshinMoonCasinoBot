using brok1.Instance.Types.Enums;

namespace brok1.Instance.Types;

public class AdminPanel
{
    public string userIdOrUserName { get; set; }
    public bool isUserName { get; set; }
    public double num { get; set; }
    public bool settingNum { get; set; }
    public char editNum { get; set; }
    public string function { get; set; }
    public EAdminPanelStage stage { get; set; }
}
