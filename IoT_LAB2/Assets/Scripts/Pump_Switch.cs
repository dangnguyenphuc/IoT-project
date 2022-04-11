using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using UnityEngine.EventSystems;

namespace my_mqtt
{
    public class Pump_Switch : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private bool _isOn_pump = false;
        public bool isOn_pump
        {
            get
            {
                return _isOn_pump;
            }
        }

        [SerializeField]
        private RectTransform toggleIndicator_pump;
        [SerializeField]
        private Image backgroundImage_pump;

        [SerializeField]
        private Color onColor_pump;
        [SerializeField]
        private Color offColor_pump;
        private float offX_pump;
        private float onX_pump;
        private float tweenTime = 0.25f;

        public delegate void ValueChanged(bool value);
        public event ValueChanged valueChanged;


        void Awake()
        {
            offX_pump = -50;
            onX_pump = 50;
        }

        private void OnEnable()
        {
            Toggle_pump(isOn_pump);
        }

        public void Toggle_pump(bool value)
        {
            if (value != isOn_pump)
            {
                _isOn_pump = value;

                ToggleColor_pump(isOn_pump);
                MoveIndicator_pump(isOn_pump);

                if (valueChanged != null)
                {
                    valueChanged(isOn_pump);
                }
            }


        }

        private void ToggleColor_pump(bool value)
        {
            if (value)
                backgroundImage_pump.DOColor(onColor_pump, tweenTime);
            else backgroundImage_pump.DOColor(offColor_pump, tweenTime);
        }

        private void MoveIndicator_pump(bool value)
        {

            if (value)
                toggleIndicator_pump.DOAnchorPosX(onX_pump, tweenTime);
            else
                toggleIndicator_pump.DOAnchorPosX(offX_pump, tweenTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Toggle_pump(!isOn_pump);
            GameObject manager = GameObject.Find("Manager");
            manager.GetComponent<Recv_mess>().PublishPUMP();
        }
    }
}
