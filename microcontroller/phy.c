
#include "reg52.h"
#include <string.h>
#include <stdarg.h>
#include <stdio.h>


#include "phy.h"
#if 0
uint8 code table[]={0xc0,0xf9,0xa4,0xb0,0x99,0x92,0x82,0xf8,0x80,0x90};
sbit P2_0 = P2^0;
sbit P2_1 = P2^1;
sbit P2_2 = P2^2;
sbit P2_3 = P2^3;




SEGMENT_DISPLAYER segmentDisplaysIndex = SEGMENT_DISPLAYER_1_ON;


void dealSegmentDisplays()
{

#if 1	
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
#endif
	}
}

#endif
