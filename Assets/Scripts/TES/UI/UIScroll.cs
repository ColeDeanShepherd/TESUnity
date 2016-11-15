using System;
using TESUnity.ESM;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    public class UIScroll : MonoBehaviour
    {
        private BOOKRecord _bookRecord;

        [SerializeField]
        private GameObject _container = null;
        [SerializeField]
        private Image _background = null;
        [SerializeField]
        private Text _content = null;

        public event Action<BOOKRecord> OnTake = null;
        public event Action<BOOKRecord> OnClosed = null;

        void Start()
        {
            var texture = TESUnity.instance.TextureManager.LoadTexture("scroll", true);
            _background.sprite = GUIUtils.CreateSprite(texture);

            // If the book is already opened, don't change its transform.
            if (_bookRecord == null)
                Close();
        }

        void Update()
        {
            if (_bookRecord == null)
                return;

            if (Input.GetButtonDown("Use"))
                Take();
            else if (Input.GetButton("Menu"))
                Close();
        }

        public void Show(BOOKRecord book)
        {
            _bookRecord = book;

            var words = _bookRecord.TEXT.value;
            words = words.Replace("\r\n", "");
            words = words.Replace("<BR><BR>", "");
            words = words.Replace("<BR>", "\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);

            _content.text = words;

            _container.SetActive(true);
        }

        public void Take()
        {
            if (OnTake != null)
                OnTake(_bookRecord);

            Close();
        }

        public void Close()
        {
            _container.SetActive(false);
            _bookRecord = null;

            if (OnClosed != null)
                OnClosed(_bookRecord);
        }
    }
}
