using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicSystem
{
    [CreateNodeMenu("Targeting/Radius")]
    public class RadiusTargeting : TargetingStrategy, ICanAffectOthers
    {
        public EffectType GetEffectType => _effectType;
        public LayerMask GetAffectedLayers => _affectedLayers;
        public float GetTargetingRange => _range;

        public override void StartTargeting(SpellData spellData, Action onFinished)
        {
            spellData.Targets = GetGameObjectsInRadius(spellData.GetUser);
            GameObject explosion = Instantiate(Resources.Load<GameObject>("ExplosionEffectSprite"), spellData.GetUser.transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * _range;

            AnimationClip clip = Resources.Load<AnimationClip>($"Anims/{spellData.GetSpellAnimName}");
            explosion.GetComponent<Animator>().Play(spellData.GetSpellAnimName);
            explosion.GetComponent<GeneralEffectScript>().StartCoroutine(DestroyOnTimer(clip.length, explosion));
            onFinished();
        }
        private IEnumerator DestroyOnTimer(float duration, GameObject go)
        {
            yield return new WaitForSeconds(duration);
            Destroy(go);
        }
        private IEnumerable<GameObject> GetGameObjectsInRadius(SpellController user)
        {
            Collider2D[] foundObjects = Physics2D.OverlapCircleAll(user.transform.position, _range, _affectedLayers);

            foreach (Collider2D collider in foundObjects)
            {
                yield return collider.gameObject;
            }
        }
    }
}