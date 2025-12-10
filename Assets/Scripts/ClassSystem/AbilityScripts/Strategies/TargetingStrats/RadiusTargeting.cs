using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [CreateNodeMenu("Targeting/Radius")]
    public class RadiusTargeting : TargetingStrategy, ICanAffectOthers
    {
        public EffectType GetEffectType => _effectType;
        public LayerMask GetAffectedLayers => _affectedLayers;
        public float GetTargetingRange => _range;

        public override void StartTargeting(AbilityData abilityData, Action onFinished)
        {
            abilityData.Targets = GetGameObjectsInRadius(abilityData.GetUser);
            GameObject explosion = Instantiate(Resources.Load<GameObject>("AbilityEffects/ExplosionEffectSprite"), abilityData.GetUser.transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * _range;

            AnimationClip clip = Resources.Load<AnimationClip>($"Anims/{abilityData.GetAbilityAnimName}");
            explosion.GetComponent<Animator>().Play(abilityData.GetAbilityAnimName);
            explosion.GetComponent<GeneralEffectScript>().StartCoroutine(DestroyOnTimer(clip.length, explosion));
            onFinished();
        }
        private IEnumerator DestroyOnTimer(float duration, GameObject go)
        {
            yield return new WaitForSeconds(duration);
            Destroy(go);
        }
        private IEnumerable<GameObject> GetGameObjectsInRadius(PlayerAbilityController user)
        {
            Collider2D[] foundObjects = Physics2D.OverlapCircleAll(user.transform.position, _range, _affectedLayers);

            foreach (Collider2D collider in foundObjects)
            {
                yield return collider.gameObject;
            }
        }
    }
}