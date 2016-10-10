using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    public class UIBook : MonoBehaviour
    {
        private static Sprite BookSprite;

        [SerializeField]
        private GameObject _container = null;
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

            _container.GetComponent<Image>().sprite =BookSprite;
        }

        public void Show(string[] pages)
        {

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
