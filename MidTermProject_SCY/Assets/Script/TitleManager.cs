using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public GameObject ScorePanel;
    private bool ScorePanelActive = false;

    public void ScorePanelToggle()
    {
        ScorePanelActive = !ScorePanelActive;
        ScorePanel.SetActive(ScorePanelActive);
    }
}
