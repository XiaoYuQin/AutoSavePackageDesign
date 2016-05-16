#include <reg52.h>	   //���ļ��ж�����52��һЩ���⹦�ܼĴ���
#include "timer.h"
#include "uart.h"
#include "user_sys.h"
#include "phy.h"
#include "business.h"
#include "1602.h"
#include "DHT11.h"

extern xdata uint8  U8T_data_H,U8T_data_L,U8RH_data_H,U8RH_data_L,U8checkdata;

/*
sbit P2_0 = P2^0;
sbit P2_1 = P2^1;
sbit P2_2 = P2^2;
sbit P2_3 = P2^3;*/

//uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};

uint8 code dis1[]={"QXMCU"};
uint8 code dis2[]={"HTTP://QXMCU.COM"};

sbit lockerPin=P1^7;	//����λѡ

void delayms(int i)
{
  int j,k; 
  for(j=i;j>0;j--)
    for(k=125;k>0;k--);
}

void init()
{

	lockerPin = 0;
	Init_Timer0();

	InitUART(B57600);	
	uartSendStr("int..\r\n");
	//Init_Timer0();
//	TIM2Inital();

	userSystemEventInit();		
	uartSendStr("int end...\r\n");	
	lockerInit();
	
}
void main()
{ 
	int i;
  	init();

	
	//LED1  = 0; //��P1_7��Ϊ�͵�ƽ ,����LED
 		
	setEvent(EVENT_UART_RECIVE, ENABLE,30,1);
	setEvent(EVENT_LOCKER_STATUS_RSP, ENABLE,300,1);
	//setEvent(EVENT_SEGMENT_DISPLAYER, ENABLE,1,1);
	uartDataClean();	

	i=0;
	uartSendStr("LCD \r\n");	
	while(1)
	{
		userBusinessEventHandle();

	}
}


