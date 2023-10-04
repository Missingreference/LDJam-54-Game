using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;
using Unity.VisualScripting;
using UnityEngine.Audio;

public class Human : Actor
{
    public FacingDirection facingDirection = FacingDirection.Right;

    //Hover
    public float spriteHoverSpeed = 0.5f;
    public float spriteHoverDistance = 0.05f;

    //Attack
    public float attackSwipeSpeed { get; set; } = 7.0f;
    public float attackSwipeWidth { get; set; } = 0.5f;
    public float attackSwipeDistance { get; set; } = 1.0f;
    public float attackSwipeRotation { get; set; } = 80.0f;

    //Damage Flash
    public float damageFlashTime { get; private set; } = 0.2f;
    public int damageFlashCount { get; private set; } = 3;

    //Colliders
    public BoxCollider2D bodyTrigger { get; private set; }
    public BoxCollider2D movementCollider { get; private set; }


    //Weapon
    public HumanWeapon weapon { get; private set; }

    //Events
    public Action onAttack;
    public Action onFacingDirectionChanged;


    private Sprite m_MainSprite;
    private Sprite[] m_IdleSpriteSheet;

    private float m_HoverTimer = 0.0f;
    private float m_DamageFlashTimer = 0.0f;
    private float m_DamageFlashCount = 0;

    //public List<DamageTrigger> attackTriggers = new List<DamageTrigger>();
    //private List<DamageTrigger> m_PooledAttackTriggers = new List<DamageTrigger>();
    //private List<float> m_AttackSwipeTimers = new List<float>();
    //private List<float> m_AttackDirections = new List<float>();

    //Ability triggers
    public HumanSlashTrigger slashTrigger;
    public HumanCleaveTrigger cleaveTrigger;
    public HumanEarthTrigger earthTrigger;
    public HumanPoisonTrigger poisonTrigger;

    public HumanSmiteTrigger smiteTrigger;
    public HumanThrowKnifeTrigger knifeThrowTrigger;

    public RelicPickerUpper relicPickerUpper;

    private AudioSource m_AudioSource;

    protected override void Awake()
    {
        base.Awake();

        if(m_AudioSources == null)
        {
            int audioSourceCount = 10;
            m_AudioSources = new AudioSource[audioSourceCount];
            GameObject ap = new GameObject("Audio Player");
            AudioMixer mixer = Resources.Load<AudioMixer>("Audio Mixer");
            for(int i = 0; i < audioSourceCount; i++)
            {
                AudioSource source = ap.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
                m_AudioSources[i] = source;

                source.clip = m_HurtSound;
                source.minDistance = 50.0f;
            }
        }
        m_AudioSource = gameObject.AddComponent<AudioSource>();
        m_AudioSource.minDistance = 50.0f;
        m_AudioSource.clip = m_HurtSound;

        m_AudioSource.pitch = 3f;
        //moveSpeed = 1.0f;

        gameObject.layer = 7; //Player Movement

        m_MainSprite = Resources.Load<Sprite>("Sprites/Character/Character1");
        m_IdleSpriteSheet = Resources.LoadAll<Sprite>("Sprites/Character/Character_Idle_Sheet");
        spriteRenderer.sprite = m_MainSprite;
        spriteRenderer.gameObject.layer = 6; //Player Body

        bodyTrigger = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        bodyTrigger.isTrigger = true;
        bodyTrigger.offset = new Vector2(0.0f, 0.35f);
        bodyTrigger.size = (spriteRenderer.bounds.extents * 2.0f) * 0.85f;
        bodyTrigger.callbackLayers = (1 << 12);

        relicPickerUpper = bodyTrigger.AddComponent<RelicPickerUpper>();
        

        movementCollider = gameObject.AddComponent<BoxCollider2D>();
        movementCollider.isTrigger = false;
        movementCollider.offset = new Vector2(0.0f, 0.35f);
        movementCollider.size = (spriteRenderer.bounds.extents * 2.0f) * 0.9f;

        GameObject weaponObject = new GameObject("Weapon");
        weaponObject.transform.parent = transform;
        weapon = weaponObject.AddComponent<HumanWeapon>();

        //Ability triggers
        GameObject slashTriggerObject = new GameObject("Slash Trigger");
        slashTriggerObject.transform.parent = transform;
        slashTriggerObject.transform.localPosition = Vector3.zero;
        slashTriggerObject.transform.localEulerAngles = Vector3.zero;
        slashTriggerObject.transform.localScale = Vector3.one;
        slashTrigger = slashTriggerObject.AddComponent<HumanSlashTrigger>();

        /*
        GameObject knifeThrowTriggerObject = new GameObject("Knife Throw Trigger");
        knifeThrowTriggerObject.transform.parent = transform;
        knifeThrowTriggerObject.transform.localPosition = Vector3.zero;
        knifeThrowTriggerObject.transform.localEulerAngles = Vector3.zero;
        knifeThrowTriggerObject.transform.localScale = Vector3.one;
        knifeThrowTrigger = knifeThrowTriggerObject.AddComponent<HumanThrowKnifeTrigger>();
        */

        GameObject cleaveTriggerObject = new GameObject("Cleave Trigger");
        cleaveTriggerObject.transform.parent = transform;
        cleaveTriggerObject.transform.localPosition = Vector3.zero;
        cleaveTriggerObject.transform.localEulerAngles = Vector3.zero;
        cleaveTriggerObject.transform.localScale = Vector3.one;
        cleaveTrigger = cleaveTriggerObject.AddComponent<HumanCleaveTrigger>();
        

        GameObject earthTriggerObject = new GameObject("Earth Trigger");
        earthTriggerObject.transform.parent = transform;
        earthTriggerObject.transform.localPosition = Vector3.zero;
        earthTriggerObject.transform.localEulerAngles = Vector3.zero;
        earthTriggerObject.transform.localScale = Vector3.one;
        earthTrigger = earthTriggerObject.AddComponent<HumanEarthTrigger>();
        
        
        GameObject poisonTriggerObject = new GameObject("Poison Trigger");
        poisonTriggerObject.transform.parent = transform;
        poisonTriggerObject.transform.localPosition = Vector3.zero;
        poisonTriggerObject.transform.localEulerAngles = Vector3.zero;
        poisonTriggerObject.transform.localScale = Vector3.one;
        poisonTrigger = poisonTriggerObject.AddComponent<HumanPoisonTrigger>();
        
        GameObject smiteTriggerObject = new GameObject("Smite Trigger");
        smiteTriggerObject.transform.parent = transform;
        smiteTriggerObject.transform.localPosition = Vector3.zero;
        smiteTriggerObject.transform.localEulerAngles = Vector3.zero;
        smiteTriggerObject.transform.localScale = Vector3.one;
        smiteTrigger = smiteTriggerObject.AddComponent<HumanSmiteTrigger>();

        //slashTriggerObject.gameObject.SetActive(false);
        cleaveTriggerObject.gameObject.SetActive(false);
        earthTriggerObject.gameObject.SetActive(false);
        poisonTriggerObject.gameObject.SetActive(false);
        smiteTriggerObject.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        m_AudioSources = null;
    }


    protected override void Update()
    {
        AnimateHover();

        if(m_DamageFlashCount > 0)
        {
            AnimateDamageFlash();
        }
    }

    private int m_CurrentFrame = -1;
    private void AnimateHover()
    {
        m_HoverTimer += spriteHoverSpeed * Time.deltaTime;

        if(m_HoverTimer > 1.0f)
        {
            m_HoverTimer %= 1.0f;
        }

        float percent;
        if(m_HoverTimer < 0.5f)
        {
            percent = m_HoverTimer * 2.0f;
        }
        else
        {
            percent = 1.0f - ((m_HoverTimer - 0.5f) * 2.0f);
        }

        float height = Easing.Sinusoidal.InOut(spriteHoverDistance * -0.25f, spriteHoverDistance * 0.75f, percent);

        spriteRenderer.transform.localPosition = new Vector3(spriteRenderer.transform.localPosition.x, height, spriteRenderer.transform.localPosition.z);

        //Choose Sprite
        percent = m_HoverTimer;

        int frame = (int)(m_IdleSpriteSheet.Length * percent);
        if(frame != m_CurrentFrame)
        {
            m_CurrentFrame = frame;
            spriteRenderer.sprite = m_IdleSpriteSheet[frame];
        }
    }

    private void AnimateDamageFlash()
    {
        m_DamageFlashTimer += Time.deltaTime;
        if(m_DamageFlashTimer >= damageFlashTime)
        {
            m_DamageFlashTimer -= damageFlashTime;
            m_DamageFlashCount--;
            if(m_DamageFlashCount == 0)
            {
                m_DamageFlashTimer = 0.0f;
            }
        }

        if(m_DamageFlashCount > 0)
        {
            float percent = m_DamageFlashTimer / damageFlashTime;
            if(percent < 0.5f)
            {
                percent *= 1.0f;
                spriteRenderer.color = Easing.Linear.InOut(Color.white, Color.red, percent);
            }
            else
            {
                percent *= 0.5f;
                spriteRenderer.color = Easing.Linear.InOut(Color.red, Color.white, percent);
            }
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        m_DamageFlashCount = damageFlashCount;

        m_AudioSource.PlayOneShot(m_HurtSound);
    }

    protected override void OnMove()
    {
        if(!isAlive) return;

        rigidbody.AddForce(moveDirection * moveSpeed, ForceMode2D.Force);

        if(moveDirection.x < 0)
        {
            SetFacingDirection(FacingDirection.Left);
        }
        else if(moveDirection.x > 0)
        {
            SetFacingDirection(FacingDirection.Right);
        }
    }

    public void SetFacingDirection(FacingDirection direction) 
    {
        if(direction != facingDirection)
        {
            if(direction == FacingDirection.Left)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            facingDirection = direction;
            onFacingDirectionChanged?.Invoke();
        }
    }


    private void DealDamage(Actor target)
    {
        target.TakeDamage(5);
    }
}
