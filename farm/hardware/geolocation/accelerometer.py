from interfaces.sensors import ISensor, AReading
import seeed_python_reterminal.core as rt
import seeed_python_reterminal.acceleration as rt_accel
import math


class Accelerometer(ISensor):
    def __init__(self, model: str) -> None:
        self._model = model
        self.type = type
        self.device = rt.get_acceleration_device()

    def read_sensor(self) -> list[AReading]:
        oldAccelerationX = oldAccelerationY = oldAccelerationZ = None
        accelerationX = accelerationY = accelerationZ = None

        while True:
            for event in self.device.read_loop():
                accelEvent = rt_accel.AccelerationEvent(event)
                if str(accelEvent.name) == "AccelerationName.X":
                    oldAccelerationX = accelerationX
                    accelerationX = accelEvent.value
                elif str(accelEvent.name) == "AccelerationName.Y":
                    oldAccelerationY = accelerationY
                    accelerationY = accelEvent.value
                elif str(accelEvent.name) == "AccelerationName.Z":
                    oldAccelerationZ = accelerationZ
                    accelerationZ = accelEvent.value

                # if all acceleration values are not None
                if accelerationX and accelerationY and accelerationZ and \
                        oldAccelerationX and oldAccelerationY and oldAccelerationZ:

                    vibration = math.sqrt(
                        (accelerationX - oldAccelerationX)**2 +
                        (accelerationY - oldAccelerationY)**2 +
                        (accelerationZ - oldAccelerationZ)**2)

                    # Formulas from: https://stackoverflow.com/questions/68457455/calculation-of-yaw-pitch-and-roll-from-acceleration

                    pitch_angle = math.atan2(-accelerationX, (math.sqrt((accelerationY*accelerationY) +
                                                                        (accelerationZ*accelerationZ)))) * (180 / math.pi)

                    roll_angle = math.atan2(accelerationY, math.sqrt(
                        accelerationX*accelerationX + accelerationZ*accelerationZ)) * (180 / math.pi)

                    return [
                        AReading(AReading.Type.VIBRATION,
                                 AReading.Unit.UNITS, vibration),
                        AReading(AReading.Type.PITCH,
                                 AReading.Unit.DEGREES, pitch_angle),
                        AReading(AReading.Type.ROLL,
                                 AReading.Unit.DEGREES, roll_angle),
                    ]
