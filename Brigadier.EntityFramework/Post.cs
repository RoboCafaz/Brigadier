//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Brigadier.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Post
    {
        public int Id { get; set; }
        public int LocalThreadId { get; set; }
        public int LinkTypeId { get; set; }
        public int TargetThreadId { get; set; }
        public System.DateTime Created { get; set; }
        public bool Done { get; set; }
    
        public virtual LinkType LinkType { get; set; }
        public virtual Thread LocalThread { get; set; }
        public virtual Thread TargetThread { get; set; }
    }
}
