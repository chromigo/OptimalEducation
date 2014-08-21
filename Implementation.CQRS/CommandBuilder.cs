using System.Threading.Tasks;
using Interfaces.CQRS;
namespace Implementation.CQRS
{
    public class CommandBuilder : ICommandBuilder
    {
        public async Task ExecuteAsync<TCommandContext>(TCommandContext commandContext) where TCommandContext : ICommandContext
        {
            await DependencyResolver.Current.GetService<ICommand<TCommandContext>>().ExecuteAsync(commandContext);
        }
    }
}
