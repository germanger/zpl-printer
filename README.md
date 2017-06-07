# zpl-printer
A desktop app for previewing and printing ZPL files through a Zebra. Made with .NET C#

## Features
- Print using the print dialog
- Preview using Labelary.com (requires Internet connection)
- Open ZPL files (manually or via file type association)

## Previewing

This project uses Labery.com API for generating a visual representation of the label from the ZPL code. So if you want to preview before printing, you will require an Internet connection. The API returns a PDF and it is displayed in a `System.Windows.Forms.WebBrowser`

## Printing

This project uses .NET's `System.Windows.Forms.PrintDialog`, as shown in these posts:
- https://stackoverflow.com/questions/2044676/net-code-to-send-zpl-to-zebra-printers/12842174#12842174
- https://support.microsoft.com/en-us/help/322091/how-to-send-raw-data-to-a-printer-by-using-visual-c-.net


## "Zebra ZPL II Utility"

There is this similar project from 2010: https://zebrazpliiutlity.codeplex.com/
but it has two problems:

- No preview
- It doesn't let you associate ZPL files ("open with...")

## Suggested usage

1. Associate .ZPL files to open with this utility
2. Create ZPL code from your own software and output to a file, ie. foo.zpl
3. In your software, use something like `System.Diagnostics.Process.Start("foo.ZPL")`
4. Windows will open the .ZPL with this utility

## Screenshots

![Previewing](https://raw.githubusercontent.com/germanger/zpl-printer/master/screenshot1.jpg)

![Printing](https://raw.githubusercontent.com/germanger/zpl-printer/master/screenshot2.jpg)



