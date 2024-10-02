using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyManager : MonoBehaviour
{
    private List<TrackingReady> trackingReadies = new List<TrackingReady>();
    public void AddTrackingReady(TrackingReady trackingReady)
    {
        trackingReadies.Add(trackingReady);
    }
    public void GetCountReady(out int count)
    {
        int countRaw = 0;
        foreach (var item in trackingReadies)
        {
            if (item.GetReady())
            {
                countRaw++;
            }
        }
        count = countRaw;
    }
}
