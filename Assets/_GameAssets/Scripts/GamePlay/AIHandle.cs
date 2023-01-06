
    using System.Collections;
    using UnityEngine;

    public class AIHandle:MonoBehaviour,ITeamControl
    {
        public TeamMgr Team { get; set; }
        public void Init(TeamMgr team)
        {
            this.Team = team;
        }

        public void Setup()
        {
            StartCoroutine(SpawnCor());
        }

        private IEnumerator SpawnCor()
        {
            var i = 0;
            while (true)
            {
                yield return new WaitForSeconds(5);
                Team.trainingProcesses[i%Team.trainingProcesses.Count].SpawnHero();
                i++;
                if (i % 5 == 0) Team.CurState = (TeamMgr.State) (i / 5 % 3);
            }
            yield break;
        }
    }
