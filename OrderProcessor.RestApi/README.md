# Order Processor

Order Processor exposes an API, through which users and systems can create customers and place orders for a customer. 

E-mail is used as customer identifier.

## Usage

Simply run the **OrderProcessor.RestApi** project and invoke the API endpoints via Swagger UI.

Observe the log statements in the Console window.

### Namespaces

**Commands** - Models the commands that can be sent to the system.

**Domain** - Logic modelling customers, levels and orders, modelled as DDD aggregates and entities. Validates and executes commands.

**Email** - Composes and sends e-mails on placed orders.

**Handlers** - Single points of entry for handling issued commands.

**Messaging** - Message queueing for routing commands to the processor.

**Persistence** - Repository and framework for loading/saving aggregates and their state.

**Processing** - Processes commands by listening (polling, in this case) the message queue, and invokes the appropriate command handlers.

### Command flow

**API endpoint** -> **Queue** -> **Processor** -> **Command handler** -> **Aggregate**

### Improvements

Following improvements would make sense:
- Real message queue with retries, listening, distributed consumers
- Real database (document?)
- Idempotency on commands already handled
- Allow customers to change their e-mail address ;)
- Implement queries. Only commands are supported now.
- Use better JSON serialization on Id types in Swagger UI / ASP.NET
- Use / write a real framework for aggregate persistence.




