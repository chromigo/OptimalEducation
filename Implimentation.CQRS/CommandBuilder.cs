using System.Threading.Tasks;
using System.Web.Mvc;
using Interfaces.CQRS;
namespace Implimentation.CQRS
{
    public class CommandBuilder : ICommandBuilder
    {
        public async Task ExecuteAsync<TCommandContext>(TCommandContext commandContext) where TCommandContext : ICommandContext
        {
            await DependencyResolver.Current.GetService<ICommand<TCommandContext>>().ExecuteAsync(commandContext);
        }
    }
}
