using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : NetworkBehaviour
{
    public override void Spawned()
    {
        base.Spawned();
        if(Runner.LocalPlayer.PlayerId == 1)
        {
            //Tu bo quyen State Authority
            //Object.ReleaseStateAuthority();
            //Yeu cau quyen State Authority
            Object.RequestStateAuthority();
        }
      
    }
    
    private bool direction = false;
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.IsValid && HasStateAuthority)
        {
            transform.position += direction ?
                -Vector3.forward * Runner.DeltaTime * 2
                : Vector3.forward * Runner.DeltaTime * 2;
            if(transform.position.z >= 20 && direction == false)
            {
                direction = true;
            }
            if(transform.position.z <= 0 && direction == true)
            {
                direction = false;
            }
           
        }
    }
}
