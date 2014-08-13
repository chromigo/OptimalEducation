using System.Threading.Tasks;
using System.Web.Mvc;
using CQRS;
namespace OptimalEducation
{
    public class CommandBuilder : ICommandBuilder
    {
        public async Task ExecuteAsync<TCommandContext>(TCommandContext commandContext) where TCommandContext : ICommandContext
        {
            await DependencyResolver.Current.GetService<ICommand<TCommandContext>>().ExecuteAsync(commandContext);
        }
    }
}
