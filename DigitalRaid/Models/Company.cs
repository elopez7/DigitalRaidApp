using System.ComponentModel;

namespace DigitalRaid.Models;

public class Company
{
    public int Id { get; set; }

    [DisplayName("Company Name")]
    public string Name { get; set; }

    [DisplayName("Company Description")]
    public string Description { get; set; }

    //Navigation Properties
    public virtual ICollection<DRUser> Members { get; set; } = new HashSet<DRUser>();

    public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
}
