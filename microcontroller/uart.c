#include "reg52.h"
#include <string.h>
#include <stdarg.h>
#include <stdio.h>
#include "type.h"

#include "uart.h"

#define UARTDATA_LEN 20

uint8 uartData[UARTDATA_LEN];
uint8 uartIndex;
/*------------------------------------------------
					 串口初始化
 ------------------------------------------------*/

 void InitUART(UART_BUARD buard)
 {

	 SCON  = 0x50;				 // SCON: 模式 1, 8-bit UART, 使能接收	
	 TMOD |= 0x20;				 // TMOD: timer 1, mode 2, 8-bit 重装
	if(buard == B19200)
	{
		TH1 = 0xFD;				 // TH1:  重装值 9600 波特率 晶振 11.0592MHz  
		TL1 = 0xFD;
	}
	else if(buard == B57600)
	{
		TH1 = 0xFF;				 // TH1:  重装值 9600 波特率 晶振 11.0592MHz  
		TL1 = 0xFF;	
	}
	TR1   = 1; 				 // TR1:  timer 1 打开						   
	SM0=0;			 //设置串行口工作方式
	SM1=1;
	REN=1;         	//允许串行接收位
	EA=1;          	//开启总中断
	ES=1;			//开启串口中断

#if 0
	 SCON  = 0x50;				 // SCON: 模式 1, 8-bit UART, 使能接收	
	 TMOD |= 0x20;				 // TMOD: timer 1, mode 2, 8-bit 重装
	 TH1   = 0xFD;				 // TH1:  重装值 9600 波特率 晶振 11.0592MHz  
	 TL1=0xfd;
	 TR1   = 1; 				 // TR1:  timer 1 打开						   
	SM0=0;			 //设置串行口工作方式
	SM1=1;
	REN=1;         	//允许串行接收位
	EA=1;          	//开启总中断
	ES=1;			//开启串口中断
	 //ES	 = 1;				   //打开串口中断
#endif
#if 0
	 SCON  = 0x50;				 // SCON: 模式 1, 8-bit UART, 使能接收	
	 TMOD |= 0x20;				 // TMOD: timer 1, mode 2, 8-bit 重装
	 TH1   = 0xFF;				 // TH1:  重装值 9600 波特率 晶振 11.0592MHz  
	 TL1=0xFF;
	 TR1   = 1; 				 // TR1:  timer 1 打开						   
	SM0=0;			 //设置串行口工作方式
	SM1=1;
	REN=1;         	//允许串行接收位
	EA=1;          	//开启总中断
	ES=1;			//开启串口中断
	 //ES	 = 1;				   //打开串口中断
#endif

 }

 
 /*------------------------------------------------
					 发送一个字节
 ------------------------------------------------*/
 void uartSendByte(unsigned char dat)
 {
	ES=0;
	SBUF = dat;
	while(!TI);
		TI = 0;
	ES=1;			//打开中断
 }
 /*------------------------------------------------
					 发送一个字符串
 ------------------------------------------------*/
 void uartSendStr(unsigned char *s)
 {
  while(*s!='\0')// \0 表示字符串结束标志，
				 //通过检测是否字符串末尾
   {
   uartSendByte(*s);
   s++;
   }
 }
void uartSendForStr(unsigned char *s,unsigned int len)
{
	int i;
	for(i=0;i<len;i++)
	{
		uartSendByte(s[i]);
	}
}
void uartRecive() interrupt 4
{
	char a;
	RI=0;			//接收到数据后，将RI置0
	a=SBUF;			//接收数据
	uartData[uartIndex] = a;
	uartIndex++;
	if(uartIndex > UARTDATA_LEN-1)	uartIndex = 0;
}
void uartDataClean(){
	uint8 i;
	for(i = 0;i<UARTDATA_LEN;i++){
		uartData[i] = 0;
	}
	uartIndex = 0;
}
#if 0
void debug(uint8 *fmt ,...)
{

	va_list arg_ptr;
	uint8	 LocalText[64];
	uint8	 cnt;
	uint8	 m;
	for(cnt=0 ; cnt<64 ; cnt++) 
	{
		LocalText[cnt] = 0x00;
	}

	va_start(arg_ptr, fmt);
	vsprintf(LocalText, fmt, arg_ptr);
	va_end(arg_ptr);
	for(m=0 ; m<64 ; m++) 
	{
		if(LocalText[m]==0x00)
		{
			break;
		}
	}
	uartSendForStr(LocalText,m);

}
#endif	