using System;
using System.Collections;

namespace Domain
{
    public class Aggregate
    {
        public int EventsLoaded { get; private set; }

        public Guid Id { get; set; }

        public void ApplyEvents(IEnumerable events)
        {
            foreach (var e in events)
            {
                GetType().GetMethod("ApplyOnEvent")
                    ?.MakeGenericMethod(events.GetType())
                    .Invoke(this, new[] { e });
            }
        }

        public void ApplyOneEvent<TEvent>(TEvent e)
        {
            if (!(this is IApplyEvent<TEvent> applier))
            {
                throw new InvalidOperationException(
                    $"Aggregate {GetType().Name} does not know how to apply event {e.GetType().Name}");
            }

            applier.Apply(e);
            EventsLoaded++;
        }
    }
}
