
int buttons[] = {1,2,3,4,5,6,7,8};
int potentiometers[] = {9,10,11,12,13,14,15,16};
void setup() {
  Serial.begin(9600);

  pinMode(A0, INPUT);
  pinMode(A1, INPUT);
  pinMode(A2, INPUT);
  pinMode(A3, INPUT);
  pinMode(A4, INPUT);
  pinMode(A5, INPUT);
  pinMode(A6, OUTPUT);
  pinMode(A7, OUTPUT);

  for(int i=2; i <=12;i++){
    pinMode(i, INPUT_PULLUP);  
  }

  for(int i = 4; i <=9; i++){
    pinMode(i, INPUT);  
  }
  
  
}
void loop() {
  int input = 0;
  for(int i=2; i <=12;i++){
    if(digitalRead(i) == LOW){
      input |= (1 << (i-2));
    }
    else{
      input |= (0 << (i-2));
    }
  }

  for(int i=4; i <=9;i++){
    if(digitalRead(i) == HIGH){
      input |= (1 << (i-2));
    }
    else{
      input |= (0 << (i-2));
    }
  }

  for(int i = 14; i<=19 ; i++){
    float val = analogRead(i);
    val = floatMap(val, 0, 1023, 0, 1);
    if(val > 0.5f){
      input |= (1 << (i-3));  
    }
    if(val < 0.2f){
      input |= (0 << (i-3));  
    }
  }
  Serial.println(input);
  delay(200);
}

float floatMap(float x, float in_min, float in_max, float out_min, float out_max)
{
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}
