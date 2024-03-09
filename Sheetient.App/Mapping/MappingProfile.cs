using AutoMapper;
using System.Reflection;

namespace Sheetient.App.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
            .Where(t => Array.Exists(
                t.GetInterfaces(),
                i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapping<>)));

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapping`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, [this]);

            }
        }
    }
}
