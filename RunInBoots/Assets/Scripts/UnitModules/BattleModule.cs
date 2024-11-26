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
    public int invincibleTime = 10;

    private UnityEvent _attacked;
    private Material[] _material;
    private Material[] _invincibleMaterial;
    private Renderer[] _renderers;
    private int _defaultLayer;
    private bool _isTransparent = false;

    // Start is called before the first frame update
    void Start()
    {
        _attacked = new UnityEvent();
        _attacked.AddListener(OnAttacked);
        _renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        _material = new Material[_renderers.Length];
        _invincibleMaterial = new Material[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _material[i] = new Material(_renderers[i].material);
            _invincibleMaterial[i] = new Material(_material[i]);
            _invincibleMaterial[i].color = new Color(_material[i].color.r, _material[i].color.g, _material[i].color.b, 0.5f);
        }
        _defaultLayer = gameObject.layer;
    }
    
    public void Attacked()
    {
        _attacked.Invoke();
    }

    private void OnAttacked()
    {
        health -= 1;
        if (health == 0 && death != null)
        {
            death.Invoke();
        }
        else if (health > 0)
        {
            Debug.Log("Invincible Coroutine");
            StartCoroutine(Invincible());
        }
    }

    public void BeInvinvible()
    {
        StartCoroutine(Invincible());
    }

    IEnumerator Invincible()
    {
        float elapsedTime = 0f;
        float blinkTime = 0.5f;
        
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        while (elapsedTime < invincibleTime)
        {
            if(elapsedTime % blinkTime < 0.01f)
            {
                if(_isTransparent)
                {
                    for(int i = 0; i < _renderers.Length; i++)
                    {
                        _renderers[i].material = _material[i];
                    }
                    _isTransparent = false;
                }
                else
                {
                    for(int i = 0; i < _renderers.Length; i++)
                    {
                        _renderers[i].material = _invincibleMaterial[i];
                    }
                    _isTransparent = true;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.layer = _defaultLayer;
        for(int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _material[i];
        }
        _isTransparent = false;
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
