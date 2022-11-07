using DG.Tweening;
using MiniGames.Games.Bowman;
using Coconut.Core.Asyncs;
using Coconut.Game;
using UnityEngine;
using Utils;

public class BowmanExerciseFx : MonoBehaviour
{
    [SerializeField] private string _noneState = "None";
    [SerializeField] private string _fadedState = "Faded";
    [SerializeField] private string _introTrigger = "Intro";
    [SerializeField] private string _correctTrigger = "Correct";
    [SerializeField] private string _incorrectTrigger = "Incorrect";

    [SerializeField] private Transform _flagRoot;
    [SerializeField] private Transform _flagSceneRoot;
    [SerializeField] private Transform _flagUIRoot;
    [SerializeField] private Transform _equalSignRoot;
    [SerializeField] private Transform _targetRoot;
    [SerializeField] private SplashTarget _targetSplash;

    [Header("Audio")]
    [SerializeField] private ShootSoundOnEvent _backSound;

    private Animator _animator;
    private Target _incorretTarget;

    //---------------------------------------------------

    public async Promise ShowTutorial()
    {
        Debug.Log("Show tutorial");
    }

    public async Promise Intro(ExerciseFlag flag)
    {
        _targetSplash.SetNumber("?");
        _ = flag.Transition(_flagUIRoot, false);
        _animator.SetTrigger(_introTrigger);
        await _animator.WaitStateAsync(_noneState);
        _backSound.Play();
        await flag.Transition(_flagSceneRoot, true);
    }

    public async Promise Incorrect(ExerciseFlag flag, Target target)
    {
        _incorretTarget = target;

        _targetSplash.SetNumber(target.Number.ToString());
        _ = flag.Transition(_flagUIRoot, false);
        _animator.SetTrigger(_incorrectTrigger);
        await _animator.WaitStateAsync(_noneState);
        _backSound.Play();
        await flag.Transition(_flagSceneRoot, true);

        _incorretTarget = null;
    }

    public async Promise Correct(ExerciseFlag flag, Target target)
    {
        _targetSplash.SetNumber(target.Number.ToString());
        _ = flag.Transition(_flagUIRoot, false);
        _animator.SetTrigger(_correctTrigger);
        await _animator.WaitStateAsync(_fadedState);
    }

    public void OnFireLightning()
    {
        if (!_incorretTarget) { return; }

        _incorretTarget.DestroyProjectile();
        _incorretTarget.ResetView();
    }

    //---------------------------------------------------

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}