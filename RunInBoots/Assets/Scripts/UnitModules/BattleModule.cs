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
    public UnityEvent preAttacked;
    public enum eTeam { Player, Enemy };
    public eTeam team = eTeam.Player;
    public int invincibleTime = 10;

    private UnityEvent _attacked;
    private Material[] _material;
    private Material[] _invincibleMaterial;
    private Renderer[] _renderers;
    private Color[] _originColors;
    private int _defaultLayer;
    private bool _isTransparent = false;
    

    // Start is called before the first frame update
    void Start()
    {
        _attacked = new UnityEvent();
        _attacked.AddListener(OnAttacked);
        _renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // add all materials in the array
        int materialCount = 0;
        for (int i = 0; i < _renderers.Length; i++)
            materialCount += _renderers[i].materials.Length;

        // init array
        _material = new Material[materialCount];
        _invincibleMaterial = new Material[materialCount];

        int index = 0;
        for (int i = 0; i < _renderers.Length; i++)
        {
            Material[] mats = _renderers[i].materials;
            for (int j = 0; j < mats.Length; j++)
            {
                _material[index] = new Material(mats[j]);
                _invincibleMaterial[index] = new Material(_material[index]);
                // emission color for invincible
                _invincibleMaterial[index].SetColor("_EmissionColor", new Color(0.25f, 0.25f, 0.25f));
                _invincibleMaterial[index].EnableKeyword("_EMISSION");
                index++;
            }
        }
        _defaultLayer = gameObject.layer;
    }

    public void BeInvincible()
    {
        StartCoroutine(Invincible());
    }
    
    public void Attacked()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Invincible")) return;
        preAttacked.Invoke();
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

    IEnumerator Invincible()
    {
        float elapsedTime = 0f;
        float blinkTime = 0.5f;
        
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        while (elapsedTime < invincibleTime)
        {
            if(elapsedTime % blinkTime < 0.01f)
            {
                SetMaterial();
                _isTransparent = !_isTransparent;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.layer = _defaultLayer;
        _isTransparent = true;
        SetMaterial();
        _isTransparent = false;
    }

    private void SetMaterial() 
    {
        int index = 0;
        for (int i = 0; i < _renderers.Length; i++)
        {
            var materials = _renderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
                materials[j] = _isTransparent ? _material[index++] : _invincibleMaterial[index++];
            _renderers[i].materials = materials;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        BattleModule other = collision.gameObject.GetComponent<BattleModule>();
        // if collision object has battle module, attack
        Vector3 contactNormal = collision.GetContact(0).normal;
        if(gameObject.CompareTag("AttackEffect")) Attack(other, contactNormal, true);
        else Attack(other, contactNormal);
    }
    private void OnTriggerEnter(Collider otherCollider)
    {
        BattleModule other = otherCollider.gameObject.GetComponent<BattleModule>();
        Vector3 contactNormal = (otherCollider.transform.position - transform.position).normalized;
        if(gameObject.CompareTag("AttackEffect")) Attack(other, contactNormal, true);
        else Attack(other, contactNormal);
    }

    void Attack(BattleModule other, Vector3 contactNormal, bool isVFX = false) 
    {
        if (other != null && other.team != team)
        {
            // if attack is from VFX, attack in all directions
            if(isVFX && other.attackAllowed != Vector4.zero) other.Attacked();
            
            // if attack is from player, attack in the direction of the player
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
