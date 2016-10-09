using UnityEngine;
using TESUnity.ESM;
using System.Text.RegularExpressions;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class BookComponent : ObjectComponent
    {
        private static PlayerComponent _player = null;
        private GameObject _container = null;

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
            var targetText = Regex.Replace(BOOK.TEXT.value, @"<[^>]*>", string.Empty);

            Debug.Log(BOOK.TEXT.value);
            Debug.Log(BOOK.TEXT.value.Length);
            var tes = TESUnity.instance;
            var scrollTexture = tes.Engine.textureManager.LoadTexture("scroll");//tx_menubook

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

            Player.Pause(true);
        }
    }
}