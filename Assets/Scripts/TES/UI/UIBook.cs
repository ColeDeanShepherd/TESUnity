﻿using System;
using System.Text;
using TESUnity.ESM;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    public class UIBook : MonoBehaviour
    {
        private int _numberOfPages;
        private int _cursor;
        private string[] _pages;
        private BOOKRecord _bookRecord;

        [SerializeField]
        private GameObject _container = null;
        [SerializeField]
        private Image _background = null;
        [SerializeField]
        private Text _page1 = null;
        [SerializeField]
        private Text _page2 = null;
        [SerializeField]
        private Button _nextButton = null;
        [SerializeField]
        private Button _previousButton = null;

        public event Action<BOOKRecord> OnTake = null;
        public event Action<BOOKRecord> OnClosed = null;

        void Start()
        {
            var texture = TESUnity.instance.Engine.textureManager.LoadTexture("tx_menubook");
            _background.sprite = GUIUtils.CreateSprite(texture);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            Close();
        }

        public void Show(BOOKRecord book)
        {
            _bookRecord = book;

            var words = _bookRecord.TEXT.value;
            words = words.Replace("<BR>", "\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);

            var countChar = words.Length;
            var numCharPerPage = 700;
            var j = 0;

            // Ceil returns the bad value... 16.6 returns 16..
            _numberOfPages = Mathf.CeilToInt(countChar / numCharPerPage) + 1;
            _pages = new string[_numberOfPages];



            for (int i = 0; i < countChar; i++)
            {
                if (i % numCharPerPage == 0 && i > 0)
                    j++;

                if (_pages[j] == null)
                    _pages[j] = String.Empty;

                _pages[j] += words[i];
            }

            _cursor = 0;

            UpdateBook();

            gameObject.SetActive(true);
        }

        private void UpdateBook()
        {
            if (_numberOfPages > 1)
            {
                _page1.text = _pages[_cursor];
                _page2.text = _cursor + 1 >= _numberOfPages ? "" : _pages[_cursor + 1];
            }
            else
            {
                _page1.text = _pages[0];
                _page2.text = string.Empty;
            }

            _nextButton.interactable = _cursor + 2 < _numberOfPages;
            _previousButton.interactable = _cursor - 2 >= 0;

            if (_cursor + 2 < _numberOfPages && _pages[_cursor + 2] == string.Empty)
                _nextButton.interactable = false;
        }

        public void Take()
        {
            if (OnTake != null)
                OnTake(_bookRecord);

            Close();
        }

        public void Next()
        {
            if (_cursor + 2 >= _numberOfPages)
                return;

            if (_pages[_cursor + 2] == string.Empty)
                return;

            _cursor += 2;

            UpdateBook();
        }

        public void Previous()
        {
            if (_cursor - 2 < 0)
                return;

            _cursor -= 2;

            UpdateBook();
        }

        public void Close()
        {
            _container.SetActive(false);

            if (OnClosed != null)
                OnClosed(_bookRecord);
        }

        public static UIBook Create(GameObject parent)
        {
            var uiBookAsset = Resources.Load<GameObject>("UI/Book");
            var uiBookGO = (GameObject)GameObject.Instantiate(uiBookAsset, parent.transform);
            return uiBookGO.GetComponent<UIBook>();
        }
    }
}
