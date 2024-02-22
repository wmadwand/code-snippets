using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutorialActions
{

    public class ShopSpellInfoPopup : ITutorialAction
    {
        public SpellSlotItemLocator Locator;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            var item = Locator.GetSlotItem();
            if (item)
            {
                item.OnInfo();
            }
            yield break;
        }
    }

}
