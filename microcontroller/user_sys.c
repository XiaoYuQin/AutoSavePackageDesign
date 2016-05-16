#include <stdio.h>

#include "user_sys.h"
#include "type.h"

app_Event getApp;

void userSystemSetEventDone(uint8 event_id)
{
	getApp.eventDoCount_t[event_id]++;
	if(getApp.eventDoCount_t[event_id]>=255)	
		getApp.eventDoCount_t[event_id]=0;
	getApp.eventResultFlag_t[event_id]=DISABLE;
}

void userSystemHandle()
{
	uint8_t i;
	for(i=0;i<EVENT_NUMBER;i++)
	{
		if(getApp.eventEDable_t[i]==ENABLE)
		{
			getApp.eventTimerCount_t[i]++;
			if(getApp.eventTimerCount_t[i]>getApp.eventTimerTime_t[i])
			{
				getApp.eventResultFlag_t[i]=ENABLE;
				getApp.eventTimerCount_t[i]=0;
				/*getApp.eventDoCount_t[i]++;
				if(getApp.eventDoCount_t[i]>=255)
					getApp.eventDoCount_t[i]=0;*/
			}
		}
		else
		{
			getApp.eventResultFlag_t[i]=DISABLE;
			getApp.eventTimerCount_t[i]=0;
			//getApp.eventDoCount_t[i]=0;
		}
	}
}
void userSystemEventInit()
{
	uint8_t i;
	for(i=0;i<EVENT_NUMBER;i++)
	{
		getApp.eventId_t[i]=0;
		getApp.eventTimerCount_t[i]=0;
		getApp.eventResultFlag_t[i]=DISABLE;
		getApp.eventEDable_t[i]=DISABLE;
		getApp.eventDoCount_t[i]=0;
		getApp.eventSetCount_t[i]=0;
		getApp.eventTimerTime_t[i]=0;
	}
}
void setEvent(uint8_t event_id,uint8_t event_edable,uint16_t event_ones_timer,uint8_t event_run_count)
{
	getApp.eventId_t[event_id]=event_id;
	getApp.eventTimerTime_t[event_id]=event_ones_timer;
	getApp.eventEDable_t[event_id]=event_edable;
	getApp.eventSetCount_t[event_id]=event_run_count;
	getApp.eventTimerCount_t[event_id]=0;
	getApp.eventDoCount_t[event_id]=0;
}










