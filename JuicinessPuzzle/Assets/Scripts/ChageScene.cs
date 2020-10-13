using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChageScene : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject HowToPanel;
    public GameObject fadePanel;
    public GameObject gameIntroPanel;

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName ("NoJuiceLevel1"))
        {
            SceneManager.LoadScene("NoJuiceLevel2");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("NoJuiceLevel2"))
        {
            SceneManager.LoadScene("NoJuiceLevel3");
        }
    }

    public void NextLevelAllJuice()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LotsOfJuice"))
        {
            SceneManager.LoadScene("LotsOfJuiceLevel2");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LotsOfJuiceLevel2"))
        {
            SceneManager.LoadScene("LotsOfJuiceLevel3");
        }
    }

    public void NextLevelSomeJuice()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SomeJuice1"))
        {
            SceneManager.LoadScene("SomeJuice2");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SomeJuice2"))
        {
            SceneManager.LoadScene("SomeJuice3");
        }
    }

    public void RestartLevel()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("NoJuiceLevel1"))
        {
            SceneManager.LoadScene("NoJuiceLevel1");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("NoJuiceLevel2"))
        {
            SceneManager.LoadScene("NoJuiceLevel2");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("NoJuiceLevel3"))
        {
            SceneManager.LoadScene("NoJuiceLevel3");
        }
    }

    public void RestartLevelFullJuice()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LotsOfJuice"))
        {
            SceneManager.LoadScene("LotsOfJuice");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LotsOfJuiceLevel2"))
        {
            SceneManager.LoadScene("LotsOfJuiceLevel2");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LotsOfJuiceLevel3"))
        {
            SceneManager.LoadScene("LotsOfJuiceLevel3");
        }
    }

    public void RestartLevelSomeJuice()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SomeJuice1"))
        {
            SceneManager.LoadScene("SomeJuice1");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SomeJuice2"))
        {
            SceneManager.LoadScene("SomeJuice2");
        }
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SomeJuice3"))
        {
            SceneManager.LoadScene("SomeJuice3");
        }
    }

    public void StartGame()
    {
        if (startPanel.activeInHierarchy)
        {
            startPanel.SetActive(false);
            fadePanel.SetActive(true);
        }
    }

    public void StartGameFromHowToPlay()
    {
        if (HowToPanel.activeInHierarchy)
        {
            HowToPanel.SetActive(false);
            fadePanel.SetActive(true);
        }
    }

    public void HowToPlay()
    {
        if (startPanel.activeInHierarchy)
        {
            startPanel.SetActive(false);
            HowToPanel.SetActive(true);
        }
    }

    public void HidePanel()
    {
        if(gameIntroPanel.activeSelf)
        {
            StartCoroutine(SetPanelFalse()); 
        }
    }

    IEnumerator SetPanelFalse()
    {
        yield return new WaitForSeconds(.8f);
        gameIntroPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
       // UnityEditor.EditorApplication.isPlaying = false;
    }
}
