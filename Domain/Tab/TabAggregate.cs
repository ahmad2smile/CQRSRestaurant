using Domain.Tab.Commands;
using Domain.Tab.Events;
using System.Collections;

namespace Domain.Tab
{
    public class TabAggregate : Aggregate, IHandleCommand<OpenTab>
    {
        public IEnumerable Handle(OpenTab command)
        {
            yield return new TabOpened
            {
                Id = command.Id,
                TableNumber = command.TableNumber,
                Waiter = command.Waiter
            };
        }
    }
}
