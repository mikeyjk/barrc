'barrc'

The intention here is to have configuration files describing what arguments to provide to bar.
It intends to have features such as automatic handling of using named pipes (common way of using bar).

I chose C# because I wanted a really simple intro to the language prior to my final semester of university.
Bash is probably more appropriate.
This application requires the Mono C# application and compiler.

This is incredibly rough around the edges and currently I wouldn't recommend anyone uses it.

It reads from ~/.config/barrc/barrc , to determine the number of bars, if there is a named pipe to read from, what fonts and colors to be used etc. Haven't added the handling of geometry yet.
