int fwdPin = 5;
int bckPin = 6;
int lftPin = 4;
int rgtPin = 7;

String input;

void fwd1()
{
  analogWrite(fwdPin, 140);
  digitalWrite(bckPin, 1);
}
void fwd2()
{
  digitalWrite(fwdPin, 0);
  digitalWrite(bckPin, 1);
}
void stp()
{
  digitalWrite(fwdPin, 1);
  digitalWrite(bckPin, 1);
}
void bck()
{
  analogWrite(bckPin, 140);
  digitalWrite(fwdPin, 1);
}
void left()
{
  digitalWrite(lftPin, 0);
  digitalWrite(rgtPin, 1);
}
void right()
{
  digitalWrite(lftPin, 1);
  digitalWrite(rgtPin, 0);
}
void centre()
{
  digitalWrite(lftPin, 1);
  digitalWrite(rgtPin, 1);
}

void setup() 
{
  Serial.begin(9600);
  while(!Serial){;}
    
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
  {
    input = Serial.readString();

    if(input.indexOf("Speed: 2") != -1)
      fwd2();
    if(input.indexOf("Speed: 1") != -1)
     fwd1();
    if(input.indexOf("Speed: 0") != -1)
      stp();
    if(input.indexOf("Speed: -1") != -1)
     bck();
    if(input.indexOf("Steer: 1") != -1)
     left();
    if(input.indexOf("Steer: 0") != -1)
     centre();
    if(input.indexOf("Steer: -1") != -1)
     right();
  }
}
