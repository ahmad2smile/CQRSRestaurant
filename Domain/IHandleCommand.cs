using System.Collections;

namespace Domain
{
    public interface IHandleCommand<in TCommand>
    {
        IEnumerable Handle(TCommand command);
    }
}
