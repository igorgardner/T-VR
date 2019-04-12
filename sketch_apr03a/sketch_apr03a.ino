
// Example 5 - Receive with start- and end-markers combined with parsing

const byte numChars = 32;
char receivedChars[numChars];
char tempChars[numChars];        // temporary array for use when parsing

      // variables to hold the parsed data
char messageFromPC[numChars] = {0};
float inputSteer = 0.0;
float inputThrottle = 0.0;

boolean newData = false;

int pin0 = 4;
int pin1 = 5;
int pin2 = 6;
int pin3 = 7;

//============

void setup() {
    Serial.begin(9600);

    pinMode(pin0, OUTPUT);
    pinMode(pin1, OUTPUT);
    pinMode(pin2, OUTPUT);
    pinMode(pin3, OUTPUT);
}

//============

void loop() {
    if (Serial.available() > 0) 
    {
          // read the incoming byte:
          char incomingByte = Serial.read();

          if (incomingByte == '1')
            digitalWrite(pin0, 1);
          else
            digitalWrite(pin0, 0);

          incomingByte = Serial.read();

          if (incomingByte == '1')
            digitalWrite(pin1, 1);
          else
            digitalWrite(pin1, 0);

          incomingByte = Serial.read();

          if (incomingByte == '1')
            digitalWrite(pin2, 1);
          else
            digitalWrite(pin2, 0);

          incomingByte = Serial.read();

          if (incomingByte == '1')
            digitalWrite(pin0, 1);
          else
            digitalWrite(pin0, 0);
    }
}

//============

void recvWithStartEndMarkers() {
    static boolean recvInProgress = false;
    static byte ndx = 0;
    char startMarker = '<';
    char endMarker = '>';
    char rc;

    while (Serial.available() > 0 && newData == false) {
        rc = Serial.read();

        if (recvInProgress == true) {
            if (rc != endMarker) {
                receivedChars[ndx] = rc;
                ndx++;
                if (ndx >= numChars) {
                    ndx = numChars - 1;
                }
            }
            else {
                receivedChars[ndx] = '\0'; // terminate the string
                recvInProgress = false;
                ndx = 0;
                newData = true;
            }
        }

        else if (rc == startMarker) {
            recvInProgress = true;
        }
    }
}

//============

void parseData() {      // split the data into its parts

    char * strtokIndx; // this is used by strtok() as an index

    strtokIndx = strtok(tempChars,",");// get the first part - the string
    strcpy(messageFromPC, strtokIndx); // copy it to messageFromPC
    
 
    strtokIndx = strtok(NULL, ","); // this continues where the previous call left off
    inputSteer = atoi(strtokIndx);     // convert this part to a float

    strtokIndx = strtok(NULL, ",");
    inputThrottle = atof(strtokIndx);     // convert this part to a float

}

//============

void showParsedData() {
    Serial.print("Message ");
    Serial.println(messageFromPC);
    Serial.print("Integer ");
    Serial.println(inputSteer);
    Serial.print("Float ");
    Serial.println(inputThrottle);
}
