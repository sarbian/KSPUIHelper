using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace KSPUIHelper.ksp_ui_extensions
{
    // Button with support for other than the left button.
    // Yeah, amazing code. Thx Unity for not having this stock
    [AddComponentMenu("UI/ButtonAdv", 30)]
    class ButtonAdv : Selectable, IPointerClickHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent<PointerEventData>
        {
        }

        // Event delegates triggered on click.
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        protected ButtonAdv()
        {
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            m_OnClick.Invoke(eventData);
        }

        // Trigger all registered callbacks.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press(eventData);
        }
    }
}
