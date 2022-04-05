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

**Processing** - Processesing of commands from the message queue, by invoking the appropriate command handlers.

### Command flow

**API endpoint** -> **Queue** -> **Processor** -> **Command handler** -> **Aggregate**

### Improvements

Following improvements would make sense:
- Expand the model to include order lines, products, and more.
- Real message queue with retries, listening, distributed consumers
- Real database (document db?)
- Enable idempotency on commands already handled, in case of more-than-once-delivery
- Handle error cases? Now, commands are just discarded if not able to execute (customer create attempted twice, order place on non-existing customer).
- Allow customers to change their e-mail address ;)
- Implement queries. Only commands are supported now.
- Use better JSON serialization on Id types
- Use / write a real framework for aggregate persistence.




