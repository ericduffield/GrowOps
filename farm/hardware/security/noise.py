
from interfaces.sensors import AReading, ISensor
from grove.adc import ADC


class NoiseDetector(ISensor):
    _sensor_model: str
    reading_type: AReading.Type
    _channel_0_high = 850
    _channel_0_low = 350
    _channel_1_high = 1600
    _channel_1_low = 1200

    def __init__(self,  model: str, type: AReading.Type, channel_0: int, channel_1: int):
        """Constructor for Sensor  class. May be called from childclass.
        :param str model: specific model of sensor hardware. Ex. AHT20 or LTR-303ALS-01
        :param ReadingType type: Type of reading this sensor produces. Ex. 'NONE'
        """
        self._sensor_model = model
        self.reading_type = type
        self._sensor = ADC(0x04)
        self.unit = AReading.Unit.RELATIVENOISE
        self._channel_0 = channel_0
        self._channel_1 = channel_1

    def read_sensor(self) -> list[AReading]:
        """Takes a reading form the sensor
        :return list[AReading]: List of readings measured by the sensor.
        can return up to two values, one if noise is detected on channel 0 another
        if detected on channel 1
        """
        channel_0_voltage = self._sensor.read_voltage(self._channel_0)
        channel_1_voltage = self._sensor.read_voltage(self._channel_1)
        
        readings = self._calculate_readings(channel_0_voltage, channel_1_voltage)
        
        return readings
    
    def _calculate_readings(self, channel_0: float, channel_1: float) -> list[AReading]:
        readings: list[AReading] = []
        if channel_0 < self._channel_0_low or channel_0 > self._channel_0_high:
            if channel_1 < self._channel_1_low or channel_1 > self._channel_1_high:
                readings.append(
                    AReading(self.reading_type, self.unit, channel_1))
                readings.append(
                    AReading(self.reading_type, self.unit, channel_0))
            else:
                readings.append(
                    AReading(self.reading_type, self.unit, channel_0))

        if channel_1 < self._channel_1_low or channel_1 > self._channel_1_high:
            if channel_0 < self._channel_0_low or channel_0 > self._channel_0_high:
                readings.append(
                    AReading(self.reading_type, self.unit, channel_1))
                readings.append(
                    AReading(self.reading_type, self.unit, channel_0))
            else:
                readings.append(
                    AReading(self.reading_type, self.unit, channel_1))
                
        return readings
