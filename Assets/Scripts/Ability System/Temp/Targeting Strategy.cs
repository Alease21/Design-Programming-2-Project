using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace AbilitySystem
{
    public abstract class TargetingStrategy : AbilityNodeBase
    {
        [Input(connectionType = ConnectionType.Override)] public bool enter; //only one thing can be plugged in

        public abstract void StartTargeting(AbilityData abilityData, Action onFinished);
    }
}