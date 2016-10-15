using System.Diagnostics.CodeAnalysis;

namespace DisplayPi.LCD1602
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]

    public sealed partial class LcdDisplay//.Constants
    {
        //commands
        private const int LCD_CLEARDISPLAY = 0x01;
        private const int LCD_RETURNHOME = 0x02;
        private const int LCD_ENTRYMODESET = 0x04;
        private const int LCD_DISPLAYCONTROL = 0x08;
        private const int LCD_CURSORSHIFT = 0x10;
        private const int LCD_FUNCTIONSET = 0x20;
        private const int LCD_SETCGRAMADDR = 0x40;
        private const int LCD_SETDDRAMADDR = 0x80;

        //flags for display entry mode
        private const int LCD_ENTRYRIGHT = 0x00;
        private const int LCD_ENTRYLEFT = 0x02;
        private const int LCD_ENTRYSHIFTINCREMENT = 0x01;
        private const int LCD_ENTRYSHIFTDECREMENT = 0x00;

        //flags for display on/off control
        private const int LCD_DISPLAYON = 0x04;
        private const int LCD_DISPLAYOFF = 0x00;
        private const int LCD_CURSORON = 0x02;
        private const int LCD_CURSOROFF = 0x00;
        private const int LCD_BLINKON = 0x01;
        private const int LCD_BLINKOFF = 0x00;

        //flags for display/cursor shift
        private const int LCD_DISPLAYMOVE = 0x08;
        private const int LCD_CURSORMOVE = 0x00;

        //flags for display/cursor shift
        private const int LCD_MOVERIGHT = 0x04;
        private const int LCD_MOVELEFT = 0x00;

        //flags for function set
        private const int LCD_8BITMODE = 0x10;
        private const int LCD_4BITMODE = 0x00;
        private const int LCD_2LINE = 0x08;
        private const int LCD_1LINE = 0x00;
        private const int LCD_5x10DOTS = 0x04;
        private const int LCD_5x8DOTS = 0x00;

    }
}
