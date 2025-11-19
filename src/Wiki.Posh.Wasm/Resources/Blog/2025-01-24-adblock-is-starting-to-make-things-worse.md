## Adblock is starting to make my experience worse.

<p>
    I'm a pretty big advocate for blocking ads. Part of the way I do this is using Proton VPN's
    NetShield. This means that while I'm connected to the internet via a VPN (which is always),
    my traffic gets sent via Proton's DNS servers, which won't resolve certain domains known for
    serving ads.
</p>

<p>
    Recently, I opened Twitter, a rare occurrence these days. I tried to click on a link (a
    review for a movie), only to be met with something showing that the domain couldn't be
    resolved. Twitter had changed the link, sending me via t [dot] co, which is blocked by
    Proton's NetShield.
</p>

<p>
    Given that the t [dot] co link is more like a bit [dot] ly link than a web.archive.org link,
    in that it doesn't directly encode the rest of the URL, there was no way for me to see the
    true URL that the post was pointing to. I couldn't copy it from the post or Twitter's
    built-in browser. There was no way for me to get to that link without disabling my
    NetShield.
</p>

<p>
    It would be sort of reasonable for Twitter to do this in order to discourage the use of
    adblock. The thing is, I still see ads on Twitter. This may be because Proton's NetShield
    doesn't catch the domain that serves them, or, more likely, they're served from the same
    domain as posts, making blocking them imply blocking core functionality.
</p>

<p>
    The same is true of Reddit. Ads still make their way onto my feed, through NetShield and
    uBlock Origin, and in the incredibly rare case I actually click on one, I can't get through
    to the URL due to being sent through ad [dot] doubleclick [dot] net.
</p>

<p>
    Unfortunately, I can't split tunnel an app out of NetShield without split tunneling it out
    of VPN entirely (i.e., I can't configure different DNS providers for different apps). That
    would be a nice feature to have.
</p>

<p>
    In short, using adblock on certain sites has not at all reduced the number of ads I see, but
    it has prevented me from getting through to external content promoted on those sites, even
    content promoted naturally.
</p>