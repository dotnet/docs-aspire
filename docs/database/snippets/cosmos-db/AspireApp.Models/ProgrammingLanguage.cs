namespace AspireApp.Models;

public record ProgrammingLanguage(
    string Id,
    string Name,
    string Description,
    DateOnly InitialReleaseDate);
