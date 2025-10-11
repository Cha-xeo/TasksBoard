namespace TaskBoard.Api.Mappers
{
    public interface IBaseMapper<TEntity, TDto>
    {
        static abstract TDto ToDto(TEntity entity, List<string>? expands = null);
    }
}
