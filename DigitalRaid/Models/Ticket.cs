using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalRaid.Models;

public class Ticket
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [DisplayName("Title")]
    public string Title { get; set; }

    [Required]
    [DisplayName("Description")]
    public string Description { get; set; }

    [DataType(DataType.Date)]
    [DisplayName("Created")]
    public DateTimeOffset CreatedDate { get; set; }

    [DataType(DataType.Date)]
    [DisplayName("Updated")]
    public DateTimeOffset? UpdatedDate { get; set; }
    
    [DisplayName("Archived")]
    public bool Archived { get; set; }

    [DisplayName("Project")]
    public int ProjectId { get; set; }

    [DisplayName("Ticket Type")]
    public int TicketTypeId { get; set; }
    
    [DisplayName("Ticket Priority")]
    public int TicketPriorityId { get; set; }
    
    [DisplayName("Ticket Status")]
    public int TicketStatusId { get; set; }

    [DisplayName("Ticket Owner")]
    public string OwnerUserId { get; set; }

    [DisplayName("Ticket Developer")]
    public string DeveloperUserId { get; set; }

    //Navigation Properties
    public virtual Project Project { get; set; }
    public virtual TicketType TicketType { get; set; }
    public virtual TicketPriority TicketPriority { get; set; }
    public virtual TicketStatus TicketStatus { get; set; }
    public virtual DRUser OwnerUser { get; set; }
    public virtual DRUser DeveloperUser { get; set; }

    public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
    public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
    public virtual ICollection<Notifications> Notifications { get; set; } = new HashSet<Notifications>();
    public virtual ICollection<TicketHistory> History { get;  set; } = new HashSet<TicketHistory>();
}
