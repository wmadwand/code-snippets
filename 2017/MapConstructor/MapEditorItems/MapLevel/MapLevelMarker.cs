using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using game.map;
using System;
using UnityEngine.UI;

namespace MapConstructor
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class MapLevelMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        #region Properties
        [SerializeField]
        MapLevelType type;

        public MapLevelType Type
        {
            get { return type; }
        }

        [SerializeField]
        Button _button;

        [SerializeField]
        Image _targetGraphic;

        [Header("Sprites")]
        [SerializeField]
        Sprite[] blocked;

        [SerializeField]
        Sprite[] available;

        [SerializeField]
        Sprite[] passed;

        [SerializeField]
        Sprite hidden;

        [SerializeField]
        RectTransform friendsBeaconGroup;
        public Vector2 FriendsBeaconGroup { get { return friendsBeaconGroup != null ? friendsBeaconGroup.anchoredPosition : Vector2.zero; } }

        [SerializeField]
        MapBonusLevelType subType;
        public MapBonusLevelType SubType { get { return subType; } }

        [SerializeField]
        string after;
        public string After { get { return after; } }

        private MapLevel handler;
        #endregion

        public MapLevelMarker() { }

        public void SetHandler(MapLevel _hadler)
        {
            handler = _hadler;
        }

        #region Mouse events
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (handler == null)
            {
                return;
            }

            handler.OnMapLevelEnter();
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            if (handler == null)
            {
                return;
            }

            handler.OnMapLevelExit();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (handler == null)
            {
                return;
            }

            handler.OnMapLevelClick();
        } 
        #endregion

        public void ChangeButtonSprites(MapLevelState state)
        {
            switch (state)
            {
                case MapLevelState.Blocked:
                    {
                        _targetGraphic.sprite = blocked[0];
                        _button.spriteState = new SpriteState { highlightedSprite = blocked[1], pressedSprite = blocked[2] };
                        _button.interactable = false;
                        break;
                    }
                case MapLevelState.Available:
                    {
                        _targetGraphic.sprite = available[0];
                        _button.spriteState = new SpriteState { highlightedSprite = available[1], pressedSprite = available[2] };
                        _button.interactable = true;
                        break;
                    }
                case MapLevelState.Passed:
                    {
                        _targetGraphic.sprite = passed[0];
                        _button.spriteState = new SpriteState { highlightedSprite = passed[1], pressedSprite = passed[2] };
                        _button.interactable = true;
                        break;
                    }
                case MapLevelState.Hidden:
                    {
                        _targetGraphic.sprite = hidden;
                        _button.interactable = false;
                        //_button.spriteState = new SpriteState { highlightedSprite = blocked[1], pressedSprite = blocked[2] };
                        break;
                    }
            }
        }
    }
}