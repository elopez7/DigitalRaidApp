using System.ComponentModel;

namespace DigitalRaid.Models;

public class TicketComment
{
    public int Id { get; set; }

    [DisplayName("Member Comment")]
    public string Comment { get; set; }

    [DisplayName("Date")]
    public DateTimeOffset CreatedDate { get; set; }

    [DisplayName("Ticket")]
    public int TicketId { get; set; }

    [DisplayName("Team Member")]
    public string UserId { get; set; }

    //Navigation Properties
    public virtual Ticket Ticket { get; set; }
    public virtual DRUser User { get; set; }
}
