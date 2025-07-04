﻿using Core.Books;

namespace Application.Interfaces.Repositories;

public interface IGenreRepository : IBaseRepository<Genre, Guid>
{
    Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}