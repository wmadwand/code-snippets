using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meta.UI.MainMenu
{
    public class EventChestsPanel : BehaviourWithBattleSimulator, IUpdatableView
    {
        [SerializeField] private ChestRewardsScreen ChestRewardsPrefab;
        [SerializeField] private EventChestItem[] Items;

        private List<PlayerResources.EventChest> AllChests => Game.PlayerProfile.Resources.EventChests;

        public bool NoUpdateRequired => false;

        protected override void Awake()
        {
            base.Awake();

            foreach (var item in Items)
            {
                item.TryOpen = OnChestTryOpen;
            }
            UpdateState();
        }

        //TODO: Move UpdateState from here
        private void Update()
        {
            UpdateState();
        }

        private void OnChestTryOpen(EventChestItem item)
        {
            var chestsCount = AllChests.Sum(c => c.Count);
            Game.Services.Resources.OpenEventChest(item.CurrentChest.Rarity, rewards =>
            {
                if (rewards?.Count > 0)
                {
                    if (item.CurrentChest.Count <= 0)
                    {
                        item.gameObject.SetActive(false);
                    }
                    var ui = Instantiate(ChestRewardsPrefab, transform.parent);
                    ui.Init(item.CurrentChest, rewards);
                    if (chestsCount == 1)
                    {
                        //OnClose();
                    }
                }

            });
        }

        public void UpdateState()
        {
            var allKeys = Game.PlayerProfile.Resources.EventChestKeys;

            foreach (var item in Items)
            {
                var evChest = AllChests.FirstOrDefault(c => c.Rarity == item.GetRarity()
                              && c.Chests.FirstOrDefault(cc => Game.PlayerProfile.Progress.LevelIndex + 1 >= cc.L) != null);

                var isEnoughKeysForOpen = false;
                if (evChest != null)
                {
                    allKeys -= Game.Config.Meta.GetEventChestOpeningPrice(evChest.Rarity);
                    isEnoughKeysForOpen = allKeys >= 0;
                }

                item.UpdateState(evChest, isEnoughKeysForOpen);
            }
        }
    }
}