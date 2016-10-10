using UnityEngine;
using TESUnity.ESM;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class BookComponent : GenericObjectComponent
    {
        private static PlayerComponent _player = null;
        private GameObject _container = null;

        public bool IsScroll
        {
            get { return ((BOOKRecord)record).BKDT.scroll == 1; }
        }

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
            var BOOK = (BOOKRecord)record;
            objData.name = BOOK.FNAM != null ? BOOK.FNAM.value : BOOK.NAME.value;

            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(BOOK.ITEX.value, "icons");
            objData.weight = BOOK.BKDT.weight.ToString();
            objData.value = BOOK.BKDT.value.ToString();
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
                CreateBook(BOOK);

            _container.transform.SetAsLastSibling();

            Player.Pause(true);
        }

        private void CreateScroll(BOOKRecord book)
        {
            var tes = TESUnity.instance;
            var scrollTexture = tes.Engine.textureManager.LoadTexture("scroll");
            var targetText = Regex.Replace(book.TEXT.value, @"<[^>]*>", string.Empty);

            _container = GUIUtils.CreateImage(Sprite.Create(scrollTexture, new Rect(0, 0, scrollTexture.width, scrollTexture.height), Vector2.zero), GUIUtils.MainCanvas);
            var scrollTransform = _container.GetComponent<RectTransform>();
            scrollTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 640);
            scrollTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 480);

            var textGO = GUIUtils.CreateText(targetText, _container);
            textGO.AddComponent<Shadow>();

            var textTransform = textGO.GetComponent<RectTransform>();
            textTransform.anchorMin = Vector3.zero;
            textTransform.anchorMax = Vector3.one;
            textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 540);
            textTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 380);

            var text = textGO.GetComponent<Text>();
            text.color = Color.white;
            text.resizeTextForBestFit = true;
        }

        private void CreateBook(BOOKRecord book)
        {
            var tes = TESUnity.instance;
            var bookTexture = tes.Engine.textureManager.LoadTexture("tx_menubook");
            var targetText = Regex.Replace(book.TEXT.value, @"<[^>]*>", string.Empty);

            _container = GUIUtils.CreateImage(Sprite.Create(bookTexture, new Rect(0, 0, bookTexture.width, bookTexture.height), Vector2.zero), GUIUtils.MainCanvas);

            Debug.Log(book.TEXT.value);
        }
    }
}