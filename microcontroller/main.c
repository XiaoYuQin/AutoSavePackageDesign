#include <reg52.h>	   //此文件中定义了52的一些特殊功能寄存器
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

sbit lockerPin=P1^7;	//定义位选

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
	//uartSendStr("int..\r\n");
	//Init_Timer0();
//	TIM2Inital();

	userSystemEventInit();		
	//uartSendStr("int end...\r\n");	
	lcd_init();
}
void main()
{ 
	int i;
  	init();

	
	//LED1  = 0; //置P1_7口为低电平 ,点亮LED
 		
	setEvent(EVENT_UART_RECIVE, ENABLE,30,1);
	setEvent(EVENT_LOCKER_STATUS_RSP, ENABLE,300,1);
	setEvent(EVENT_LOCKER_LOCKED, ENABLE,1,1);
	
	//setEvent(EVENT_SEGMENT_DISPLAYER, ENABLE,1,1);
	uartDataClean();	

	while (dis1 [i]!='\0')
	{
		lcd_wdat (dis1 [i]);
		i++;	
	} 

	lcd_pos (0x40);
	i=0;
	while (dis2 [i]!='\0')
	{
		lcd_wdat (dis2 [i]);
		i++;	
	} 
	//uartSendStr("LCD \r\n");	
	dealWHISU();
	uartSendByte(U8T_data_H);	
	uartSendByte(U8T_data_L);	
	uartSendByte(U8RH_data_H);	
	uartSendByte(U8RH_data_L);	
	while(1)
	{
		/*for(i=0;i<10;i++)	  
		{
		 P0=table[i];
		   P2_0 = 0;
		   delayms(5);
		   //P2_0 = 1;
		   delayms(10000);
		}	 */
		userBusinessEventHandle();

	}
}


