#include <reg52.h>	
#include "1602.h"
#include <intrins.h>


sbit rs=P0^7;
sbit rw=P0^6;
sbit ep=P0^5;



void delay (int m)
{
	unsigned char i,j;
	for (i=0;i<m;i++)
	for (j=0;j<253;j++);
}

bool lcd_bz()
{
	bool result;
	rs=0;           // ?ив?|D?o?
	rw=1;
	ep=1;
	_nop_();
	_nop_();
	_nop_();
	_nop_();
	result = (bool)(P2&0x80);
	ep=0;
	return result;
}

void lcd_wcmd (uint8 cmd)
{
	while (lcd_bz());
	rs=0;
	rw=0;
	ep=0;
	_nop_();
	_nop_();
	P2=cmd ;
	_nop_();
	_nop_();
	_nop_();
	_nop_();
	ep=1;
	_nop_();
	_nop_();
	_nop_();
	_nop_();
	ep=0;
}

void lcd_pos (uint8 pos)
{
	lcd_wcmd (pos|0x80);
}

void lcd_wdat (uint8 dat)
{
	while (lcd_bz());
	rs=1;
	rw=0;
	ep=0;
	_nop_();
	_nop_();
	P2=dat ;
	_nop_();
	_nop_();
	_nop_();
	_nop_();
	ep=1;
	_nop_();
	_nop_();
	_nop_();
	_nop_();
	ep=0;
}

void lcd_init ()
{
	lcd_wcmd (0x38);
	delay (1);
	lcd_wcmd (0x0c);
	delay (1);
	lcd_wcmd (0x06);
	delay (1);
	lcd_wcmd (0x01);
	delay (1);
	delay (10);
	lcd_pos (0);
}