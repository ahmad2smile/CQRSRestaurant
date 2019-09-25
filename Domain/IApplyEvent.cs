namespace Domain
{
    public interface IApplyEvent<in TEvent>
    {
        void Apply(TEvent e);
    }
}