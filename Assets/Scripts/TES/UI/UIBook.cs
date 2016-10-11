using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    public class UIBook : MonoBehaviour
    {
        private static Sprite BookSprite;
        private int _numberOfPages;
        private int _currentPage;
        private string[] _pages;

        [SerializeField]
        private GameObject _container = null;
        [SerializeField]
        private Image _background = null;
        [SerializeField]
        private Text _content1 = null;
        [SerializeField]
        private Text _content2 = null;
        [SerializeField]
        private Text _nextButton = null;
        [SerializeField]
        private Text _previousButton = null;

        void Start()
        {
            if (BookSprite == null)
            {
                var texture = TESUnity.instance.Engine.textureManager.LoadTexture("tx_menubook");
                BookSprite = GUIUtils.CreateSprite(texture);
            }

            _background.sprite = BookSprite;
        }

        public void Show(string pages)
        {
            // 1. Replace <BR> by jump
            // 2. Remove HTML tags
            // 3. Determine the number of pages
            // 4. Display the first and second page
            // 5. Update buttons.
            
            pages = pages.Replace("<BR>", "\r\n");
            pages = System.Text.RegularExpressions.Regex.Replace(pages, @"<[^>]*>", string.Empty);

            var countChar = pages.Length;
            var numCharPerPage = 350;
            _numberOfPages = Mathf.CeilToInt(countChar / numCharPerPage);
            _currentPage = 0;
            _pages = new string[_numberOfPages];

        }

        public void Take()
        {
            Debug.Log("Take");
        }

        public void Next()
        {
            Debug.Log("Next");
        }

        public void Previous()
        {
            Debug.Log("Previous");
        }

        public void Close()
        {
            Debug.Log("Close");
        }
    }
}
