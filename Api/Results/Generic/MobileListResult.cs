using Core.Interfaces;

namespace Api.Results.Generic;

public class MobileListResult<TEntity> where TEntity : IMobileListResult
{
    public List<TEntity> Items { get; set; }


    public MobileListResult(IEnumerable<TEntity> items)
    {
        Items = new List<TEntity>(items);
    }
}