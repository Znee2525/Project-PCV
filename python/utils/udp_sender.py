import socket
import json

class UDPSender:
    def __init__(self, ip="127.0.0.1", port=5055):
        self.addr = (ip, port)
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    def send(self, data: dict):
        message = json.dumps(data).encode("utf-8")
        self.sock.sendto(message, self.addr)
