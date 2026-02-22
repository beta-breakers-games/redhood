using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
 
public class MainMenu : MonoBehaviour
{

    public void Play()
    {

        SceneManager.LoadScene("RedHood");
    }

        public void Menu()
    {

        SceneManager.LoadScene("Menu");
    }
 
    public void Quit()
    {
        Application.Quit();
    }

}
