using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanTakeDamage {
    public void ApplyDamage(int damage, PlayerRef player, Action callback = null);

}
