using System;
using System.Collections;
using UnityEngine;

namespace MagicSystem
{
    [CreateNodeMenu("Targeting/Projectile")]
    public class ProjectileTarget : TargetingStrategy, ICanAffectOthers
    {
        public EffectType GetEffectType => _effectType;
        public LayerMask GetAffectedLayers => _affectedLayers;
        public float GetTargetingRange => _range;

        public override void StartTargeting(SpellData spellData, Action onFinished)
        {
            Vector3 dir = PlayerMovement2.instance.GetMouseDir;

            GameObject spellGO = Instantiate(Resources.Load<GameObject>("ProjectileEffectSprite"),
                spellData.GetUser.transform.position, Quaternion.LookRotation(Vector3.back, -dir));
            
            spellGO.GetComponent<Animator>().Play(spellData.GetSpellAnimName);

            Rigidbody2D rb = spellGO.GetComponent<Rigidbody2D>();
            rb.AddForce(dir * _projectileSpeed, ForceMode2D.Impulse);

            ProjectileScript ps = spellGO.AddComponent<ProjectileScript>();
            ps.layerMask = _affectedLayers;
            ps.range = _range;
            ps.StartCoroutine(ProjectileCoroutine(ps, spellData, onFinished));
            ps.StartCoroutine(CheckDistTravelled(_range, ps));
        }

        public IEnumerator ProjectileCoroutine(ProjectileScript ps, SpellData spellData, Action onFinished)
        {
            yield return new WaitUntil(() => ps.isFinished);
            if (ps.target != null)
                spellData.Targets = ps.target;
            onFinished();

            Destroy(ps.gameObject);
        }
        public IEnumerator CheckDistTravelled(float range, ProjectileScript ps)
        {
            Vector3 initPos = ps.transform.position;

            yield return new WaitUntil(() => (ps.transform.position - initPos).magnitude > range);
            ps.isFinished = true;
        }
    }
}