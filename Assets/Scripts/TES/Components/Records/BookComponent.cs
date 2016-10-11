using UnityEngine;
using TESUnity.ESM;
using UnityEngine.UI;
using TESUnity.UI;

namespace TESUnity.Components.Records
{
    public class BookComponent : GenericObjectComponent
    {
        private static PlayerComponent _player = null;
        private GameObject _container = null;
        private static UIBook _uiBook = null;

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
                _uiBook = UIBook.Create(GUIUtils.MainCanvas);
             
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
                CreateScroll(BOOK);
            else
            {
                _uiBook.Show(BOOK);
                _uiBook.OnClosed += OnClosed;
                _uiBook.OnTake += OnTake;
            }

            Player.Pause(true);
        }

        // TODO: Create UIScroll and delete that code.
        private void CreateScroll(BOOKRecord book)
        {
            var tes = TESUnity.instance;
            var scrollTexture = tes.Engine.textureManager.LoadTexture("scroll");

            var words = book.TEXT.value;
            words = words.Replace("<BR>", "\r\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);

            _container = GUIUtils.CreateImage(Sprite.Create(scrollTexture, new Rect(0, 0, scrollTexture.width, scrollTexture.height), Vector2.zero), GUIUtils.MainCanvas);
            var scrollTransform = _container.GetComponent<RectTransform>();
            scrollTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 640);
            scrollTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 480);

            var textGO = GUIUtils.CreateText(words, _container);
            textGO.AddComponent<Shadow>();

            var textTransform = textGO.GetComponent<RectTransform>();
            textTransform.anchorMin = Vector3.zero;
            textTransform.anchorMax = Vector3.one;
            textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 540);
            textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380);

            var text = textGO.GetComponent<Text>();
            text.color = Color.white;
            text.resizeTextForBestFit = true;

            _container.transform.SetAsLastSibling();
        }

        private void OnTake(BOOKRecord obj)
        {
            var inventory = FindObjectOfType<PlayerInventory>();
            inventory.Add(obj);
        }

        private void OnClosed(BOOKRecord obj)
        {
            _uiBook.OnClosed -= OnClosed;
            _uiBook.OnTake -= OnTake;
            Player.Pause(false);
        }
    }
}