using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}

		public float MovementRange = 100f;
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Horizontal"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Vertical"; // The name given to the vertical axis for the cross platform input

		public RectTransform ArrowParent;

		public RectTransform GetWidth;

		Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

		void OnEnable()
		{
			CreateVirtualAxes();
		}

        void Start()
        {
			MovementRange=(GetWidth.rect.width/2)*.75f;
            m_StartPos = transform.position;
        }

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = m_StartPos - value;
			delta.y = -delta.y;
			delta /= MovementRange;
			//Debug.Log("UVA V3 = "+ value);
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-delta.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(delta.y);
			}
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

			// create new axes based on axes to use
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}

        private Vector3 newPos;

        public Vector3 GetTouchInputManual()
        {
            return newPos;
        }

        public void OnDrag(PointerEventData data)
		{
			newPos = Vector3.zero;

			if (m_UseX)
			{
				float delta = (float)(data.position.x - m_StartPos.x);
				//delta = Mathf.Clamp(delta, - MovementRange, MovementRange);
				//Debug.Log("Mathf.Clamp(delta, - MovementRange, MovementRange) "+Mathf.Clamp(delta, - MovementRange, MovementRange));
				newPos.x = delta;
			}

			if (m_UseY)
			{
				float delta = (float)(data.position.y - m_StartPos.y);
				//delta = Mathf.Clamp(delta, -MovementRange, MovementRange);
				newPos.y = delta;
			}
			//Debug.Log("NewPos Mag is "+newPos.magnitude);
			newPos=Vector3.ClampMagnitude(newPos,MovementRange);
			//Debug.Log("NewPos Mag is "+newPos.magnitude);
			transform.position = new Vector3(m_StartPos.x + newPos.x, m_StartPos.y + newPos.y, m_StartPos.z + newPos.z);
			UpdateVirtualAxes(transform.position);

			//Make the arrow on the thumb stick rotate with direction of thumb stick
			Vector2 fromVector2 = new Vector2(0, 1);
			Vector2 toVector2 = new Vector2(-newPos.x,newPos.y+1);
			float ang = Vector2.Angle(fromVector2, toVector2);
			Vector3 cross = Vector3.Cross(fromVector2, toVector2);
			if (cross.z > 0)
				ang = 360 - ang;
			ArrowParent.rotation=Quaternion.Euler(0,0,ang);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
            newPos = Vector3.zero;
        }


		public void OnPointerDown(PointerEventData data) {
            OnDrag(data);
        }

		void OnDisable()
		{
			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}
	}
}