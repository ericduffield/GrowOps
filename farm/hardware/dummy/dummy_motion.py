from interfaces.sensors import ISensor, AReading
from datetime import datetime
from grove.grove_mini_pir_motion_sensor import GroveMiniPIRMotionSensor
import random as r


class DummyMotion(ISensor):
    _sensor_model: str
    reading_type: AReading.Type

    def __init__(self, gpio: int,  model: str, type: AReading.Type):
        """Constructor for Motion Sensor class.
        :param str model: specific model of sensor hardware. Ex. Grove - Adjustable PIR Motion Sensor
        :param ReadingType type: Type of reading this sensor produces. Ex. 'MOTION'
        """
        self.gpio = gpio
        self._sensor_model = model
        self.reading_type = type
        self.unit = AReading.Unit.NONE
        self._sensor = GroveMiniPIRMotionSensor(gpio)
        self._sensor.on_detect = self._callback
        self.readings: list[float] = []
        
    def read_sensor(self) -> list[AReading]:
        """Takes a reading form the sensor
        :return list[AReading]: Returns a list of readings that contain a float timestamp of when motion was detected
        """
        i = 0
        while i < 10:
            if r.uniform(0,1) < 0.2:
                self._callback()
            i+=1
        
        readings = []
        for reading in self.readings:
            readings.append(AReading(self.reading_type, self.unit, reading))
        self.readings = []
        return readings
    
    def _callback(self):
        dt = datetime.now()
        self.readings.append(dt.timestamp())