#ifndef _1602_H_
#define _1602_H_
#include "type.h"



void delay (int m);
bool lcd_bz();
void lcd_wcmd (uint8 cmd);
void lcd_pos (uint8 pos);
void lcd_wdat (uint8 dat);
void lcd_init ();


#endif

