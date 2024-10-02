using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    [SerializeField]
    GameObject LoadingObj;
    public void ShowLoading()
    {
        LoadingObj.gameObject.SetActive(true);
    }
    public void HideLoading()
    {
        LoadingObj.gameObject.SetActive(false);
    }
}
