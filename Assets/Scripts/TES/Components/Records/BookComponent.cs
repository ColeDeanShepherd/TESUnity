using TESUnity.ESM;
using TESUnity.UI;

namespace TESUnity.Components.Records
{
    public class BookComponent : GenericObjectComponent
    {
        private static PlayerComponent _player = null;
        private static UIManager _uiManager = null;

        public static PlayerComponent Player
        {
            get
            {
                if (_player == null)
                    _player = FindObjectOfType<PlayerComponent>();

                return _player;
            }
        }

        public static UIManager UIManager
        {
            get
            {
                if (_uiManager == null)
                    _uiManager = FindObjectOfType<UIManager>();

                return _uiManager;
            }
        }

        void Start()
        {
            usable = true;
            pickable = false;

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

            if (BOOK.TEXT == null)
            {
                if (BOOK.BKDT.scroll == 1)
                    OnTakeScroll(BOOK);
                else
                    OnTakeBook(BOOK);
            }

            if (BOOK.BKDT.scroll == 1)
            {
                UIManager.Scroll.Show(BOOK);
                UIManager.Scroll.OnClosed += OnCloseScroll;
                UIManager.Scroll.OnTake += OnTakeScroll;
            }
            else
            {
                UIManager.Book.Show(BOOK);
                UIManager.Book.OnClosed += OnCloseBook;
                UIManager.Book.OnTake += OnTakeBook;
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
            UIManager.Scroll.OnClosed -= OnCloseScroll;
            UIManager.Scroll.OnTake -= OnTakeScroll;
            Player.Pause(false);
        }

        private void OnTakeBook(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.Add(this);
        }

        private void OnCloseBook(BOOKRecord obj)
        {
            UIManager.Book.OnClosed -= OnCloseBook;
            UIManager.Book.OnTake -= OnTakeBook;
            Player.Pause(false);
        }
    }
}