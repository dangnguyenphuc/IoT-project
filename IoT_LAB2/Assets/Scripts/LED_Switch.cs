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
    public class LED_Switch : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private bool _isOn = false;
        public bool isOn
        {
            get
            {
                return _isOn;
            }
        }

        [SerializeField]
        private RectTransform toggleIndicator;
        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Color onColor;
        [SerializeField]
        private Color offColor;
        private float offX;
        private float onX;
        private float tweenTime = 0.25f;

        public delegate void ValueChanged(bool value);
        public event ValueChanged valueChanged;


        void Awake()
        {
            offX = -50;
            onX = 50;
        }

        private void OnEnable()
        {
            Toggle(isOn);
        }

        public void Toggle(bool value)
        {
            if(value != isOn)
            {
                _isOn = value;

                ToggleColor(isOn);
                MoveIndicator(isOn);

                if(valueChanged != null)
                {
                    valueChanged(isOn);
                }
            }
            
            
        }

        private void ToggleColor(bool value)
        {
            if (value)
                backgroundImage.DOColor(onColor, tweenTime);
            else backgroundImage.DOColor(offColor, tweenTime);
        }

        private void MoveIndicator(bool value)
        {
           
            if (value)
                toggleIndicator.DOAnchorPosX(onX, tweenTime);
            else
                toggleIndicator.DOAnchorPosX(offX, tweenTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Toggle(!isOn);
            GameObject manager = GameObject.Find("Manager");
            if (backgroundImage.transform.parent.name == "LED")
            {
                manager.GetComponent<Recv_mess>().PublishLED();
            }
            if (backgroundImage.transform.parent.name == "PUMP")
            {
                manager.GetComponent<Recv_mess>().PublishPUMP();
            }
            
        }
    }
}
