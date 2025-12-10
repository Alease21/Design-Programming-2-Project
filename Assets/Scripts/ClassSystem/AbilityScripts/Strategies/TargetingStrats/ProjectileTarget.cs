using System;
using System.Collections;
using UnityEngine;

namespace AbilitySystem
{
    [CreateNodeMenu("Targeting/Projectile")]
    public class ProjectileTarget : TargetingStrategy, ICanAffectOthers
    {
        public EffectType GetEffectType => _effectType;
        public LayerMask GetAffectedLayers => _affectedLayers;
        public float GetTargetingRange => _range;

        public override void StartTargeting(AbilityData abilityData, Action onFinished)
        {
            Vector3 dir = Vector3.zero;
            UnitScript unit = abilityData.GetUser.GetComponent<UnitScript>();

            if (unit.GetUnitType == UnitTypes.Player)
            {
                dir = abilityData.GetUser.GetComponent<PlayerMovement>().GetMouseDir;
            }
            else if (unit.GetUnitType == UnitTypes.Enemy)
            {
                EnemyFSM efsm = abilityData.GetUser.GetComponent<EnemyFSM>();
                Vector3 tarPos = efsm.GetCurTarget.transform.position;
                Vector3 pos = efsm.transform.position;

                dir = (tarPos - pos).normalized;
            }
            else
            {
                Debug.LogError("Proj target dir error. invalid unit type used");
            }

            GameObject abilityGO = Instantiate(Resources.Load<GameObject>("AbilityEffects/ProjectileEffectSprite"),
            abilityData.GetUser.transform.position, Quaternion.LookRotation(Vector3.back, -dir));

            abilityGO.GetComponent<Animator>().Play(abilityData.GetAbilityAnimName);

            Rigidbody2D rb = abilityGO.GetComponent<Rigidbody2D>();
            rb.AddForce(dir * _projectileSpeed, ForceMode2D.Impulse);

            ProjectileScript ps = abilityGO.AddComponent<ProjectileScript>();
            ps.layerMask = _affectedLayers;
            ps.range = _range;
            ps.StartCoroutine(ProjectileCoroutine(ps, abilityData, onFinished));
            ps.StartCoroutine(CheckDistTravelled(_range, ps));
        }

        public IEnumerator ProjectileCoroutine(ProjectileScript ps, AbilityData abilityData, Action onFinished)
        {
            yield return new WaitUntil(() => ps.isFinished);
            if (ps.target != null)
                abilityData.Targets = ps.target;
            onFinished();

            Destroy(ps.gameObject);
        }
        public IEnumerator CheckDistTravelled(float range, ProjectileScript ps)
        {
            Vector3 initPos = ps.transform.position;

            yield return new WaitUntil(() => (ps.transform.position - initPos).magnitude > range);
            ps.ProjectileFinished();
        }
    }
}