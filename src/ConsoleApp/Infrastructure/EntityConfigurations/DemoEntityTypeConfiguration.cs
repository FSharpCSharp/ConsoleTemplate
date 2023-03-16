using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.Domain.AggregatesModel.DemoAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsoleApp.Infrastructure.EntityConfigurations
{
    internal class DemoEntityTypeConfiguration : IEntityTypeConfiguration<Demo>
    {
        public void Configure(EntityTypeBuilder<Demo> buyerConfiguration)
        {
            buyerConfiguration.ToTable("entries");

            buyerConfiguration.HasKey(b => b.Id);

            buyerConfiguration.Ignore(b => b.DomainEvents);

            buyerConfiguration.Property(b => b.Title);

            buyerConfiguration.Property(b => b.Description);
        }
    }
}
