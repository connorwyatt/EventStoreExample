namespace ConnorWyatt.EventStoreExample.Shared.Events;

public record EventEnvelope<T>(T Event, EventMetadata Metadata) where T : class, IEvent;
