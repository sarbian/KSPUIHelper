using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI.Extensions.Tweens;

namespace UnityEngine.UI.Extensions
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform)), RequireComponent(typeof(LayoutElement))]
	public class UICollapsible : MonoBehaviour {
		
		public enum Transition
		{
			Instant,
			Tween
		}
		
		public enum State
		{
			Collapsed,
			Expanded
		}
		
		[SerializeField] private float m_MinHeight = 18f;
		[SerializeField] private Transition m_Transition = Transition.Tween;
		[SerializeField] private float m_TransitionDuration = 0.3f;
		[SerializeField] private State m_CurrentState = State.Expanded;
		
		/// <summary>
		/// Gets or sets the transition.
		/// </summary>
		/// <value>The transition.</value>
		public Transition transition
		{
			get { return this.m_Transition; }
			set { this.m_Transition = value; }
		}
		
		/// <summary>
		/// Gets or sets the duration of the transition.
		/// </summary>
		/// <value>The duration of the transition.</value>
		public float transitionDuration
		{
			get { return this.m_TransitionDuration; }
			set { this.m_TransitionDuration = value; }
		}
		
		private RectTransform m_RectTransform;
		private Toggle m_Toggle;
		private LayoutElement m_LayoutElement;
		
		[NonSerialized]
		private readonly TweenRunner<FloatTween> m_FloatTweenRunner;
		
		protected UICollapsible()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected virtual void Awake()
		{
			this.m_RectTransform = this.transform as RectTransform;
			this.m_LayoutElement = this.gameObject.GetComponent<LayoutElement>();
			this.m_Toggle = this.gameObject.GetComponent<Toggle>();
			
			if (this.m_Toggle != null)
			{
				this.m_Toggle.onValueChanged.AddListener(OnValueChanged);
			}
		}
		
		protected virtual void OnValidate()
		{
			LayoutElement le = this.gameObject.GetComponent<LayoutElement>();

			if (le != null)
			{
				le.preferredHeight = (this.m_CurrentState == State.Expanded) ? -1f : this.m_MinHeight;
			}
		}
		
		public void OnValueChanged(bool state)
		{
			if (!this.enabled || !this.gameObject.activeInHierarchy)
				return;
			
			this.TransitionToState(state ? State.Expanded : State.Collapsed);
		}
		
		public void TransitionToState(State state)
		{
			if (this.m_LayoutElement == null)
				return;
			
			// Save as current state
			this.m_CurrentState = state;
			
			// Transition
			if (this.m_Transition == Transition.Instant)
			{
				this.m_LayoutElement.preferredHeight = (state == State.Expanded) ? -1f : this.m_MinHeight;
			}
			else if (this.m_Transition == Transition.Tween)
			{
				if (state == State.Expanded)
				{
					this.StartTween(this.m_LayoutElement.preferredHeight, this.GetExpandedHeight());
				}
				else
				{
					this.StartTween((this.m_LayoutElement.preferredHeight == -1f) ? this.m_RectTransform.rect.height : this.m_LayoutElement.preferredHeight, this.m_MinHeight);
				}
			}
		}
		
		protected float GetExpandedHeight()
		{
			if (this.m_LayoutElement == null)
				return this.m_MinHeight;
			
			float originalPrefH = this.m_LayoutElement.preferredHeight;
			this.m_LayoutElement.preferredHeight = -1f;
			float h = LayoutUtility.GetPreferredHeight(this.m_RectTransform);
			this.m_LayoutElement.preferredHeight = originalPrefH;
			
			return h;
		}
		
		protected void StartTween(float startFloat, float targetFloat)
		{
			FloatTween info = new FloatTween
			{
				duration = this.m_TransitionDuration,
				startFloat = startFloat,
				targetFloat = targetFloat
			};
			info.AddOnChangedCallback(SetHeight);
			info.ignoreTimeScale = true;
			this.m_FloatTweenRunner.StartTween(info);
		}
		
		protected void SetHeight(float height)
		{
			if (this.m_LayoutElement == null)
				return;
			
			this.m_LayoutElement.preferredHeight = height;
		}
	}
}