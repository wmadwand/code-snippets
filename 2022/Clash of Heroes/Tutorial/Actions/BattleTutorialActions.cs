using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TutorialActions
{

    public class StartWave : ITutorialAction
    {
        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            var ui = GameObject.FindObjectOfType<BattlePanelUI>();
            if (ui)
            {
                ui.StartWaveHandler();
            }
            yield break;
        }
    }

    public class SetBattleDeck : ITutorialAction
    {
        [SerializeField] List<Card> Cards;
        [SerializeField] int CardsInHand;
        [SerializeField] DraggableCardSelector DraggableCardSelector;
        [SerializeField] int Seed;
        [SerializeField] bool ActiveBattlePanel = true;

        IEnumerator ITutorialAction.Play(TutorialStep tutorial, TutorialContext context)
        {
            var rnd = new System.Random(Seed);
            context.BattleSimulator.State.Deck.SetCards(Cards.ToList(), CardsInHand, rnd);
            var ui = GameObject.FindObjectOfType<BattlePanelUI>();
            if (ui)
            {
                ui.RefreshDeckCards(DraggableCardSelector);
                ui.ActiveBattlePanel.SetActive(ActiveBattlePanel);
            }
            yield break;
        }
    }

}
