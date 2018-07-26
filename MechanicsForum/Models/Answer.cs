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
    
    public partial class Answer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Answer()
        {
            this.AnswersMedias = new HashSet<AnswersMedia>();
        }
    
        public int Id { get; set; }
        public string AnswerDesc { get; set; }
        public string AnsweredBy { get; set; }
        public string MediaPath { get; set; }
        public Nullable<System.DateTime> AnswerDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int Problem_Id { get; set; }
        public Nullable<int> CountModified { get; set; }
    
        public virtual Problem Problem { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AnswersMedia> AnswersMedias { get; set; }
    }
}
