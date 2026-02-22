namespace AuthPlaypen.Application.Dtos;

public record CursorPagedResultDto<T>(IReadOnlyCollection<T> Items, string? NextCursor);
