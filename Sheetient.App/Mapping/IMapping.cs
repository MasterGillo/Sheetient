using AutoMapper;

namespace Sheetient.App.Mapping
{
    public interface IMapping<T>
    {
        void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType()).ReverseMap();
    }
}
