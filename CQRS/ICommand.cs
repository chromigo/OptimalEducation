using System.Threading.Tasks;

namespace CQRS
{
    /// <summary>
    ///     Критерии запроса
    /// </summary>
    public interface ICommandContext { }

    public interface ICommand<in TCommandContext>
        where TCommandContext : ICommandContext
    {
        /// <summary>
        ///     Выполняет действия команды.
        /// </summary>
        /// <param name="commandContext">Контекст команды</param>
        Task ExecuteAsync(TCommandContext commandContext);
    }

    public interface ICommandBuilder
    {
        /// <summary>
        ///     Создает команду с определенным контекстом и выполняет её.
        /// </summary>
        /// <typeparam name="TCommandContext">Тип контекста команды.</typeparam>
        /// <param name="commandContext">Контекст команды.</param>
        Task ExecuteAsync<TCommandContext>(TCommandContext commandContext)
            where TCommandContext : ICommandContext;
    }
}
