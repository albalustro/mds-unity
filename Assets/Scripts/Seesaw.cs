using UnityEngine;
using System.Collections;

public class Seesaw : MDSBehaviour {


	public Sprite Right_BIsGreater;
	public Sprite Middle_Equal;
	public Sprite Left_AIsGreater;

	public MathValidador mathValidator;

	private SpriteRenderer _spriteRenderer;


    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        int? A, B;

        mathValidator.GetNumericValues(out A, out B);

        if(!A.HasValue)
            A = 0;

        if(!B.HasValue)
            B = 0;

        if(A.Value > B.Value)
            _spriteRenderer.sprite = Left_AIsGreater;
        else if(A.Value == B.Value)
            _spriteRenderer.sprite = Middle_Equal;
        else if(A.Value < B.Value)
            _spriteRenderer.sprite = Right_BIsGreater;
    }
}
