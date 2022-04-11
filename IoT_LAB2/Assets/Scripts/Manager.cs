using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace my_mqtt
{
    public class Manager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasLayer1;
        

        [SerializeField]
        private CanvasGroup _canvasLayer2;
        [SerializeField]
        private Text temperature;
        [SerializeField]
        private Text humidity;

        //List to draw curve.
        public List<int> temp_list = new List<int>();
        public List<int> humid_list = new List<int>();

        private Tween twenFade;


        //delete this

        public void Update_Status(Status_Data _status_data)
        {
                        temperature.text = _status_data.temp + "°C";
                        humidity.text = _status_data.humid + "%";
            if (temp_list.Count == 13)
            {
                temp_list.RemoveAt(0);
            }
            if (humid_list.Count == 13)
            {
                humid_list.RemoveAt(0);
            }

            temp_list.Add(int.Parse(_status_data.temp));

            humid_list.Add(int.Parse(_status_data.humid));
            GameObject.Find("Window_Graph").GetComponent<Window_Graph>().Removed_Graph();
            GameObject.Find("Window_Graph").GetComponent<Window_Graph>().ShowGraph(temp_list, humid_list);
        }

        public pump_data Update_Pump(pump_data _pump_data)
        {
            _pump_data.device = "PUMP";
            if (GameObject.Find("PUMP").GetComponent<LED_Switch>().isOn)
            {
                _pump_data.status = "ON";
            }

            else _pump_data.status = "OFF";
 
            return _pump_data;

        }

        public led_data Update_LED(led_data _led_data)
        {
            _led_data.device = "LED";
            if (GameObject.Find("LED").GetComponent<LED_Switch>().isOn)
            {
                _led_data.status = "ON";
            }

            else _led_data.status = "OFF";

            return _led_data;

        }

        public void Update_PUMP_UI(pump_data _pump_data)
        {
            if (_pump_data.status == "ON")
            {
                GameObject.Find("PUMP").GetComponent<LED_Switch>().Toggle(true);
            } 
            else GameObject.Find("PUMP").GetComponent<LED_Switch>().Toggle(false);
        }

        public void Update_LED_UI(led_data _led_data)
        {
            if (_led_data.status == "ON")
            {
                GameObject.Find("LED").GetComponent<LED_Switch>().Toggle(true);
            }
            else GameObject.Find("LED").GetComponent<LED_Switch>().Toggle(false);
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }



        IEnumerator _IESwitchLayer()
        {
            if (_canvasLayer1.interactable == true)
            {
                FadeOut(_canvasLayer1, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayer2, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }
    }
}
