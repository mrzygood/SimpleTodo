namespace SimpleTodo.Exceptions;

public sealed class CannotModifyFinishedTodoException(Guid id)
    : Exception($"Cannot modify finished todo item with id {id}");
