//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MechanicsForum.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Problem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Problem()
        {
            this.Answers = new HashSet<Answer>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string MediaPath { get; set; }
        public string location { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> PostDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> DateClosed { get; set; }
        public string SolvedBy { get; set; }
        public string ClosedBy { get; set; }
        public string Summary { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
