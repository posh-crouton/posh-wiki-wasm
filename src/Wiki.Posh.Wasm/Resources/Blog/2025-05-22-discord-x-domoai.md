---
title: Discord X DomoAI: The Full Story (+ Advice)
---

Recently, you might have seen some posts or messages about Discord partnering with a service called DomoAI. Many such posts claim that DomoAI is a bot "hiding" in Discord servers and using the images sent there to train AI, or something similar - let's break down what's *actually* happening, what's fact, and what's speculation. 

## TL;DR (at the top)

* Discord **has not** officially endorsed DomoAI. 
* DomoAI **can't** see an image unless a user specifically tells it to. 
* DomoAI **claims** they don't store your images or use them to train AI models. 
* The **only** effective deterrents are disabling external apps (for server admins) and poisoning your works (for artists). 

## Is this even new? 

First, let's add some context. DomoAI isn't even a new thing. The DomoAI discord integration has been active since 20 September, 2023, and a DomoAI [article](https://domoai.app/blog/domoai-x-discord-our-image-restyle-app-now-live-in-the-discord-app-directory) talking about the app was released some time before 18th December, 2024 (the first time the wayback machine archived the page), having been updated a few times since. 

## Discord is *not* "partnering" with DomoAI. 

Let's find out if Discord has released any statements declaring association with or even endorsement of DomoAI. 

A quick [Google search](https://www.google.com/search?q=%22domoai%22+site%3Adiscord.com) reveals that Discord has a server for DomoAI, and an "app" in its app directory - and that's the only two times DomoAI is mentioned on discord.com. There are no official pages discussing any business affiliation with DomoAI. Additionally, Discord has not posted about DomoAI on any of its social media platforms. 

This lack of public disclosure suggests that Discord Inc. has not partnered with or otherwise endorsed DOMOAI PTE.LTD. Just because Discord platforms DomoAI does not mean that they endorse it or are affiliated with it in any significant way. 

## What does DomoAI say about how they use images? 

Some time on or before 18 December 2024, DomoAI released an [article](https://domoai.app/blog/domoai-x-discord-our-image-restyle-app-now-live-in-the-discord-app-directory) entitled "DomoAI x Discord: Redefining Creativity with Image Restyling in Every Chat and Server". 

That title might be a little misleading. "DomoAI x Discord" implies a relationship between Discord and DomoAI that has not been shown to exist - as far as we can tell, DomoAI is a plugin ("app") like any other (examples including Wordle, YouTube, Gartic Phone, and many more integrations you might use on a daily basis.)

Within this article, they state: 

> DomoAI does not access or use any image unless you explicitly choose it through the "Edit with Apps" option in Discord.

This claim is consistent with the capabilities of the "app". Due to limitations on what any such service can do, it is not possible for DomoAI to "see" an image unless a user specifically requests that it do so. 

> We do not train on your artwork or images, and no content is stored after generation is complete.

We can't exactly prove this. Images processed by the DomoAI app must be sent to DomoAI's servers for processing, and while they *say* they don't store them, we can never be 100% sure they're telling the truth. 

> Your creations remain 100% yours. We want to assure you that your privacy and creative rights are fully respected.

Again, this is down to how much you trust DomoAI to be honest. They say they're not doing this, but you can never know for sure.

## Apps Vs Bots: DomoAI is not "hiding" in your servers or reading your messages. 

Discord terminology can be quite confusing, but there's an important difference between the kind of app DomoAI is, and the kind of app you might refer to as a "bot". 

Bots have been supported in Discord for a long time. They appear like members of your server, showing up in the user list, and depending on the permissions given by the server administrator, they may or may not be able to do most of the things a real person could do.

DomoAI isn't quite the same - it's an "App". While you *can* manually add it to a server, you can also add it to your account, and use it in any server. Either way, it only has access to any given message if a user explicitly grants it permission for that message in particular. This is a technical limitation imposed by Discord. 

You can verify on Discord's side that DomoAI can't read messages without explicit permission [here](https://discord.com/discovery/applications/1153984868804468756) - when adding it to an account or server, Discord disclaims: 

> This application **cannot** read your messages or send messages as you". 

## So what *can* it do? 

The DomoAI app can restyle images and generate videos from still images or prompts. That's pretty much it. 

Depending on server settings, the generated content may be visible only to the user who requested it. 

## I'm an artist, what can I do to protect my art? 

If you share an image online, it could be processed by or used to train AI - all it takes is for the wrong person to get their hands on it. That didn't start with DomoAI, and it won't end there either. 

If you want to continue to share your art publically, but are afraid of it being used to train AI or transformed without your consent, you might consider "poisoning" it. This means running it through a filter that is invisible to the human eye, but causes AI to be unable to properly process it, for either training or editing. 

[The University of Chicago](https://nightshade.cs.uchicago.edu/userguide.html) shows us how to use Nightshade, a tool for poisoning images against AI, as well as discussing its risks and limitations. 

## I'm a server owner, what can I do to protect my members? 

If you want to prevent users in your server from using DomoAI, the nuclear option is to disable the "Use External Apps" permission for your users. Bear in mind that this will prevent them from using *all* external apps - though they'll still have access to any that you've added to your server yourself. 

**You may have seen some suggestions to ban certain IDs from your server. This has no effect - users can still invoke DomoAI commands.**

## TL;DR (at the bottom)

* Discord **has not** officially endorsed DomoAI. 
* DomoAI **can't** see an image unless a user specifically tells it to. 
* DomoAI **claims** they don't store your images or use them to train AI models. 
* The **only** effective deterrents are disabling external apps (for server admins) and poisoning your works (for artists). 