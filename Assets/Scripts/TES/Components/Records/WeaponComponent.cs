using System.Collections;
using TESUnity.ESM;
using UnityEngine;

namespace TESUnity.Components.Records
{
    public class WeaponComponent : GenericObjectComponent
    {
        private bool _isEquiped = false;
        private bool _isVisible = true;
        private bool _animating = false;
        private Transform _hand = null;
        private Renderer[] _renderers = null;

        void Start()
        {
            var WEAP = (WEAPRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = WEAP.FNAM.value;
            objData.weight = WEAP.WPDT.weight.ToString();
            objData.value = WEAP.WPDT.value.ToString();
            objData.interactionPrefix = "Take ";

            _renderers = GetComponentsInChildren<Renderer>();

            var colliders = GetComponents<MeshCollider>();
            for (int i = 0; i < colliders.Length; i++)
                Destroy(colliders[i]);
        }

        void Update()
        {
            if (_isEquiped)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (_isVisible)
                        PlayAttackAnimation();
                    else
                        SetVisible(true);
                }
            }
        }

        public void SetVisible(bool visible)
        {
            if (visible == _isVisible)
                return;

            for (int i = 0; i < _renderers.Length; i++)
                _renderers[i].enabled = visible;

            _isVisible = visible;
        }

        public void Equip(Transform hand)
        {
            m_transform.parent = hand;
            m_transform.localPosition = Vector3.zero;
            m_transform.localRotation = Quaternion.identity;
            _hand = hand;
            _isEquiped = true;
        }

        public void Unequip(Transform disabledObjects)
        {
            m_transform.parent = disabledObjects;
            _isEquiped = false;
            _hand = null;
        }

        public void PlayAttackAnimation()
        {
            if (!_animating)
                StartCoroutine(PlayAttackAnimationCoroutine());
        }

        private IEnumerator PlayAttackAnimationCoroutine()
        {
            _animating = true;

            var originalRotation = _hand.localRotation;
            var target = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            var time = 0.25f;
            var elapsed = 0.0f;
            var endOfFrame = new WaitForEndOfFrame();

            while (elapsed < time)
            {
                _hand.localRotation = Quaternion.Slerp(_hand.localRotation, target, elapsed / time);
                elapsed += Time.deltaTime;
                yield return endOfFrame;
            }

            time = 0.4f;
            elapsed = 0.0f;

            while (elapsed < time)
            {
                _hand.localRotation = Quaternion.Slerp(_hand.localRotation, originalRotation, elapsed / time);
                elapsed += Time.deltaTime;
                yield return endOfFrame;
            }

            _animating = false;
        }
    }
}
