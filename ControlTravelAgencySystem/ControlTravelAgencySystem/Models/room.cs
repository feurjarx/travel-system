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
    
    public partial class room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public room()
        {
            this.callout_room = new HashSet<callout_room>();
            this.hotel_service_order = new HashSet<hotel_service_order>();
        }
    
        public int id { get; set; }
        public string number { get; set; }
        public int hotel_id { get; set; }
        public int cost_per_day { get; set; }
        public string @class { get; set; }
        public int seats_number { get; set; }
        public Nullable<int> room_size { get; set; }
        public string description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<callout_room> callout_room { get; set; }
        public virtual hotel hotel { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<hotel_service_order> hotel_service_order { get; set; }
    }
}
