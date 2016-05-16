#ifndef __USER_SYS_H_
#define __USER_SYS_H_

#include "type.h"

#define EVENT_NUMBER			EVENT_LAST

#define ENABLE 1
#define DISABLE 0


typedef enum
{
	EVENT_UART_RECIVE,
	EVENT_LOCKER_STATUS_RSP,	
	EVENT_LOCKER1_UNLOCKED,
	EVENT_LOCKER2_UNLOCKED,
	EVENT_LOCKER3_UNLOCKED,	
	EVENT_SEGMENT_DISPLAYER,
	EVENT_LAST
}EVENT_LIST;



/*ȫ���¼��ṹ�壬APP  �������*/
typedef struct 
{
	uint8_t 	eventId_t[EVENT_NUMBER];			/*�¼���id*/
	uint16_t 	eventTimerTime_t[EVENT_NUMBER];		/*ʱ�䶨ʱ���Ķ�ʱʱ��*/
	uint16_t 	eventTimerCount_t[EVENT_NUMBER];	/*�¼���ʱ�����жϴ���*/
	uint8_t 	eventDoCount_t[EVENT_NUMBER];		/*�¼����붨ʱ���Ĵ���*/
	uint8_t 	eventSetCount_t[EVENT_NUMBER];		/*�����¼�ִ�еĴ���*/
	uint8_t 	eventResultFlag_t[EVENT_NUMBER];	/*�¼�*/
	uint8_t 	eventEDable_t[EVENT_NUMBER];
}app_Event;



void userSystemSetEventDone(uint8 event_id);
void userSystemHandle();
void userSystemEventInit();
void setEvent(uint8_t event_id,uint8_t event_edable,uint16_t event_ones_timer,uint8_t event_run_count);


#endif
