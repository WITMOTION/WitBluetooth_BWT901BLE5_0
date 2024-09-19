/*
  Arduino HM10 Bluetooth Test for Unity Plugin
  Language: Wiring/Arduino

  This program waits for input from the Unity app and reflects any strings back
  to that app. If this app receives a single byte of 0x01 it will toggle the led

  The circuit:
  - connect HM10 BLE device to serial RX/TX lines

  created 05 July 2018
  by Tony Pitman

  http://www.shatalmic.com/arduino-hm10-test
*/

int inByte = 0;         // incoming serial byte
boolean ledStatus = false;

void setup() {
  // start serial port at 9600 bps and wait for port to open:
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }

    // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, LOW);
}

void loop() {
  // if we get a valid byte, read analog ins:
  if (Serial.available() > 0) {
    // get incoming byte:
    inByte = Serial.read();
    if (inByte == 0x01) {
      if (ledStatus) {
        digitalWrite(LED_BUILTIN, HIGH);
        Serial.print("Setting LED On");
        ledStatus = false;
      }
      else {
        digitalWrite(LED_BUILTIN, LOW);
        Serial.print("Setting LED Off");
        ledStatus = true;
      }
    }
    else {
      Serial.write(inByte);
    }
  }
}

