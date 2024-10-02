using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackingReady : NetworkBehaviour
{
    [Networked]
    Vector3 roboPosition { get; set; }
   
    Transform roboTrans {  get; set; }

    [Networked(OnChanged = nameof(OnToltalPlayerChanged))]
    int totalPlayer {  get; set; }

    [Networked(OnChanged =nameof(ChangedVisualReady))]
    private NetworkBool IsReady { get; set; }
    private TextMeshProUGUI text;
    public bool GetReady()
    {
        return IsReady;
    }
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    protected static void OnToltalPlayerChanged(Changed<TrackingReady> changed)
    {
        changed.Behaviour.StartDetect();
    }
    protected static void ChangedVisualReady(Changed<TrackingReady> changed)
    {
        changed.Behaviour.text.enabled = changed.Behaviour.IsReady;
        Singleton<ReadyManager>.Instance.GetCountReady(out int countCurrentReady);
        PlayerManager pm = FindObjectOfType<PlayerManager>();
        if (pm != null)
        {
            pm.OnAllRoboReady(countCurrentReady);
        }
        
    }
    public void HideReady()
    {
        if (HasStateAuthority && Object.IsValid)
        {
            this.IsReady = false;
            text.enabled = false;
        }
    }
    public void OnChangedReady()
    {
        if (HasStateAuthority)
        {
            bool newReady = !IsReady;
            this.IsReady = newReady;
            text.enabled = newReady;
        }
        
    }
    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            IsReady = false;
        }
        transform.parent = Singleton<ReadyManager>.Instance.transform;
        totalPlayer += 1;

        Singleton<ReadyManager>.Instance.AddTrackingReady(this);
    }
    public void StartDetect()
    {
        StartCoroutine(DetectRobo());
    }
    IEnumerator DetectRobo()
    {
        yield return new WaitForSeconds(0.3f);
        if (roboTrans == null)
        {
            RoboController[] allRobos = FindObjectsOfType<RoboController>();
            foreach (var robo in allRobos)
            {
                if (robo.Object.InputAuthority.PlayerId == Runner.LocalPlayer.PlayerId)
                {
                    SetRoboTrans(robo.transform);
                }
            }
        }
    }
    private void Update()
    {
       /* if (Input.GetKeyUp(KeyCode.R) && HasStateAuthority)
        {
           OnChangedReady(!IsReady);
        }*/
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority && roboTrans != null)
        {
            roboPosition = roboTrans.position;
        }
      
    }

    public override void Render()
    {
        base.Render();
        if (this.roboTrans != null)
        {
            transform.position = roboPosition + new Vector3(0, 4, 0);
        }
    }

    public void SetRoboTrans(Transform roboTrans)
    {
        this.roboTrans = roboTrans;
    }

    private void LateUpdate()
    {
       
    }
}
