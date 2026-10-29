using Mia_Web.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Mia_Web.Application.TodoItems.EventHandlers;

public class LogTodoItemCompleted : INotificationHandler<TodoItemCompletedEvent>
{
    private readonly ILogger<LogTodoItemCompleted> _logger;

    public LogTodoItemCompleted(ILogger<LogTodoItemCompleted> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Mia_Web Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
