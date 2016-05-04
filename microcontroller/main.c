#include <reg52.h>	   //此文件中定义了52的一些特殊功能寄存器
#include "timer.h"
#include "uart.h"
#include "user_sys.h"
#include "business.h"

/*sbit P2_0 = P2^0;
sbit P2_1 = P2^1;
sbit P2_2 = P2^2;
sbit P2_3 = P2^3;
*/
//uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};

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
	uartSendStr("int..\r\n");
	//Init_Timer0();
//	TIM2Inital();

	userSystemEventInit();		
	uartSendStr("int end...\r\n");	

}
void main()
{ 
  	init();

	
	//LED1  = 0; //置P1_7口为低电平 ,点亮LED
 		
	setEvent(EVENT_UART_RECIVE, ENABLE,30,1);
	setEvent(EVENT_LOCKER_STATUS_RSP, ENABLE,300,1);
	setEvent(EVENT_LOCKER_LOCKED, ENABLE,1,1);
	
	setEvent(EVENT_SEGMENT_DISPLAYER, ENABLE,1,1);
	uartDataClean();	
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


