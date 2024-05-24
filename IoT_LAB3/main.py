print("IoT Gateway")
import paho.mqtt.client as mqttclient
import time
import json
import serial.tools.list_ports

BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
mess = ""

#TODO: Add your token and your comport
THINGS_BOARD_ACCESS_TOKEN = "ck8JDMcFi3S0ZG7D4UGp"


def get_port():
    ports = serial.tools.list_ports.comports()
    res = ""
    for i in range(len(ports)):
        strPort = str(ports[i])
        if "BBC micro:bit" in strPort:
            port = strPort.split(" ")
            res = port[0]
    return res
# TODO: get_port()
collect_data = {
            'temperature': 0,
            'light': 0
}
_DATA = {
            'TE': 'temperature',
            'LI': 'light',
}
_METHOD = {
    'setLED': 'LED1',
    'setFAN': 'FAN1',
    'setLED2': 'LED2',
    'setFAN2': 'FAN2',
}
bbc_port = get_port() # FIXME: declare port

if len(bbc_port) > 0:
    ser = serial.Serial(port=bbc_port, baudrate=115200)

def processData(data):
    global collect_data,_DATA
    data = data.replace("!", "")
    data = data.replace("#", "")
    splitData = data.split(":")
    print(splitData)

    collect_data[_DATA[splitData[1]]] = int(splitData[2])

def writeSerial(rc_data):
    if rc_data['method'] == 'setLED':
        data = f"{1 if rc_data['params'] else 0}#"
    elif rc_data['method'] == 'setFAN':
        data = f"{3 if rc_data['params'] else 2}#"
    elif rc_data['method'] == 'setLED2':
        data = f"{5 if rc_data['params'] else 4}#"
    elif rc_data['method'] == 'setFAN2':
        data = f"{7 if rc_data['params'] else 6}#"
    print("Sent data: " + data)
    ser.write(data.encode())

def readSerial():
    bytesToRead = ser.inWaiting()
    if (bytesToRead > 0):
        global mess
        mess = mess + ser.read(bytesToRead).decode("UTF-8")
        while ("#" in mess) and ("!" in mess):
            start = mess.find("!")
            end = mess.find("#")
            try:
                processData(mess[start:end + 1])
            except:
                pass
            if (end == len(mess)):
                mess = ""
            else:
                mess = mess[end+1:]
        return True


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")



def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        temp_data['value'] = jsonobj['params']
        if jsonobj['method'] == 'setLED':
            client.publish('v1/devices/me/LED1', json.dumps(temp_data), 1)
        if jsonobj['method'] == 'setLED2':
            client.publish('v1/devices/me/LED2', json.dumps(temp_data), 1)
        if jsonobj['method'] == 'setFAN':
            client.publish('v1/devices/me/FAN1', json.dumps(temp_data), 1)
        if jsonobj['method'] == 'setFAN2':
            client.publish('v1/devices/me/FAN2', json.dumps(temp_data), 1)
        writeSerial(jsonobj)
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

while True:

    if readSerial():
        client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
    time.sleep(1)