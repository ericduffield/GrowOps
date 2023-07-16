# from interfaces.sensors import ISensor, AReading
import serial
import pynmea2

from interfaces.sensors import ISensor, AReading


class Gps(ISensor):
    def __init__(self, model: str) -> None:
        self._model = model
        self.type = type
        self.ser = serial.Serial('/dev/ttyAMA0', 9600, timeout=1)
        self.ser.reset_input_buffer()
        self.ser.flush()

    def read_sensor(self) -> list[AReading]:
        while True:
            try:
                # code from https://github.com/microsoft/IoT-For-Beginners/blob/main/3-transport/lessons/1-location-tracking/code-gps-decode/pi/gps-sensor/app.py

                line = self.ser.readline().decode('utf-8')
                msg = pynmea2.parse(line)

                if msg.sentence_type == 'GGA':
                    lat = pynmea2.dm_to_sd(msg.lat)
                    lon = pynmea2.dm_to_sd(msg.lon)

                    if msg.lat_dir == 'S':
                        lat = lat * -1

                    if msg.lon_dir == 'W':
                        lon = lon * -1

                    if (msg.num_sats == "00"):
                        lat = lon = "None"

                    return [
                        AReading(AReading.Type.LATITUDE,
                                 AReading.Unit.DEGREES, lat),
                        AReading(AReading.Type.LONGITUDE,
                                 AReading.Unit.DEGREES, lon)
                    ]

            except Exception as e:
                print("Error: ", str(e))
