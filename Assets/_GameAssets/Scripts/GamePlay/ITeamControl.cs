
    using System.Collections.Generic;

    public interface ITeamControl
    {
        TeamMgr Team { get; set; }
        void Init(TeamMgr team);
        void Setup();
    }
