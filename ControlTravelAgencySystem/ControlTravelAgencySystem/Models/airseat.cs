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
    
    public partial class airseat
    {
        public int id { get; set; }
        public int flight_id { get; set; }
        public string @class { get; set; }
    
        public virtual flight flight { get; set; }
    }
}
