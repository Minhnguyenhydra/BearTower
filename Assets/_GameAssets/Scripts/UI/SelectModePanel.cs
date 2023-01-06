
public class SelectModePanel : Panel
{
    public void OnClickCampaign(int map)
    {
        HUDPanel.SelectedMap = map;
        SceneTransition.ChangeScene("Game");
    }

    public void OnClickEndless()
    {

    }
}
