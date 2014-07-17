using System.Threading.Tasks;
using System.Web.Mvc;
using CQRS;
namespace OptimalEducation
{
    public class CommandBuilder : ICommandBuilder
    {
        //private readonly IDependencyResolver dependencyResolver;

        //public CommandBuilder(IDependencyResolver dependencyResolver)
        //{
        //    this.dependencyResolver = dependencyResolver;
        //}

        public async Task ExecuteAsync<TCommandContext>(TCommandContext commandContext) where TCommandContext : ICommandContext
        {
            await DependencyResolver.Current.GetService<ICommand<TCommandContext>>().ExecuteAsync(commandContext);
            //await dependencyResolver.GetService<ICommand<TCommandContext>>().ExecuteAsync(commandContext);
        }
    }
}
