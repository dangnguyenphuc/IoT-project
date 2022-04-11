print("Hello ThingsBoard")
import paho.mqtt.client as mqttclient
import json
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.service import Service
import time
import re


class GGMaps:
    def __init__(self,url):
        s = Service('/Users/phucdang/Desktop/Document/iot/Lab1/chromedriver')
        self.driver = webdriver.Chrome(service=s)
        self.driver.get(url)
        time.sleep(3.5)
    def getlatlng(self, cur_url):
        try:
            coords = re.search(r"@?\d{1,3}?\.\d{4,8},?\d{1,3}?\.\d{4,8},16z", cur_url).group()
            coord = coords.split('@')[1]

            lat = float(coord.split(',')[0])

            long = float(coord.split(',')[1])
            return [lat,long]
        except:
            return (-1000,-1000)
    def scrape(self):
            button = self.driver.find_element(By.XPATH,'//*[@id="mylocation"]')
            button.click()
            time.sleep(2.5)
            button.click()
            time.sleep(1.5)
            coords = self.getlatlng(self.driver.current_url)
            return [coords[0],coords[1]]

url = "https://www.google.com/maps"
gmaps = GGMaps(url)



BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "Lxp1BFcimniWOdJucVzg"

def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setValue":
            temp_data['value'] = jsonobj['params']
            client.publish('v1/devices/me/attributes', json.dumps(temp_data), 1)
    except:
        pass


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")


client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

temp = 30
humi = 50
latlong = gmaps.scrape()
latitude = latlong[0]
longitude = latlong[1]

while True:
    collect_data = {'temperature': temp, 'humidity': humi, 'latitude':latitude,'longitude':longitude}
    latlong1 = gmaps.scrape()
    latitude = latlong1[0]
    longitude = latlong1[1]
    temp += 1
    humi += 1
    client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
    time.sleep(5)

