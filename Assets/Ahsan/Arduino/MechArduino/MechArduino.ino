

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

  pinMode(2, INPUT_PULLUP);
  pinMode(3, INPUT_PULLUP);
  pinMode(12, INPUT_PULLUP);


  for(int i = 4; i <=11; i++){
    pinMode(i, INPUT);  
  }
  
  
}
void loop() {
  if(Serial.available() > 0){
    
    char led = Serial.read();
    if(led == '1'){
      digitalWrite(13, HIGH);
    }
    if(led == '0'){
      digitalWrite(13, LOW);
    }
  }
  

  uint32_t input = 0;
    
  input |= digitalButton(input, 2);
  input |= digitalButton(input, 3);
  input |= digitalButton(input, 12);

  for(int i=4; i <=11;i++){
    input |= digitalSwitch(input, i);
  }

  for(int i = A0; i<=A5 ; i++){
    float val = analogRead(i);
    val = floatMap(val, 0, 1023, 0, 1);
    if(val > 0.75f){
      input |= (uint32_t)((uint32_t)1 << (i-3));  
    }
    if(val < 0.5f){
      input |= (uint32_t)((uint32_t)0 << (i-3));  
    }
  }
  Serial.println((input));
  delay(200); 
}

float floatMap(float x, float in_min, float in_max, float out_min, float out_max)
{
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

uint32_t digitalButton(uint32_t input, int pin){
  if(digitalRead(pin) == LOW){
      input |= (uint32_t)((uint32_t)1 << (pin-2));
    }
    else{
      input |= (uint32_t)((uint32_t)0 << (pin-2));
  }
  return input;
}

uint32_t digitalSwitch(uint32_t input, int pin){
  if(digitalRead(pin) == HIGH){
      input |= (uint32_t)((uint32_t)1 << (pin-2));
    }
    else{
      input |= (uint32_t)((uint32_t)0 << (pin-2));
  }
  return input;
}
