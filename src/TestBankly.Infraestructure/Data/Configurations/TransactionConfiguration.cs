using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestBankly.Domain.Models;

namespace TestBankly.Infraestructure.Data.Configurations
{
    //public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    //{
    //    public void Configure(EntityTypeBuilder<Transaction> builder)
    //    {
    //        builder.ToTable("Transaction");
    //        builder.HasKey(p => p.Id);
    //        builder.Property(p => p.TransactionId).HasColumnType("VARCHAR(100)").IsRequired();
    //        builder.Property(p => p.Status).HasColumnType("VARCHAR(10)").IsRequired();
    //        builder.Property(p => p.Value).HasDefaultValue(0).IsRequired();
    //        builder.Property(p => p.AccountDestination).HasColumnType("VARCHAR(10)").IsRequired();
    //        builder.Property(p => p.AccountOrigin).HasColumnType("VARCHAR(10)").IsRequired();
    //        builder.Property(p => p.ErrorReason).HasColumnType("VARCHAR(200)");

    //        builder.HasIndex(i => i.TransactionId).HasName("idx_transaction_id");
    //    }
    //}
}
