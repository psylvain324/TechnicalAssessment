namespace TechnicalAssessment.Services.Interfaces
{
    public interface ICommandHandler<out TReturn>
    {
        TReturn Handle();
    }
}
