using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField]
    GameObject panelWinner;
    TextMeshProUGUI textWinner;
    Button playAgain;

    private List<RoboController> roboControllers = new List<RoboController>();
    public void AddRobo(RoboController roboController)
    {
        this.roboControllers.Add(roboController);
    }
    public void GetCountRobo(out int count)
    {
        count = roboControllers.Count;
    }
    public RoboController GetRobo(int id)
    {
        for (int i = 0; i < roboControllers.Count; i++)
        {
            if (roboControllers[i].Object.InputAuthority.PlayerId == id)
            {
                return roboControllers[i];
            }
        }
        return null;
    }
    public void OnAllRoboReady(int currentReady)
    {
        if(currentReady == roboControllers.Count)
        {
            Runner.SessionInfo.IsOpen = false;
            Runner.SessionInfo.IsVisible = false;
            //Singleton<Loading>.Instance.ShowLoading();
            if (Runner.IsSharedModeMasterClient)
            {
                 Runner.SetActiveScene(1);
            }
         
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }
        else
        {
            Debug.Log($"{currentReady}/{roboControllers.Count} Robo Realdy");
        }
      
    }
    public bool CheckWinner()
    {
        int countDie = 0;
        string name = "";

        foreach (var item in roboControllers)
        {
            if (item.Lives > 0)
            {
                name = "Player" + item.Object.InputAuthority.PlayerId;
            }
            if (item.Lives ==0)
            {
                countDie++;
            }
        }
        if (countDie==roboControllers.Count-1 &&countDie!=0)
        {
            Debug.Log("check winner true");
            GameObject panel= Instantiate(panelWinner);
            panel.SetActive(true);
            if (panelWinner != null)
            {
                textWinner ??= GameObject.FindGameObjectWithTag("PlayerNameWinner").GetComponent<TextMeshProUGUI>();
                playAgain ??= GameObject.FindGameObjectWithTag("ButtonAgain").GetComponent<Button>();
                
                textWinner.text = name + "is Winner";
                playAgain.onClick.AddListener(() =>
                {
                    panel.SetActive(false);
                    Runner.Shutdown(false);
                }
                );
            }
            
            return true;
        }
        return false;
    }
}
