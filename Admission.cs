//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NorthenLightsHospital
{
    using System;
    using System.Collections.Generic;
    
    public partial class Admission
    {
        public int ID_Admission { get; set; }
        public bool Chirurgie_programme { get; set; }
        public System.DateTime Date_admission { get; set; }
        public Nullable<System.DateTime> Date_chirurgie { get; set; }
        public Nullable<System.DateTime> Date_du_conge { get; set; }
        public bool Televiseur { get; set; }
        public string NSS { get; set; }
        public Nullable<int> Numero_Lit { get; set; }
        public int ID_Medcin { get; set; }
    
        public virtual Lit Lit { get; set; }
        public virtual Medcin Medcin { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
