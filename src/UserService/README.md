# User Service

Provider of Messaging APIs:

1) Publishes `UserEvent`
2) Conumes `CreateUserCommand`


## Questions


1) Support for AsyncAPI (e.g. using Saunter)

See <https://localhost:7198/asyncapi/ui/index.html> and <https://localhost:7198/asyncapi/asyncapi.json>.

What would be nice:
- Minimal: detect Event types that are published and Command types that are consumed (i.e. messaging endpoints owned by this endpoint)
- Attributes to add additional documentation to be included in AsyncAPI spec file