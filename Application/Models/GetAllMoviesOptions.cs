﻿namespace MovieHub.Application.Models;

public class GetAllMoviesOptions
{
    public string? Title { get; set; }

    public int? YearOfRelease { get; set; }

    public IEnumerable<int>? Genres { get; set; }

    public Guid? UserId { get; set; }

    public string? SortField { get; set; }

    public SortOrder? SortOrder { get; set; }
    
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public IEnumerable<Guid>? Actors { get; set; }
    public int Page { get; set; }

    public int PageSize { get; set; }
}