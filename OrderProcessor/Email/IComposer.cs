namespace OrderProcessor.Email;

public interface IComposer
{
    Email Do(Parameters parameters);
}