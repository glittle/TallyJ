//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TallyJ.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class Vote
    {
        public int C_RowId { get; set; }
        public System.Guid BallotGuid { get; set; }
        public int PositionOnBallot { get; set; }
        public Nullable<System.Guid> PersonGuid { get; set; }
        public byte[] PersonRowVersion { get; set; }
        public string StatusCode { get; set; }
        public Nullable<System.Guid> InvalidReasonGuid { get; set; }
        public Nullable<int> SingleNameElectionCount { get; set; }
        public byte[] C_RowVersion { get; set; }
    }
}
