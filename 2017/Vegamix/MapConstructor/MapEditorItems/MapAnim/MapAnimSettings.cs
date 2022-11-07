using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FTRuntime;
using System;
using System.Linq;

namespace MapConstructor
{
    [RequireComponent(typeof(SwfClipController))]
    public class MapAnimSettings : MonoBehaviour
    {
        #region Properties
        [SerializeField]
        MapAnimEditorData animData;
        public MapAnimEditorData AnimData
        { get { return animData; } }

        private GameObject[] animObjectsExtra;

        private bool isAnimsReady;
        private bool isAnimOnClickPlaying;
        private int animIndex = 0;

        private SwfClipController swfClipMain;
        #endregion

        #region Basic methods
        private void Awake()
        {
            swfClipMain = this.GetComponent<SwfClipController>();
            AnimObjectOnClick.AnimObjectOnClickEvent += OnMouseClick;
        }

        private void OnDestroy()
        {
            AnimObjectOnClick.AnimObjectOnClickEvent -= OnMouseClick;
        }

        private void Start()
        {
            animObjectsExtra = new GameObject[AnimData.SubAnims.Count];

            StartCoroutine(CreateSubAnims(() =>
            {
                foreach (GameObject go in animObjectsExtra)
                {
                    go.transform.SetParent(gameObject.transform);
                    go.transform.SetAsLastSibling();
                    go.transform.localPosition = Vector2.zero;

                    go.GetComponent<BoxCollider2D>().enabled = false;
                    //go.GetComponent<AnimObjectOnClick>().enabled = false;
                }
                isAnimsReady = true;
                swfClipMain.loopMode = SwfClipController.LoopModes.Loop;
                swfClipMain.Play(true);
            }));
        }
        #endregion

        #region Anim init
        protected IEnumerator CreateSubAnims(Action onComplete)
        {
            for (int i = 0; i < AnimData.SubAnims.Count; i++)
            {
                GameObject go = GameObject.Instantiate(AnimData.SubAnims[i].GO);
                animObjectsExtra[i] = go;

                MeshFilter meshF = go.GetComponent<MeshFilter>();
                yield return new WaitWhile(() => meshF == null);
                go.SetActive(false);
            }

            onComplete();
        } 
        #endregion

        #region Anim control
        private void OnMouseClick(GameObject go)
        {
            if (AnimData.SubAnims != null && isAnimsReady && !isAnimOnClickPlaying)
            {
                isAnimOnClickPlaying = true;
                StartCoroutine(PlayAnim(() => isAnimOnClickPlaying = false));
            }
        }

        IEnumerator PlayAnim(Action callback)
        {
            yield return new WaitUntil(() => swfClipMain.clip.currentFrame == 0 /*&& swfClipCurr.clip.currentFrame + 1 >= swfClipCurr.clip.frameCount*/);

            SwfClipController swfClipCurr = animObjectsExtra[animIndex].GetComponent<SwfClipController>();
            ChangeAnim(swfClipMain, swfClipCurr);

            yield return new WaitUntil(() => swfClipCurr.clip.currentFrame != 0 && swfClipCurr.clip.currentFrame + 1 >= swfClipCurr.clip.frameCount);

            if (callback != null)
            {
                ChangeAnim(swfClipCurr, swfClipMain);
                animIndex = animIndex < animObjectsExtra.Length - 1 ? ++animIndex : 0;
                callback();
            }
        }

        private void ChangeAnim(SwfClipController clip01, SwfClipController clip02)
        {
            if (clip01 == swfClipMain)
            {
                clip01.GetComponent<MeshRenderer>().enabled = false;
                clip02.gameObject.SetActive(true);
            }
            else if (clip02 == swfClipMain)
            {
                clip02.GetComponent<MeshRenderer>().enabled = true;
                clip01.gameObject.SetActive(false);
            }

            clip01.Stop(true);
            clip02.Play(true);
        } 
        #endregion
    }
}
