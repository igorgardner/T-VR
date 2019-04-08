#include <SoftwareSerial.h>
#include <SerialCommand.h>

int fwdPin = 5;
int bckPin = 6;
int lftPin = 4;
int rgtPin = 7;

SerialCommand input;

void fwd1(const char *command)
{
  analogWrite(fwdPin, 140);
  digitalWrite(bckPin, 1);
}
void fwd2(const char *command)
{
  digitalWrite(fwdPin, 0);
  digitalWrite(bckPin, 1);
}
void stp(const char *command)
{
  digitalWrite(fwdPin, 1);
  digitalWrite(bckPin, 1);
}
void bck(const char *command)
{
  analogWrite(bckPin, 140);
  digitalWrite(fwdPin, 1);
}
void left(const char *command)
{
  digitalWrite(lftPin, 0);
  digitalWrite(rgtPin, 1);
}
void right(const char *command)
{
  digitalWrite(lftPin, 1);
  digitalWrite(rgtPin, 0);
}
void centre(const char *command)
{
  digitalWrite(lftPin, 1);
  digitalWrite(rgtPin, 1);
}

void setup() 
{
  Serial.begin(9600);
  while(!Serial){;}

  input.addCommand("Speed: 2", fwd2);
  input.addCommand("Speed: 1", fwd1);
  input.addCommand("Speed: 0", stp);
  input.addCommand("Speed: -1", bck);
  input.addCommand("Steer: 1", left);
  input.addCommand("Steer: 0", centre);
  input.addCommand("Steer: -1", right);
    
  pinMode(fwdPin, OUTPUT);
  pinMode(bckPin, OUTPUT);
  pinMode(lftPin, OUTPUT);
  pinMode(rgtPin, OUTPUT);

  digitalWrite(fwdPin, 1);
  digitalWrite(bckPin, 1);
  digitalWrite(lftPin, 1);
  digitalWrite(rgtPin, 1);
}

void loop() 
{
  if(Serial.available() > 0)
    input.readSerial(); 
}
