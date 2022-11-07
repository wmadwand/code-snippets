using DG.Tweening;
using Coconut.Core;
using Coconut.Core.Asyncs;
using Coconut.Game;
using System;
using UnityEngine;

public class MathButtonsPanel : MonoBehaviour
{
    public MathButton[] MathButtons => _mathButtons;

    [SerializeField] private MathButton[] _mathButtons;
    [SerializeField] private int _buttonPulseRate = 1;
    [SerializeField] private RangeInteger _buttonPulsePauseRange = new RangeInteger(3, 6);

    //---------------------------------------------------

    public void SetActive(bool value)
    {
        Lock(!value);

        if (value)
        {
            PulseButtonsStart();
        }
        else
        {
            PulseButtonsStop();
        }
    }

    public void ResetView()
    {
        var belowZero = Camera.main.ViewportToWorldPoint(Vector3.zero);
        transform.localPosition = new Vector3(0, belowZero.y + 7, 0);

        gameObject.CancelAllPromises();
        transform.DOKill();
        Array.ForEach(_mathButtons, b => b.Reset());
    }

    public async Promise Show()
    {
        SetActive(false);
        await transform.DOLocalMove(Vector3.zero, .5f).SetUpdate(UpdateType.Normal, true);
        await transform.DOPunchPosition(Vector3.up / 7, 1f, 3).SetUpdate(UpdateType.Normal, true);
    }

    public Tween Hide()
    {
        var belowZero = Camera.main.ViewportToWorldPoint(Vector3.zero);
        var vec = new Vector3(0, belowZero.y + 7, 0);

        return transform.DOLocalMove(vec, 1).SetUpdate(UpdateType.Normal, true);
    }

    public void Lock(bool value)
    {
        Array.ForEach(_mathButtons, btn => btn.SetLock(value));
    }

    public async Promise PulseButtonsAsyncOnce()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var button in _mathButtons)
            {
                await button.Pulse();
            }

            await Timeline.Delay(_buttonPulseRate);
        }
    }

    //---------------------------------------------------

    private async Promise PulseButtonsAsyncLoop()
    {
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var button in _mathButtons)
                {
                    await button.Pulse();
                }

                await Timeline.Delay(_buttonPulseRate);
            }

            await Timeline.Delay(_buttonPulsePauseRange.Random());
        }
    }

    private async Promise PulseButtonsAsync()
    {
        await Timeline.Delay(_buttonPulsePauseRange.Random());
        await PulseButtonsAsyncLoop();
    }

    private void PulseButtonsStart()
    {
        gameObject.Promise(PulseButtonsAsync);
    }

    private void PulseButtonsStop()
    {
        gameObject.CancelPromise(PulseButtonsAsync);
    }
}