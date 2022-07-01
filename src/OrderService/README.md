# Order Service

Consumer of Messaging APIs of UserService:

1) Subscribes to `UserEvent` of `UserService`
2) Sends `CreateUserCommand` to `UserService`


## Questions

1) Attribute-based Command Routing (see `DestinationAttribute` and Routing setup). Ok like this, or somehow supported by NSB?
2) Marker interfaces from lightweight `NServiceBus.Contracts` nuget (zero-dependency)?

3) Main question:
Order Service uses classes `UserEvent` and `CreateUserCommand` of `UserService`.
With message type detection as described here <https://docs.particular.net/nservicebus/messaging/message-type-detection>, the `OrderService` is forced to use a type with a matching FullName.
i.e. it must use the C# contract of the `UserService` (copy source 1:1 or nuget).

What would be nicer:
```csharp

// In UserService (owner of the contract)
namespace UserService.Contracts;

[Event(EndpointName="UserService", EventName="UserEvent", Version="V1")]
class UserEvent
{
    // ...
}


// In OrderService (consumer)
namespace OrderService.ExternalContracts.UserService;

[Event(EndpointName="UserService", EventName="UserEvent", Version="V1")]
class UserEvent
{
    // ...
}

```

=> **Idea**: Use Event/CommandAttribute to define MessageType

- `<EndpointName>.<Event/CommandName>.<Version>` as type identifier
- Explicit Version to bake-in endpoint versioning
- EndpointName to express contract owner
- Use this name instead of fully qualified name (can still be used, where attribute is missing)