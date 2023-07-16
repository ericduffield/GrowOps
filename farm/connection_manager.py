import asyncio
import json
from typing import Callable
from interfaces.actuators import ACommand
from interfaces.sensors import AReading

from azure.iot.device.aio import IoTHubDeviceClient
from azure.iot.device import Message
from azure.iot.device import MethodResponse, MethodRequest
from dotenv import dotenv_values


class ConnectionConfig:
    """Represents all information required to successfully connect client to cloud gateway.
    """

    # Key names for configuration values inside .env file. See .env.example
    # Constants included as static class property
    DEVICE_CONN_STR = "IOTHUB_DEVICE_CONNECTION_STRING"

    def __init__(self, device_str: str) -> None:
        self._device_connection_str = device_str


class ConnectionManager:
    TELEMETRY_INTERVAL_PATCH = 'telemetry_interval'
    """Component of farm system responsible for communicating with the cloud gateway
    Includes registering command and reading endpoints and sending and receiving data
    """

    def __init__(self,) -> None:
        """Constructor for ConnectionManager and initialized an internal cloud
        gateway client.
        """
        self.telemetry_interval: int = 5
        self._connected = False
        self._config: ConnectionConfig = self._load_connection_config()
        self._client = IoTHubDeviceClient.create_from_connection_string(
            self._config._device_connection_str
        )
        # add more direct methods here if necessary
        self.method_names = ['is_online', ACommand.Type.FAN, ACommand.Type.LIGHT_PULSE,
                             ACommand.Type.BUZZER, ACommand.Type.LOCK, ACommand.Type.DOOR]
        self._client.on_method_request_received = self.method_request_handler
        self.DEBUG = False

    def _load_connection_config(self) -> ConnectionConfig:
        """Loads a connection credentials from a .env file in the farms top level
        directory

        Returns:
            ConnectionConfig: object with config information loaded from .env file
        """
        config = dotenv_values('.env')
        conn_string = config[ConnectionConfig.DEVICE_CONN_STR]
        return ConnectionConfig(str(conn_string))

    def _on_message_received(self, message: Message) -> None:
        """Callback for handling new messages from the cloud gateway. Once the 
        message is received and processed, it dispatches an ACommand to
        DeviceManager using the _command_callback method

        Args:
            message (Message): Incoming cloud gateway message. MEssages with actuator commands
            must contain a custom property of 'command-type' and a json encoded string as the
            body
        """
        try:
            command_type = message.custom_properties['command-type']
            command = ACommand(command_type, message.data)
            self._command_callback(command)
        except KeyError as e:
            print(f"{e} key not found in message properties")
        except Exception as e:
            print(
                f"Exception in _on_message_received in ConnectionManager: {e}")

    async def connect(self) -> None:
        """Connectes to the cloud gateway using connection credentials and setups a
        message handler
        """
        try:
            await self._client.connect()
            self._connected = True
            self._client.on_message_received = self._on_message_received
            self._twin = await self._client.get_twin()
            self._client.on_twin_desired_properties_patch_received = self.twin_patch_handler
            self.telemetry_interval = int(
                self._twin['reported']['telemetry_interval'])
            if (self.telemetry_interval > 30 or self.telemetry_interval < 1):
                self.telemetry_interval = 5
            print(f"Telemetry interval set to {self.telemetry_interval}")
        except Exception as e:
            print(f"Exception in connect() in ConnectionManager: {e}")

    def twin_patch_handler(self, patch: dict):
        """The callback to call when the twin sends a patch request

        Args:
            patch (dict): The patch of all the properties 
        """
        try:
            self.telemetry_interval = int(patch[self.TELEMETRY_INTERVAL_PATCH])
            print(f"Telemetry interval changed to {self.telemetry_interval}")
        except:
            print(f"Couldn't parse the patch into value")

    def register_command_callback(self, command_callback: Callable[[ACommand], None]) -> None:
        """Registers an external callback function to handle received commands

        Args:
            command_callback (Callable[[ACommand], None]): method to be called whenever a new command is
            received
        """
        self._command_callback = command_callback

    async def send_readings(self, readings: list[AReading]) -> None:
        """Sends a list of sensor readings as messages to the cloud gateway

        Args:
            readings (list[AReading]): List of readings to be sent
        """
        readingsDict: dict = {}
        for reading in readings:
            readingsDict[reading.reading_type] = reading.export_json()

        msg = Message(json.dumps(readingsDict))
        msg.custom_properties['reading-type'] = reading.reading_type
        await self._client.send_message(msg)

    async def method_request_handler(self, method_request: MethodRequest):
        """Handles direct Methods from IoT Hub.
        If the request is a valid one then it sends a 200 status with empty payload
        else returns 400 status with payload of unknown method name

        Args:
            method_request (MethodRequest): A request to invoke a direct method
            containing a name that must be in the connection managers 
            approved list of names.
        """
        # check if method name is one we are listening for
        if method_request.name in self.method_names:
            status = 200
            payload = {}
            print(f"Method request: {method_request.name}. It is valid")

            if (method_request.name == 'is_online'):
                return

            self._command_callback(
                ACommand(method_request.name, json.dumps(method_request.payload)))

        else:
            status = 400
            payload = {"details": "method name unknown"}
            if self.DEBUG:
                print(
                    f"Attempted direct method with name {method_request.name}, it is not a valid request name")
        method_response = MethodResponse.create_from_method_request(
            method_request, status, payload)
        print(f"Sending Response: {str(method_response)}")
        await self._client.send_method_response(method_response)


async def main_demo():

    def dummy_callback(command: ACommand):
        print(command)

    connection_manager = ConnectionManager()
    connection_manager.register_command_callback(dummy_callback)
    await connection_manager.connect()

    TEST_SLEEP_TIME = 3

    while True:

        # ===== Create a list of fake readings =====
        fake_temperature_reading = AReading(AReading.Type.TEMPERATURE,
                                            AReading.Unit.CELCIUS, 12.34)
        fake_humidity_reading = AReading(AReading.Type.HUMIDITY,
                                         AReading.Unit.HUMIDITY, 56.78)

        # ===== Send fake readings =====
        await connection_manager.send_readings(
            [fake_temperature_reading, fake_humidity_reading])

        await asyncio.sleep(TEST_SLEEP_TIME)


if __name__ == "__main__":
    asyncio.run(main_demo())
