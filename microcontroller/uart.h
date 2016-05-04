#ifndef _UART_H_
#define _UART_H_
#include "type.h"
#include "reg52.h"

typedef enum
{
	B19200,
	B57600
}UART_BUARD;


void InitUART(UART_BUARD buard);
void uartSendByte(unsigned char dat);
void uartSendStr(unsigned char *s);
void uartSendForStr(unsigned char *s,unsigned int len);
void uartDataClean();
//void debug(uint8 *fmt ,...);

#endif
