#include "user_sys.h"
#include "uart.h"
#include "business.h"
#include <string.h>
#include <stdio.h>
#include <reg52.h>
#include "phy.h"

#define LOCKER_TIMER 7

extern app_Event getApp;

extern uint8 uartData[15];
extern uint8 uartIndex;

#if 0
uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};
sbit P2_0 = P2^0;
sbit P2_1 = P2^1;
sbit P2_2 = P2^2;
sbit P2_3 = P2^3;
#endif

//uint8 code table[]={0x3f,0x06,0x5b,0x4f,0x66,0x6d,0x7d,0x07,0x7f,0x6f};//不带小数点的共阴数码管段值
//uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};

uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};
sbit P2_0 = P2^0;
sbit P2_1 = P2^1;
sbit P2_2 = P2^2;
sbit P2_3 = P2^3;


sbit locker1 = P1^0;
sbit locker2 = P1^1;
sbit locker3 = P1^2;


SEGMENT_DISPLAYER segmentDisplaysIndex = SEGMENT_DISPLAYER_1_ON;

sbit lockerPin=P1^7;	//定义位选

int timer;

typedef enum{
	LOCK,
	UNLOCK
}LOCKER_STATUS;

typedef enum{
	LOCKER1_STATUS_REPORT,
	LOCKER2_STATUS_REPORT,
	LOCKER3_STATUS_REPORT
}LOCKER_STATUS_STEP;

LOCKER_STATUS_STEP lockerStatusStep;



LOCKER_STATUS lockerStatus = LOCK;

xdata LOCKER_STATUS lockerStatus1 = LOCK;
xdata LOCKER_STATUS lockerStatus2 = LOCK;
xdata LOCKER_STATUS lockerStatus3 = LOCK;





void userBusinessEventHandle()
{
	uint8 i;
	for(i=0;i<EVENT_NUMBER;i++)
	{
		if((getApp.eventEDable_t[i]==ENABLE)&&(getApp.eventResultFlag_t[i]==ENABLE)&&(getApp.eventDoCount_t[i]<getApp.eventSetCount_t[i]))
		{
			userSystemSetEventDone(i);
			switch(getApp.eventId_t[i])
			{
				case EVENT_UART_RECIVE:
				{
					dealLocker(uartData);
					uartDataClean();
					setEvent(EVENT_UART_RECIVE, ENABLE,20,1);
				}
				break;
				case EVENT_LOCKER_STATUS_RSP:
				{
					//reportLockerStatus();
					setEvent(EVENT_LOCKER_STATUS_RSP, ENABLE,500,1);//定位
				}
				break;
				case EVENT_LOCKER1_UNLOCKED:
					lockerStatus1 = UNLOCK;
					//uartSendStr("unlocker1\r\n");
					if(lockerStatus1 == LOCK)
						uartSendStr("L1A\r\n");
					else
						uartSendStr("L1B\r\n");			
					locker1 = 1;
				break;
				case EVENT_LOCKER2_UNLOCKED:
					lockerStatus2 = UNLOCK;					
					//uartSendStr("unlocker2\r\n");
					if(lockerStatus2 == LOCK)
						uartSendStr("L2A\r\n");
					else
						uartSendStr("L2B\r\n");			
					locker2 = 1;
				break;
				case EVENT_LOCKER3_UNLOCKED:
					lockerStatus3 = UNLOCK;
					//uartSendStr("unlocker3\r\n");
					if(lockerStatus3 == LOCK)
						uartSendStr("L3A\r\n");
					else
						uartSendStr("L3B\r\n");			
					locker3 = 1;
				break;
				case EVENT_SEGMENT_DISPLAYER:
				{
					switch(segmentDisplaysIndex)
					{
						case SEGMENT_DISPLAYER_1_ON:
							P2_3 = 1;
							P0=table[0];
							P2_0 = 0;
							segmentDisplaysIndex = SEGMENT_DISPLAYER_2_ON;
						break;

						case SEGMENT_DISPLAYER_2_ON:
							P2_0 = 1;
							P0=table[1];
							P2_1 = 0;
							segmentDisplaysIndex = SEGMENT_DISPLAYER_3_ON;
						break;
						case SEGMENT_DISPLAYER_3_ON:
							P2_1 = 1;
							P0=table[2];
							P2_2 = 0;
							segmentDisplaysIndex = SEGMENT_DISPLAYER_4_ON;
						break;
						case SEGMENT_DISPLAYER_4_ON:
							P2_2 = 1;
							P0=table[3];
							P2_3 = 0;
							segmentDisplaysIndex = SEGMENT_DISPLAYER_1_ON;
						break;		
					}
					setEvent(EVENT_SEGMENT_DISPLAYER, ENABLE,1,1);
				}
					//uartSendStr("EVENT_SEGMENT_DISPLAYER\r\n");
					/*P0=table[i];
					P2_0 = 0;*/
					//dealSegmentDisplays();
					//setEvent(EVENT_SEGMENT_DISPLAYER, ENABLE,1,1);
					break;
				default:
					break;
			
			}
		}
		else if((getApp.eventEDable_t[i]==ENABLE)&&(getApp.eventDoCount_t[i]>=getApp.eventSetCount_t[i]))
		{
			getApp.eventEDable_t[i]=DISABLE;
		}
	}
}

void dealLocker(uint8 *cmd)
{
	if(uartIndex < 2 ) return;
	if(uartIndex == 0) return;

	switch(cmd[0])
	{
		case '1':
		{
			locker1 = 0;
			//uartSendStr("open locker1 5s\r\n");
			if(lockerStatus1 == LOCK)
				uartSendStr("L1A\r\n");
			else
				uartSendStr("L1B\r\n"); 	
			lockerStatus1 = LOCK;			
			setEvent(EVENT_LOCKER1_UNLOCKED, ENABLE,3000,1);
		}
		break;
		case '2':
		{
			locker2 = 0;			
			if(lockerStatus2 == LOCK)
				uartSendStr("L2A\r\n");
			else
				uartSendStr("L2B\r\n"); 				
			lockerStatus2 = LOCK;			
			//uartSendStr("open locker2 5s\r\n");
			setEvent(EVENT_LOCKER2_UNLOCKED, ENABLE,3000,1);
		}
		break;
		case '3':
		{
			locker3 = 0;
			if(lockerStatus3 == LOCK)
				uartSendStr("L3A\r\n");
			else
				uartSendStr("L3B\r\n");			
			lockerStatus3 = LOCK;			
			//uartSendStr("open locker3 5s\r\n");
			setEvent(EVENT_LOCKER3_UNLOCKED, ENABLE,3000,1);		
		}
		break;
	}

#if 0
	
	if((cmd[0] == 'U')&&(cmd[1] == 'N')&&cmd[2] == 'L'&&cmd[3] == 'O'&&cmd[4] == 'C'&&cmd[5] == 'K'){
		lockerPin = 1;
		timer = LOCKER_TIMER;
		uartSendStr("open locker 5s\r\n");
		lockerStatus = UNLOCK;
		reportLockerStatus();
		//setEvent(EVENT_LOCK_LOCKER, ENABLE,500,1);
		setEvent(EVENT_LOCKER_DAOJISHI, ENABLE,1,1);
	}
	if((cmd[0] == 'T')&&(cmd[1] == 'O')&&cmd[2] == 'L'&&cmd[3] == 'O'&&cmd[4] == 'C'&&cmd[5] == 'K'){

		timer = 0;
		lockerStatus = LOCK;
		reportLockerStatus();
		setEvent(EVENT_LOCKER_LOCKED, ENABLE,1,1);
		setEvent(EVENT_LOCKER_DAOJISHI, DISABLE,0,0);
	}
#endif	
}
void reportLockerStatus()
{
	switch(lockerStatusStep)
	{
		case LOCKER1_STATUS_REPORT:
		{
			lockerStatusStep = LOCKER2_STATUS_REPORT;
			if(lockerStatus1 == LOCK)
				uartSendStr("L1A\r\n");
			else
				uartSendStr("L1B\r\n"); 	
		}
		break;
		case LOCKER2_STATUS_REPORT:
		{
			lockerStatusStep = LOCKER3_STATUS_REPORT;
			if(lockerStatus2 == LOCK)
				uartSendStr("L2A\r\n");
			else
				uartSendStr("L2B\r\n"); 	
		}
		break;
		case LOCKER3_STATUS_REPORT:
		{
			lockerStatusStep = LOCKER1_STATUS_REPORT;
			if(lockerStatus3 == LOCK)
				uartSendStr("L3A\r\n");
			else
				uartSendStr("L3B\r\n");
		}
		break;
	}	
}
void lockerInit()
{
	locker1 = 1;
	locker2 = 1;
	locker3 = 1;
}
