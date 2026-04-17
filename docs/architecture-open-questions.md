# Architecture Open Questions

## 0. Fundamental architectural constraint

- This library is a domain-specific facade over an existing internal messaging library.
- Infrastructure concerns may already be handled by the underlying layer.
- Unknown infrastructure behavior must be treated as "possibly handled below" until confirmed.
- Delegation is preferred over reimplementation.

## 1. System assumptions (current understanding)

- Custom in-house broker exists.
- Messaging is publish-only (no request/reply).
- Two message categories exist:
  - Events (business facts).
  - Data/Snapshot messages (full or partial data transfer).
- Payload is always serialized to string.
- Payload may be compressed and/or encrypted.
- System is event-choreographed (event chaining exists).
- Layer 1 already provides generic messaging infrastructure capabilities.
- Layer 2 (this project) should stay thin and focused on domain abstractions and DX.

## 2. Open architectural questions

### Message delivery guarantees
- What delivery guarantees does the broker provide? (at-most-once / at-least-once / exactly-once / unknown)
- Is delivery/retry behavior fully implemented in the underlying messaging library?

### Idempotency
- Is message deduplication required at consumer level?
- Is there a global message id or deduplication key?
- Does Layer 1 already provide deduplication primitives that this facade should expose instead of re-implementing?

### Ordering guarantees
- Are messages ordered per key, per topic, or not guaranteed at all?
- Is ordering behavior configured below and only documented/mapped in this facade?

### Versioning strategy
- How are events versioned? (header / payload / both / none)
- Are breaking changes allowed or forbidden?
- Does Layer 1 expose versioning headers/contracts that must be reused by this layer?

### Snapshot / data messages behavior
- Are snapshots full state or incremental deltas?
- Are snapshots pushed periodically or event-driven?
- Are snapshots idempotent by nature?
- Which snapshot semantics are domain conventions (Layer 2) vs transport mechanics (Layer 1)?

### Retry and failure handling
- Are retries handled by broker or consumer?
- Is poison message handling required?
- Is this fully owned by Layer 1 and out of scope for Layer 2 implementation?

### Security model
- Who is responsible for encryption/decryption?
- Is encryption mandatory or optional per message type?
- Is security pipeline behavior configured in Layer 1 and only toggled by Layer 2 metadata?

### Payload constraints
- Expected max payload size for events vs data messages?
- Are there streaming requirements for large payloads?
- Are size limits and streaming mechanics enforced below and only surfaced as constraints in this facade?

### Consumer model
- Can multiple consumers process the same event independently?
- Is there competing consumer model or fan-out model?
- Which consumer semantics are fixed by the underlying broker integration vs configurable by this facade?

## 3. Working assumptions (temporary defaults)

- ASSUMPTION - TO BE CONFIRMED: Broker likely uses at-least-once delivery.
- ASSUMPTION - TO BE CONFIRMED: Consumers are responsible for idempotency.
- ASSUMPTION - TO BE CONFIRMED: Ordering is not guaranteed globally.
- ASSUMPTION - TO BE CONFIRMED: Snapshots likely represent full state transfers.

## 4. Impact on design (important section)

### Delivery guarantees
- Why it matters: Determines retry policy, acknowledgment strategy, and duplicate handling baseline.
- Affected components: Facade publish contract, envelope metadata mapping, capability exposure in DX API.

### Idempotency
- Why it matters: Prevents duplicate side effects in at-least-once delivery scenarios.
- Affected components: Envelope metadata conventions, deduplication key mapping, consumer guidance contracts.

### Ordering guarantees
- Why it matters: Defines whether sequence-dependent processing is safe by default.
- Affected components: Ordering metadata conventions, key selection guidance, API documentation constraints.

### Versioning strategy
- Why it matters: Controls forward/backward compatibility and rollout safety.
- Affected components: Envelope version metadata, domain message contracts, compatibility conventions.

### Snapshot/data message behavior
- Why it matters: Changes merge semantics, conflict handling, and replay behavior.
- Affected components: Message classification model, snapshot semantics in API, consumer-side interpretation guidance.

### Retry and failure handling
- Why it matters: Impacts resilience policy, dead-letter strategy, and observability requirements.
- Affected components: Capability mapping boundaries, error metadata exposure, non-goals documentation.

### Security model
- Why it matters: Defines trust boundaries and cryptographic responsibilities.
- Affected components: Security-related envelope metadata, configuration pass-through abstractions, responsibility boundaries.

### Payload constraints
- Why it matters: Impacts transport efficiency, memory usage, and serialization choices.
- Affected components: Payload size conventions, validation at facade boundary, DX-level guidance.

### Consumer model
- Why it matters: Determines scalability and subscription semantics.
- Affected components: Subscription-facing abstractions, semantics documentation, integration assumptions with Layer 1.
