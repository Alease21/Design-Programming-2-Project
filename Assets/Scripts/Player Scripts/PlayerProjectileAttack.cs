using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MagicManager;

public class PlayerProjectileAttack : MonoBehaviour
{
    public Vector2 target;
    public float pierceChance = 1;

    private void Awake()
    {
        //Debug.Log("spawned");
        target = magicInstance.targetArea;
        Vector3 targetLocation = new Vector3(target.x - transform.position.x, target.y - transform.position.y, 0f);
        Quaternion rotation = Quaternion.LookRotation(targetLocation, transform.TransformDirection(Vector3.up));
        if (target.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
        {
            transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.GetComponent<SpriteRenderer>().flipY = true;
            transform.rotation = new Quaternion(0, 0, -rotation.z, -rotation.w);
        }
        float radiusIncrease = 2f;
        transform.localScale = new Vector3(radiusIncrease,radiusIncrease,1f);
    }

    private void Update()
    {
        float distance = Vector2.Distance(target, transform.position);
        if (distance > 0.1f)
        {
            distance = Vector2.Distance(target, transform.position);
            transform.position = Vector2.MoveTowards(transform.position, target, magicInstance.playerAttack.projSpeed * Time.deltaTime);

        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<IEnemy>() != null)
        {
            collision.gameObject.GetComponent<IEnemy>().TakeDamage(magicInstance.damage);
            //Debug.Log("hit");
            if(!CheckPierce())
            {
                Destroy(gameObject);
            }
        }
    }
    
    public bool CheckPierce()
    {
        float random = Random.Range(1, 100);
        if (random >= 100*pierceChance)
        {
            return false;
        }
        else
        {
            pierceChance -= 1f;
            return true;
        }
    }

}
