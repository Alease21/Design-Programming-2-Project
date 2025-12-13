using Photon.Pun;
using System;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviourPunCallbacks
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private UnitScript _unitScript;
    private Vector2 _lastFacingDir = Vector2.down;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _unitScript = GetComponent<UnitScript>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private string DetermineAnimPrefix()
    {
        string className = _unitScript.GetCharacterClass.name;
        string animPrefix = "";

        if (className.Contains("Dwarf"))
            animPrefix = "Dwarf";
        else if (className.Contains("Duelist"))
            animPrefix = "Witch";
        else if (className.Contains("Plague"))
            animPrefix = "Doc";

        return animPrefix;
    }
    private string DetermineAnimSuffix()
    {
        string animSuffix = "";

        if (_lastFacingDir == Vector2.up)
            animSuffix = "Back";
        else if (_lastFacingDir == Vector2.right)
            animSuffix = "Right";
        else if (_lastFacingDir == Vector2.down)
            animSuffix = "Front";
        else if (_lastFacingDir == Vector2.left)
            animSuffix = "Left";
        return animSuffix;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float absHori = Mathf.Abs(horizontal);
        float absVerti = Mathf.Abs(vertical);

        if (horizontal >= 0 && absHori >= absVerti)
            _lastFacingDir = Vector2.right;
        else if (horizontal < 0 && absHori >= absVerti)
            _lastFacingDir = Vector2.left;

        if (vertical >= 0 && absHori < absVerti)
            _lastFacingDir = Vector2.up;
        else if (vertical < 0 && absHori > absVerti)
            _lastFacingDir = Vector2.down;


        if ((horizontal != 0f || vertical != 0f))
            PlayAnimation("RunRight");

        if (horizontal < 0)
            _spriteRenderer.flipX = true;
        else
            _spriteRenderer.flipX = false;
    }
    public void PlayAnimation(string animStateActionName)
    {
       // _animator.Play(_animPrefix + animStateActionName);
    }
}
