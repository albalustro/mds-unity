using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MDSBehaviour {

	private Animator m_anim;
    private SpriteRenderer _spriteRenderer;


	private const string paramAwayName = "away";
    private const string paramWalkingName = "walking";
    private const string paramBackName = "back";


	private readonly int paramAwayHash = Animator.StringToHash(paramAwayName);
    private readonly int paramWalkingHash = Animator.StringToHash(paramWalkingName);
    private readonly int paramBackHash = Animator.StringToHash(paramBackName);

    protected override void Awake()
	{
        base.Awake();
        m_anim = GetComponentInChildren<Animator>();
        if (m_anim == null) 
			LogError ("Não existe Animator neste GameObject ou nos filhos");

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	public void SetWalking(bool walking, Vector2 direction )
    {
        m_anim.SetBool(paramWalkingHash, walking);
        m_anim.SetBool(paramBackHash, direction.y>0);

        _spriteRenderer.flipX = direction.x > 0;
    }

    public void SetAway()
    {
        m_anim.SetTrigger(paramAwayHash);
    }

}
