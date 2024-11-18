using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleModule : MonoBehaviour
{
    public int health = 10;
    public Vector4 attackAllowed = new Vector4(0, 0, 0, 0);
    public Vector4 attackDirection = new Vector4(0, 0, 0, 0);
    public UnityEvent death;
    public enum eTeam { Player, Enemy };
    public eTeam team = eTeam.Player;
    
    public void Attacked() {
        health -= 1;
        if(health == 0 && death != null)
        {
            death.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        BattleModule other = collision.gameObject.GetComponent<BattleModule>();
        // if collision object has battle module, attack
        Vector3 contactNormal = collision.GetContact(0).normal;
        Attack(other, contactNormal);
    }
    private void OnTriggerEnter(Collider otherCollider)
    {
        BattleModule other = otherCollider.gameObject.GetComponent<BattleModule>();
        Vector3 contactNormal = (otherCollider.transform.position - transform.position).normalized;
        Attack(other, contactNormal);
    }

    void Attack(BattleModule other, Vector3 contactNormal) 
    {
        if (other != null && other.team != team)
        {
            if (Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y))
            {
                if (contactNormal.x > 0)
                {
                    // attack to the left (0, 0, 1, 0)
                    if (attackDirection.z > 0 && other.attackAllowed.w > 0)
                    {
                        other.Attacked();
                    }
                }
                else
                {
                    // attack to the right (0, 0, 0, 1)
                    if (attackDirection.w > 0 && other.attackAllowed.z > 0)
                    {
                        other.Attacked();
                    }
                }
            }
            else
            {
                if (contactNormal.y > 0)
                {
                    // attack to the bottom (0, 1, 0, 0)
                    if (attackDirection.y > 0 && other.attackAllowed.x > 0)
                    {
                        other.Attacked();
                    }
                }
                else
                {
                    // attack to the top (1, 0, 0, 0)
                    if (attackDirection.x > 0 && other.attackAllowed.y > 0)
                    {
                        other.Attacked();
                    }
                }
            }
        }
    }
}
