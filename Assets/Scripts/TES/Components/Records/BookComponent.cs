using UnityEngine;
using TESUnity.ESM;
using TESUnity.UI;

namespace TESUnity.Components.Records
{
    public class BookComponent : GenericObjectComponent
    {
        private static PlayerComponent _player = null;
        private static UIBook _uiBook = null;
        private static UIScroll _uiScroll = null;

        public static PlayerComponent Player
        {
            get
            {
                if (_player == null)
                    _player = FindObjectOfType<PlayerComponent>();

                return _player;
            }
        }

        void Start()
        {
            usable = true;
            pickable = false;

            if (_uiBook == null)
            {
                var uiTransform = GUIUtils.MainCanvas.GetComponent<Transform>();
                _uiBook = UIBook.Create(uiTransform);
                _uiScroll = UIScroll.Create(uiTransform);
            }

            var BOOK = (BOOKRecord)record;
            objData.interactionPrefix = "Read ";
            objData.name = BOOK.FNAM != null ? BOOK.FNAM.value : BOOK.NAME.value;

            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(BOOK.ITEX.value, "icons");
            objData.weight = BOOK.BKDT.weight.ToString();
            objData.value = BOOK.BKDT.value.ToString();
        }

        public override void Interact()
        {
            var BOOK = (BOOKRecord)record;

            if (BOOK.BKDT.scroll == 1)
            {
                _uiScroll.Show(BOOK);
                _uiScroll.OnClosed += OnCloseScroll;
                _uiScroll.OnTake += OnTakeScroll;
            }
            else
            {
                _uiBook.Show(BOOK);
                _uiBook.OnClosed += OnCloseBook;
                _uiBook.OnTake += OnTakeBook;
            }

            Player.Pause(true);
        }

        private void OnTakeScroll(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.Add(this);
        }

        private void OnCloseScroll(BOOKRecord obj)
        {
            _uiScroll.OnClosed -= OnCloseScroll;
            _uiScroll.OnTake -= OnTakeScroll;
            Player.Pause(false);
        }

        private void OnTakeBook(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.Add(this);
        }

        private void OnCloseBook(BOOKRecord obj)
        {
            _uiBook.OnClosed -= OnCloseBook;
            _uiBook.OnTake -= OnTakeBook;
            Player.Pause(false);
        }
    }
}