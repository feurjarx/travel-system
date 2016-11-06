//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlTravelAgencySystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class route
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public route()
        {
            this.transfers = new HashSet<transfer>();
        }
    
        public int id { get; set; }
        public string type { get; set; }
        public Nullable<int> from_airport_id { get; set; }
        public Nullable<int> to_airport_id { get; set; }
        public string starting_address { get; set; }
        public System.TimeSpan starting_time { get; set; }
        public string final_address { get; set; }
        public int duration { get; set; }
        public int total_seats { get; set; }
        public Nullable<int> distance { get; set; }
        public int cost { get; set; }
    
        public virtual airport airport { get; set; }
        public virtual airport airport1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<transfer> transfers { get; set; }
    }
}
