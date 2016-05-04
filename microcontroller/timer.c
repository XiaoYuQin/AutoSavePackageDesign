#include "reg52.h"
#include <string.h>
#include <stdarg.h>
#include <stdio.h>


#include "timer.h"
#include "user_sys.h"

//sbit LED1=P1^7;//声明端口

void Init_Timer0(void)		  //定时器初始化子程序
{
	TMOD |= 0x01;	  //使用模式1，16位定时器，使用"|"符号可以在使用多个定时器时不受影响		     
	TH0=65536-65036;	      //给定初值，这里使用定时器最大值从0开始计数一直到65535溢出
	TL0=65536-65036;
	EA=1;            //总中断打开
	ET0=1;           //定时器中断打开
	TR0=1;           //定时器开关打开
}


/*******************************************************************************
** 函数功能 ： 定时器中断程序
*******************************************************************************/

void Timer0_isr(void) interrupt 1 using 1
{
	TH0=65536-65036;	      //给定初值，这里使用定时器最大值从0开始计数一直到65535溢出
	TL0=65536-65036;
	//LED1=~LED1;        //指示灯反相，可以看到闪烁
	userSystemHandle();
}

/*------------------------------------------------
                    定时器初始化子程序
------------------------------------------------*/
/*void Init_Timer0(void)
{
 TMOD |= 0x01;	  //使用模式1，16位定时器，使用"|"符号可以在使用多个定时器时不受影响		     
 TH0=(65536-1000)/256;		  //重新赋值 2ms
 TL0=(65536-1000)%256;

 EA=1;            //总中断打开
 ET0=1;           //定时器中断打开
 TR0=1;           //定时器开关打开
}
		   */
/*------------------------------------------------
                    定时器初始化子程序
------------------------------------------------*/
void Init_Timer1(void)
{
 TMOD |= 0x10;	  //使用模式1，16位定时器，使用"|"符号可以在使用多个定时器时不受影响 
 TH1=0x55;	      //给定初值，这里使用定时器最大值从0开始计数一直到65535溢出
 TL1=0x55;
 EA=1;            //总中断打开
 ET1=1;           //定时器中断打开
 TR1=1;           //定时器开关打开
}
/*------------------------------------------------
                 定时器中断子程序
------------------------------------------------*/
void Timer1_isr(void) interrupt 3 using 1
{
	TH1=0x55;		 //给定初值，这里使用定时器最大值从0开始计数一直到65535溢出
	TL1=0x55;
}
/*------------------------------------------------
                    定时器初始化子程序
------------------------------------------------*/
void TIM2Inital(void)
{
  RCAP2H = (65536-10000)/256;//晶振12M 60ms 16bit 自动重载
  RCAP2L = (65536-10000)%256;
  ET2=1;                     //打开定时器中断
  EA=1;                      //打开总中断
  TR2=1;                     //打开定时器开关
}

/*------------------------------------------------
                 定时器中断子程序
------------------------------------------------*/
void TIM2(void) interrupt 5 using 1//定时器2中断
{
    TF2=0; 
	//P1 = ~P1;
	userSystemHandle();
}