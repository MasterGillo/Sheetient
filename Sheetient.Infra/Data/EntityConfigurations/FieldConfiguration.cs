using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sheetient.Domain.Entities;

namespace Sheetient.Infra.Data.EntityConfigurations
{
    public class FieldConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> builder)
        {
            builder.UseTpcMappingStrategy();
        }
    }
}
