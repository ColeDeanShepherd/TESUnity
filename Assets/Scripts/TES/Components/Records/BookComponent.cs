using UnityEngine;
using TESUnity.ESM;
using TESUnity.UI;

namespace TESUnity.Components.Records
{
    public class BookComponent : GenericObjectComponent
    {
        private static PlayerComponent _player = null;
        private GameObject _container = null;
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
            if (_uiBook == null)
            {
                _uiBook = UIBook.Create(GUIUtils.MainCanvas);
                _uiScroll = UIScroll.Create(GUIUtils.MainCanvas);
            }

            var BOOK = (BOOKRecord)record;
            objData.interactionPrefix = "Read ";
            objData.name = BOOK.FNAM != null ? BOOK.FNAM.value : BOOK.NAME.value;

            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(BOOK.ITEX.value, "icons");
            objData.weight = BOOK.BKDT.weight.ToString();
            objData.value = BOOK.BKDT.value.ToString();
        }

        void Update()
        {
            if (Input.GetButtonDown("Fire1") && _container != null)
            {
                Destroy(_container);
                Player.Pause(false);
                return;
            }
        }

        public override void Interact()
        {
            if (_container != null)
            {
                Destroy(_container);
                Player.Pause(false);
                return;
            }

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