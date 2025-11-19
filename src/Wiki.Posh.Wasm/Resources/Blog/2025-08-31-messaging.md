# Cleanly de-coupling classes in client-side .NET applications using weak reference messengers 

Commonly, messaging is done with multi-cast delegates. "Subscribing" to such a delegate creates a "strong reference" between instances, meaning that so long as a recipient is subscribed, a sender can't be garbage collected and thus will remain in memory. This means you must remember to unsubscribe from each and every such delegate once your recipient is no longer needed (often in a disposer or finaliser), or risk creating memory leaks. It also requires the sender and recipient instance to have knowledge of one another, increasing coupling. 

Enter: the messenger. A messenger class' purpose is to handle the routing of messages between other classes. This means that a sender and recipient class don't need to be aware of each other - in fact, they can be invisible to one another, in some architectures. The IMessenger interface from the package `CommunityToolkit.Mvvm` defines a set of behaviours required for messaging. 

Luckily, we don't need to implement messaging ourselves. The community toolkit defines a few implementations for us. Specifically, we're interested in WeakReferenceMessenger. This messenger creates weak references between sender and recipient, meaning that the sender can become eligible for garbage collection even if recipients are subscribed to its events. 

## Getting a Messenger

First, let's get our hands on a messenger. It's possible to access a single, static instance using `WeakReferenceMessenger.Default`, but this isn't always the best idea, say, if you're making a Blazor server application, as it can mean that messages are passed between instances that aren't related to one another. A better option is to set it up in your dependency injection container: 

```csharp
services.AddScoped<IMessenger, WeakReferenceMessenger>();
```

Then, you can get inject the service however you'd normally do that with your container: 

```csharp
public class MySender 
{
    private readonly IMessenger _messenger;

    public MySender(IMessenger messenger) 
    {
        _messenger = messenger;
    }
}
```

## Declaring Messages

A message can be of any type. It's common to define a type for various categories of message. The package comes with a few, such as `ValueChangedMessage<T>` and `RequestMessage<T>`, but you can define your own with no constraints - no need for inheritance. The class doesn't even need a body! 

```csharp
public class MyMessage;
```

## Sending Messages

To send a messenger, use the `IMessenger.Send` method. 

```csharp
class MySender 
{
    // ...

    public void SendMessage() 
    {
        _messenger.Send(new MyMessage());
    }
}
```

## Receiving Messages (Verbosely)

You can receive a message with an IMessenger by subscribing to it using the `Register<T>` method. This takes two arguments: an object, the recipient, and a `T`, the message. 

```csharp
public class MyRecipient 
{
    private IMessenger _messenger;

    public MyRecipient(IMessenger messenger)
    {
        _messenger = messenger;
    }

    public void Init() 
    {
        _messenger.Register<MyMessage>(this, (r, m) => 
        {
            // recipient logic goes here!
        });
    }
}
```

Of course, instead of using a lambda expression, you can extract this logic into its own named method: 

```csharp
public void Init() => _messenger.Register<MyMessage>(this, OnMyMessageReceived);

private void OnMyMessageReceived(object recipient, MyMessage message)
{
    // recipient logic goes here!
}
```

One may ask themself, why is the first argument the recipient? Why not just reference `this`? The answer is because it avoids a closure. Using `this` within the method captures the variable, which is not performant; passing a reference to it is faster. 

## Receiving Messages (Cleanly)

There's a slightly cleaner way to receive messages, using the `IRecipient<T>` interface. It forces you to define a method of the signature `void Receive<T>(object r, T m)`. You'll still need to subscribe to it, but you won't need to pass the name of the method. The drawback of this is that the method must be public, which enables other classes to call it. 

```csharp
public class MyRecipient : IRecipient<MyMessage>
{
    private IMessenger _messenger;

    public MyRecipient(IMessenger messenger) 
    {
        _messenger = messenger;
    }

    public void Init() 
    {
        _messenger.Register<MyMessage>(this);
    }

    public void Receive<MyMessage>(object recipient, MyMessage message) 
    {
        // recipient logic goes here! 
    }
}
```

## Using Channels

If you need to categorise messages beyond their types, but don't want to manage new scopes for your `IMessenger`s, channels can help. You can think of a channel like a specific group within a single messenger, or as automatically handled metadata attached to a message once it's sent. 

Much like a message, a channel can be of any type. Perhaps the cleanest way is to manage your channels in an enum: 

```csharp
public enum MessengerChannels 
{
    Api,
    Auth,
    Ui,
    // ...
}
```

Then, you can pass an instance of this type when sending and receiving messages: 

```csharp
_messenger.Send(new MyMessage(), MessengerChannels.Auth);

// ... 

_messenger.Register<MyMessage>(this, MessengerChannels.Auth, OnMyMessageReceived);
```

This means that only senders and recipients listening to that channel will be aware of that message. This saves you having to attach a `Channel` property to your messages and check it upon consumption. 