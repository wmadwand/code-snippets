using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameControllerTest : MonoBehaviour
{
    public DevicesGrid devicesGrid;
    public Transform centerPoint;
    DeviceNode chosenDevice;
    Vector3 prevScale;
    Vector3 prevPos;

    public void ChooseDevice()
    {
        chosenDevice = devicesGrid.GetRandomDevice();

        if (chosenDevice == null)
        {
            return;
        }

        devicesGrid.GetComponent<BGScroller>().Stop(true);

        chosenDevice.SetChosen(true);
        var position = chosenDevice.transform.position;

        chosenDevice.transform.SetParent(null);

        devicesGrid.transform.SetParent(chosenDevice.transform);

        prevScale = chosenDevice.transform.localScale;
        prevPos = chosenDevice.transform.position;

        chosenDevice.transform.DOScale(2, 2);
        chosenDevice.transform.DOMove(centerPoint.position, 2);
    }

    public void GoBack()
    {
        DOTween.Sequence()
            .Append(chosenDevice.transform.DOScale(prevScale, 1))
            .Join(chosenDevice.transform.DOMove(prevPos, 1))
            .AppendCallback(() =>
            {
                devicesGrid.transform.parent = null;
                chosenDevice.transform.SetParent(devicesGrid.transform);
                chosenDevice.SetChosen(false);

                devicesGrid.GetComponent<BGScroller>().Stop(false);
            })
            ;
    }
}
