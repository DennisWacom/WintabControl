# WintabControl

This is a simple demonstration in C# WPF on how to use the WintabDotNet project to retrieve the pen data, also highlighting the difference between a system context and digitizer context. WintabDotNet allows you to use C# with Wintab, to get pen data from a pen tablet/display, which supports the wintab protocol. 

Basically, digitizer context will return the pen data based on the values in the digitizer unit, and system context will convert the digitizer values automatically to screen resolution. 

## Prerequisite

Wintab Driver (Any pen tablet driver which supports wintab, e.g. Wacom tablet driver)
[WintabDotNet from Nuget](https://www.nuget.org/packages/WacomSolutionPartner.WintabDotNet/)
Physical pen tablet or pen display

