---
title: A Developer's Rant About Microsoft Teams 
---

<p>
    I often complain that for my job, I have to use Windows. Even more unfortunate is that my
    company is 100% into the Microsoft ecosystem - teams, office, Azure DevOps, the lot of it.
</p>

<p>
    Teams in particular causes me many problems. It crashes multiple times per day. I used to
    have a way to restart it when it did, but an update broke that a long time ago.
</p>

<p>
    Today, I decided to try my hand at it again... And I sure had a time doing it! Because this
    is a rant and not a story, I'll give you some of the technical details up front. This took
    some trial and error to figure out.
</p>

<p>
    Per <code>locate *.exe | grep '[Tt]eams'</code>, there are 13 Teams-related executables on
    my PC.
    3 are in %APPDATA%, with one being the installer for Teams and the other 2 being largely
    irrelevant Visual Studio extensions. The other 10 are under %USERPROFILE%, with only two not
    immediately exiting with an error code: ms-teams.exe and msteams.exe. The former is
    Microsoft Teams. A few months ago I was encouraged to upgrade to the latter, The New Teams.
    You'd think that they would be named more distinctly, but what matters is, we care about
    ms-teams.exe.
</p>

<p>
    Another thing to understand is that the service of Teams involves many processes. The UI is
    just one - if you close it (and have it configured to remain open, as we at my office are
    required to do), only the UI task ends. Some of the crashes I experience are UI-only, while
    others also crash the background tasks. Either way, grepping over the tasklist command for
    [Tt]eams is not an effective method of detemining whether the UI is open.
</p>

<p>
    Initially, I wanted to use Bash. I'm more familiar with it and generally prefer its syntax
    and behaviour over other shells. Unfortunately, it seems like the best tool for the job here
    was Powershell. If I've violated some convention, don't @ me, I have no respect for this
    language.
</p>

<p>
    If I want to have a Teams window open at all times, the first logical step is to identify if
    the problematic state currently exists. To do this, I used this horrible little construction
    to detect if any processes had a window whose title contained "Teams". If there's no
    problem, there's nothing for us to do, so let's exit the program gracefully.
</p>

<pre><code>$teamsWindows = Get-Process -Name *Teams* | ? MainWindowTitle 
If ($teamsWindows -ne $null) 
{ 
    exit 0 
}</code></pre>

<p>
    The next part is, well, opening Teams. No big deal! Watch this.
</p>

<pre><code>Start-Process ms-teams.exe </code></pre>

<p>
    Easy. The only problem is that this immediately focuses the Teams window, which I don't want
    to happen while quietly reviving it without interrupting my workflow. No big deal, right?
</p>

<p>
    ...
</p>

<p>
    Right?
</p>

<p>
    As it turns out, yes, it is a big deal. Passing -WindowStyle Minimized to Start-Process
    should in theory start a window minimised... But never mind that the window is still
    focused, this doesn't even work for Teams! ms-teams.exe lives only briefly, starts a child
    process of the teams UI, and exits. The new window immediately gains focus, and I couldn't
    find any CLI flags to prevent this, even though Teams itself has the option to open in the
    background. (XML file under user home?)
</p>

<p>
    So, a hacky solution. If we're going to open Teams, first grab the window handler (HWND) of
    the current window, then open Teams, and finally, re-focus the original HWND. I happen to
    know that we can accomplish such things with user32.dll.
</p>

<pre><code>Add-Type @"using System; 
using System.Runtime.InteropServices; 

public class User32 
{ 
    [DllImport("user32.dll")] 
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")] 
    public static extern bool SetForegroundWindow(IntPtr hWnd); 
}"@ 

$hWnd = [User32]::GetForegroundWindow() 

Start-Process ms-teams.exe -Wait 

[User32]::SetForegroundWindow($hWnd) | Out-Null</code></pre>

<p>
    Also available in the <a href="https://posh.wiki/scriptorium.html">Scriptorium</a>.
</p>

<p>
    Using -Wait is a little slower than using Start-Sleep -Milliseconds 200, but 200ms won't
    work for every machine and environment. The teams window appears briefly, but, this is good
    enough.
</p>

<p>
    Except.
</p>

<p>
    ms-teams only exits quickly if you're only starting the UI. If the services were off, it'll
    keep going, never focusing the previous window. Interestingly, if you kill that process,
    Teams UI persists, but if you close the Teams window, the process ends, and the services
    continue to run just fine. Just... What?
</p>

<p>
    Technically this is fine. It just results in an edge case where if the teams services have
    crashed recently, and I close Teams UI, AND the window I was using while it crashed is still
    open or another window now has its HWND, the wrong window MIGHT be focused after I close
    Teams.
</p>

<p>
    Still though, just... What?
</p>