using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

namespace my_mqtt
{

    public class Status_Data
    {
        public string temp { get; set; }
        public string humid { get; set; }
    }

    public class pump_data
    {
        public string device { get; set; }
        public string status { get; set; }
    }

    public class led_data
    {
        public string device { get; set; }
        public string status { get; set; }
    }

    public class Recv_mess : M2MqttUnityClient
    {
        [SerializeField]
        private List<string> topics = new List<string>();


        //LAYER1:
        public InputField address;
        public InputField username;
        public InputField pw;
        public Text _error;


        public string msg_received_from_topic_status = "";
        public string msg_received_from_topic_LED = "";
        public string msg_received_from_topic_PUMP = "";

        private List<string> eventMessages = new List<string>();
        [SerializeField]
        public Status_Data _status_data;
        [SerializeField]
        public pump_data _pump_data;
        [SerializeField]
        public led_data _led_data;


        IEnumerator ShowMessage(string message)
        {
            _error.text = message;
            _error.enabled = true;
            yield return new WaitForSeconds(3);
            _error.enabled = false;
        }

        public void Button_Connect()
        {
            if (address.text != "mqttserver.tk" || pw.text != "12345678" || username.text != "bkiot")
            {
                StartCoroutine(ShowMessage("Please check again your broker of the server, username and password!"));
                //_error.text = "Please check again your broker of the server, username and password!";
            }
            else
            {
                Manager my_manager = GameObject.Find("Manager").GetComponent<Manager>();
                this.brokerAddress = address.text;
                this.mqttUserName = username.text;
                this.mqttPassword = pw.text;
                this.Connect();
                my_manager.SwitchLayer();
            }
        }

        public void PublishPUMP()
        {
            _pump_data = new pump_data();
            _pump_data = GetComponent<Manager>().Update_Pump(_pump_data);
            string msg_config = JsonConvert.SerializeObject(_pump_data);
            client.Publish(topics[2], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("publish pump");

        }

        public void PublishLED()
        {
            _led_data = new led_data();
            _led_data = GetComponent<Manager>().Update_LED(_led_data);
            string msg_config = JsonConvert.SerializeObject(_led_data);
            client.Publish(topics[1], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("publish led");

        }

        public void PublishStatus()
        {
            System.Random rd = new System.Random();
            _status_data = new Status_Data();
            _status_data.temp = rd.Next(20, 100).ToString();
            _status_data.humid = rd.Next(10, 90).ToString();
            string msg_config = JsonConvert.SerializeObject(_status_data);
            client.Publish(topics[0], System.Text.Encoding.UTF8.GetBytes(msg_config), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("publish status");
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        protected override void OnConnecting()
        {
            base.OnConnecting();
            //SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();

            SubscribeTopics();
        }

        protected override void SubscribeTopics()
        {

            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.Log("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.Log("CONNECTION LOST!");
        }



        protected override void Start()
        {

            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            //StoreMessage(msg);
            if (topic == topics[0])
                ProcessMessageStatus(msg);

            if(topic==topics[1])
                ProcessMessageLed(msg);

            if (topic == topics[2])
                ProcessMessagePump(msg);

        }

        private void ProcessMessageStatus(string msg)
        {
            _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            msg_received_from_topic_status = msg;
            GetComponent<Manager>().Update_Status(_status_data);
        }

        private void ProcessMessagePump(string msg)
        {
            _pump_data = JsonConvert.DeserializeObject<pump_data>(msg);
            msg_received_from_topic_PUMP = msg;
            GetComponent<Manager>().Update_PUMP_UI(_pump_data);

        }
        private void ProcessMessageLed(string msg)
        {
            _led_data = JsonConvert.DeserializeObject<led_data>(msg);
            msg_received_from_topic_LED = msg;
            GetComponent<Manager>().Update_LED_UI(_led_data);

        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            //if (autoTest)
            //{
            //    autoConnect = true;
            //}
        }

        public void UpdateConfig()
        {

        }

        public void UpdateControl()
        {

        }

    }
}
