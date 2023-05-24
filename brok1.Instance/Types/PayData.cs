using brok1.Instance.Types.Enums;
using Qiwi.BillPayments.Model.Out;

namespace brok1.Instance.Types;

public class PayData
{
    public EPayStatus payStatus { get; set; }
    public int payAmount { get; set; }
    public BillResponse billResponse { get; set; }
}
