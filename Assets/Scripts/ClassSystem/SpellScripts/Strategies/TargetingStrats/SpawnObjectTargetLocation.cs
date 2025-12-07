using System;
using System.Collections;
using UnityEngine;

namespace MagicSystem
{
    [CreateNodeMenu("Targeting/Projectile")]
    public class SpawnObjectTargetLocation : TargetingStrategy, ICanAffectOthers
    {
        public EffectType GetEffectType => _effectType;
        public LayerMask GetAffectedLayers => _affectedLayers;
        public float GetTargetingRange => _range;

        public GameObject objectToSpawn;
        public float objDuration;

        public override void StartTargeting(SpellData spellData, Action onFinished)
        {
            Vector3 dir = Vector3.zero;

            if (spellData.GetUser.GetUnit.GetUnitType == UnitTypes.Player)
            {
                dir = spellData.GetUser.GetComponent<PlayerMovement>().GetMouseDir;
            }
            else if (spellData.GetUser.GetUnit.GetUnitType == UnitTypes.Enemy)
            {
                EnemyFSM efsm = spellData.GetUser.GetComponent<EnemyFSM>();
                Vector3 tarPos = efsm.GetCurTarget.transform.position;
                Vector3 pos = efsm.transform.position;

                dir = (tarPos - pos).normalized;
            }
            else
            {
                Debug.LogError("Proj target dir error. invalid unit type used");
            }

            GameObject obj = Instantiate(objectToSpawn,
                spellData.GetUser.transform.position, Quaternion.LookRotation(Vector3.back, -dir));

            obj.GetComponent<Animator>().Play(spellData.GetSpellAnimName);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            rb.AddForce(dir * _projectileSpeed, ForceMode2D.Impulse);

            ProjectileScript ps = obj.AddComponent<ProjectileScript>();
            ps.layerMask = _affectedLayers;
            ps.range = _range;
            ps.StartCoroutine(SpawnedObjCoroutine(ps, spellData, onFinished));
            ps.StartCoroutine(CheckDistTravelled(_range, ps));
        }

        public IEnumerator SpawnedObjCoroutine(ProjectileScript ps, SpellData spellData, Action onFinished)
        {
            yield return new WaitUntil(() => ps.isFinished);
            if (ps.target != null)
                spellData.Targets = ps.target;
            Rigidbody2D rb = ps.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            Debug.Log("trigger spawn obj aura here");
            yield return new WaitForSecondsRealtime(objDuration);
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