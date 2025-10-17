using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScenesButtonsManager : MonoBehaviour
{
    public void LoadNextDay()
    {
        if (PlayerPrefs.GetString("currentState").Equals("FirstLeaving"))
        {
            PlayerPrefs.SetString("currentState", "StartDayTwo");
            PlayerPrefs.SetString("pastScene", "Menu");
            SceneManager.LoadScene("Terreo");
        }
    }

    public void ReloadBattle()
    {
        if (PlayerPrefs.GetString("currentState").Equals("GroupClass"))
        {
            PlayerPrefs.SetString("pastScene", "Class");
            SceneManager.LoadScene("BattleScene");
        }
    }
}
