//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BanDT28.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hedieuhanh
    {
        public Hedieuhanh()
        {
            this.Sanphams = new HashSet<Sanpham>();
        }
    
        public int Mahdh { get; set; }
        public string Tenhdh { get; set; }
    
        public virtual ICollection<Sanpham> Sanphams { get; set; }
    }
}