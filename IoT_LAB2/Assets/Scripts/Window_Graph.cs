using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;

namespace my_mqtt
{
    public class Window_Graph : MonoBehaviour
    {
        [SerializeField]
        private Sprite circleSprite;
        private RectTransform graphContainer;
    private void Awake()
        {
            graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
            //List<int> value_temp = new List<int> { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40 };
            //List<int> value_humid = new List<int> { 3, 12, 15, 25, 10, 76, 45, 76, 23, 35, 59, 89, 93 };
             GameObject manager = GameObject.Find("Manager"); ;
                        for (int i = 0; i< 13; i++)
                {
                    manager.GetComponent<Manager>().humid_list.Add(0);
                    manager.GetComponent<Manager>().temp_list.Add(0);
                }

            ShowGraph(manager.GetComponent<Manager>().temp_list, manager.GetComponent<Manager>().humid_list);
        }

        public GameObject CreateCircle(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("circle", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = circleSprite;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(5, 5);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            return gameObject;
        }

        public void CreateData(Vector2 anchoredPosition, int data)
        {
            GameObject gameObject = new GameObject("data", typeof(Text));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Text>().fontSize = 10;
            gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0.7f);
            gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
            gameObject.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            gameObject.GetComponent<Text>().text = Convert.ToString(data);
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(14, 14);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
        }

        public void ShowGraph(List<int> valueList, List<int> valueList2)
        {
            float xSize = 50f;
            float yMax = 120f;
            float height = graphContainer.sizeDelta.y;

            GameObject lastCircle1 = null;
            GameObject lastCircle2 = null;
            for (int i = 0; i < valueList.Count; i++)
            {
                float xPos = xSize + i * xSize;
                float yPos = (valueList[i] / yMax) * height;            // y of temperature
                float yPos2 = (valueList2[i] / yMax) * height;          // y  of humid

                GameObject circleGO_temp = CreateCircle(new Vector2(xPos, yPos));
                CreateData(new Vector2(xPos, yPos + (2 / yMax) * height), valueList[i]);

                GameObject circleGO_humid = CreateCircle(new Vector2(xPos, yPos2));
                CreateData(new Vector2(xPos, yPos2 - (2 / yMax) * height), valueList2[i]);

                if (lastCircle1 != null)
                {
                    GameObject line = CreateDotConnection(lastCircle1.GetComponent<RectTransform>().anchoredPosition, circleGO_temp.GetComponent<RectTransform>().anchoredPosition);
                    line.GetComponent<Image>().color = new Color(1, 0, 0, 0.7f);
                }
                if (lastCircle2 != null)
                {
                    GameObject line = CreateDotConnection(lastCircle2.GetComponent<RectTransform>().anchoredPosition, circleGO_humid.GetComponent<RectTransform>().anchoredPosition);
                    line.GetComponent<Image>().color = new Color(0, 0, 1, 0.7f);
                }

                lastCircle1 = circleGO_temp;
                lastCircle2 = circleGO_humid;
            }
        }

        public GameObject CreateDotConnection(Vector2 posA, Vector2 posB)
        {
            Vector2 direction = (posB - posA).normalized;
            float distance = Vector2.Distance(posA, posB);

            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.sizeDelta = new Vector2(distance, 1f);
            rect.anchoredPosition = posA + direction * distance * .5f;
            rect.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
            return gameObject;
        }

        public void Removed_Graph()
        {
            GameObject parent = GameObject.Find("Window_Graph");
            foreach(Transform child in parent.transform.Find("graphContainer"))
            {
                if (child.name == "BG") continue;
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}