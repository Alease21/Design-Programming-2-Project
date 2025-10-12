using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityData
    {
        private PlayerTempScript _user;
        private IEnumerable<GameObject> _targets;

        public PlayerTempScript GetUser { get { return _user; } }
        public IEnumerable<GameObject> Targets { get { return _targets; } set { _targets = value; } }
        public AbilityData(PlayerTempScript user)
        {
            _user = user;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            _user?.StartCoroutine(coroutine);
        }
    }
}