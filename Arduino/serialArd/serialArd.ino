int fwdPin = 5;
int bckPin = 6;
int lftPin = 4;
int rgtPin = 7;

String input;

void fwd1()
{

}
void fwd2()
{
  
}
void stp()
{
  
}
void bck()
{
  
}
void left()
{
  
}
void right()
{
  
}
void centre()
{
  
}

void setup() 
{
  Serial.setTimeout(50);
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

    Serial.println(input[1]);

    if(input[0] == '7') //sp2
      {
        digitalWrite(fwdPin, 0);
        digitalWrite(bckPin, 1);
      }
    else if(input[0] == '6') //sp1
     {
        analogWrite(fwdPin, 200);
        digitalWrite(bckPin, 1);
     }
    else if(input[0] == '5') //sp0
      {
        digitalWrite(fwdPin, 1);
        digitalWrite(bckPin, 1);
      }
    else if(input[0] == '4') //sp-1
     {
      analogWrite(bckPin, 200);
      digitalWrite(fwdPin, 1);
     }
     if(input[1] == '3') //st1
     {
      digitalWrite(lftPin, 0);
      digitalWrite(rgtPin, 1);
     }
    else if(input[1] == '2') //st0
     {
      digitalWrite(lftPin, 1);
      digitalWrite(rgtPin, 1);
     }
    else if(input[1] == '1') //st-1
     {
      digitalWrite(lftPin, 1);
      digitalWrite(rgtPin, 0);
     }
  }
}
