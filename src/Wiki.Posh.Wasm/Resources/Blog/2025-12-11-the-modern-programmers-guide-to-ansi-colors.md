---
title: The Modern Programmer's Guide To ANSI Color Codes
---

As a user of Arch (btw) and neovim, I spend quite a bit of time in the terminal. I'm also most comfortable developing CLIs, rather than TUIs or GUIs or even APIs. 

Contrary to what some people may believe, the terminal doesn't have to be all black and white (or black and green, if you're a totally l33t h4x0r). Some terminal emulators support up to 16,581,375 unique colours, and some neat font features too! It's just a matter of knowing how to use them. 

## Basic Control Sequence Syntax 

All we need in order to use the full graphical capabilities of our terminal emulator is a basic understanding of ANSI control sequences, which can be likened to functions you can call by presenting them on the terminal emulator. 

The first part of calling such a function is letting the terminal emulator know that what comes next isn't just arbitrary text. We do this with the Control Sequence Indicator (CSI), which is an ASCII escape control character followed by `[`. We can't exactly type an escape character using a standard keyboard, so instead we use `\033`, `\x1b`, or simply `\e`. 

Next up come the arguments (yep, before the function name!). Functions take 0 or more arguments, which are decimal-encoded unsigned 8-bit integers, suffixed with `;`. We'll get to the different arguments you can pass in just a moment. 

The final part of our function's invocation is the name. These are generally letters. We're only concerned with the SGR (Select Graphics Rendition) function, which is &lt;sarcasm&gt;*oh-so-clearly named*&lt;/sarcasm&gt; `m`. 

So, the simplest invocation of the SGR function we can come up with is `\e[0m`, which will reset all graphics features to their defaults. 

## Colouring Text &amp; Backgrounds  

You can achieve some very basic text colouring with arguments 30-37. Each of these values represents a named colour, as shown in the table below. It's down to the terminal emulator, and often configurable by the user, which specific colours these show up as. 

| Value | Name |
|---|---|
| 30 | Black |
| 31 | Red |
| 32 | Green |
| 33 | Yellow |
| 34 | Blue |
| 35 | Purple |
| 36 | Cyan |
| 37 | White |

You can perform some arithmetic on these numbers to brighten the colours or use them as the background instead of the foreground. Note that the bright variants are non-standard, and were originally implemented by [aixterm](https://invisible-island.net/xterm/ctlseqs/ctlseqs.html).

| Operation | Effect |
|---|---|
| +10 | Use colour as background |
| +60 | Use bright variant |
| +70 | Use bright variant as background |

Colours can be taken a step further. Arguments `38;5;n` (for some byte `n`) opens up a range of 255 colours, while arguments `38;2;r;g;b` brings out all 16,581,375 RGB colours. Arguments `48;5;n` and `48;2;r;g;b` can also be used to apply these colours as background. 

## Text Decorations 

Various other values can be passed as a single argument to the SGR function to achieve a variety of decorative effects. Note that not all terminal emulators support all features, so your mileage may vary with these. 

| Value | Effect | Reversed By |
|---|---|---|
| 1 | Bold | 0, sometimes 21 |
| 2 | Dim | 22 |
| 3 | Italic | 23 |
| 4 | Underline | 24 |
| 5 | Slow blink | 25 |
| 6 | Fast blink | 25 |
| 7 | Invert colours | 27|
| 8 | Invisible | 28 |
| 9 | Strikethrough | 29 |
| 10 | Default font | | 
| 11-19 | Alternative fonts 1-9 | 10 |
| 20 | Fraktur, sometimes reverse bold (1) | 50 |
| 21 | Double-underline, sometimes reverse bold (1) | 24 |
| 26 | Proportional spacing (unused in terminals) | 50 |
| 39 | Default foreground colour | |
| 49 | Default background colour | |
| 51 | Framed | 54 |
| 52 | Encircled | 54 |
| 53 | Overlined | 55 |
| 58 | Set underline colour | 59 |
| 60 | Ideogram underline / right line | 65 |  
| 61 | Ideogram double underline / double right | 65 |  
| 62 | Ideogram overline / left line | 65 |  
| 63 | Ideogram double overline / double left | 65 |  
| 64 | Ideogram stress marking | 65 |  
| 73 | Superscript | 75 |
| 74 | Subscript | 75|

Most terminal emulators don't support Fraktur, a heavy calligraphic hand of the Latin alphabet, or blinking. A limited number support double-underline, frames, encircles, ideograms, and super/subscript. 

## Recommended Resources

I certainly wouldn't recommend spending 221 of your own Swiss Francs on ISO/IEC 6429, or even 177 of them on ISO/IEC 2022. And I'll be honest, ECMA 48 is a little dry, and JIS X 0211 might be a little difficult for the average reader of this article. 

I *would* recommend checking out [fidion/ansi](https://github.com/fidion/ansi) to check your terminal's compatibility with a variety of features. Also check out [JBlond's gist](https://gist.github.com/JBlond/2fea43a3049b38287e5e9cefc87b2124), which was a great starting point for the research that went into this article. Finally, check the [Wikipedia page for ANSI escape codes](https://en.wikipedia.org/wiki/ANSI_escape_code), where you can learn to manipulate the cursor for so much finer control than you'll get by printing `\r` at the end of your lines. 