//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlTravelAgencySystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class flight
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public flight()
        {
            this.airseats = new HashSet<airseat>();
            this.airtickets = new HashSet<airticket>();
        }
    
        public int id { get; set; }
        public Nullable<int> airline_id { get; set; }
        public string code { get; set; }
        public int from_airport_id { get; set; }
        public int to_airport_id { get; set; }
        public int flight_at { get; set; }
        public int duration { get; set; }
        public int cost { get; set; }
        public int total_seats { get; set; }
    
        public virtual airline airline { get; set; }
        public virtual airport airport { get; set; }
        public virtual airport airport1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<airseat> airseats { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<airticket> airtickets { get; set; }
    }
}