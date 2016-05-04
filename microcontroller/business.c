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

//uint8 code table[]={0x3f,0x06,0x5b,0x4f,0x66,0x6d,0x7d,0x07,0x7f,0x6f};//����С����Ĺ�������ܶ�ֵ
//uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};

uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};
sbit P2_0 = P2^0;
sbit P2_1 = P2^1;
sbit P2_2 = P2^2;
sbit P2_3 = P2^3;




SEGMENT_DISPLAYER segmentDisplaysIndex = SEGMENT_DISPLAYER_1_ON;

sbit lockerPin=P1^7;	//����λѡ

int timer;

typedef enum{
	LOCK,
	UNLOCK
}LOCKER_STATUS;

LOCKER_STATUS lockerStatus = LOCK;



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
					uartSendStr(uartData);
					dealLocker(uartData);
					uartDataClean();
					setEvent(EVENT_UART_RECIVE, ENABLE,20,1);
				}
				break;
				case EVENT_LOCK_LOCKER:
				{
					lockerStatus = UNLOCK;
					reportLockerStatus();
					//uartSendStr("UNLOCK\r\n");
				}
				break;
				case EVENT_LOCKER_STATUS_RSP:
				{
					reportLockerStatus();
					setEvent(EVENT_LOCKER_STATUS_RSP, ENABLE,3000,1);
				}
				break;
				/*������ʱ*/
				case EVENT_LOCKER_DAOJISHI:
				{
					
			

						

					/*if(timer < -1)
					{
						timer =LOCKER_TIMER;
						setEvent(EVENT_LOCKER_LOCKED, ENABLE,1,1);
						setEvent(EVENT_LOCKER_DAOJISHI, DISABLE,0,0);
						lockerStatus = LOCK;
						reportLockerStatus();
					}
					else
					{					
						uartSendStr("EVENT_LOCKER_DAOJISHI\r\n");
						setEvent(EVENT_LOCKER_DAOJISHI, ENABLE,200,1);
						lockerStatus = UNLOCK;
						reportLockerStatus();						
					}*/
				}
				break;
				case EVENT_LOCKER_LOCKED:

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
	if(uartIndex < 5 ) return;
	if(uartIndex == 0) return;
	
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
}
void reportLockerStatus()
{
	if(lockerStatus == UNLOCK)
	{
		lockerPin = 1;
		//uartSendStr("unlock\r\n");
	}
	else
	{
		lockerPin = 0;
		//uartSendStr("lock\r\n");
	}
}
