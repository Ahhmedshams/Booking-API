﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.BookingItem", b =>
                {
                    b.Property<int>("BookingId")
                        .HasColumnType("int");

                    b.Property<int>("ResourceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("BookingId", "ResourceId");

                    b.HasIndex("ResourceId");

                    b.ToTable("BookingItems", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ClientBooking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<DateTime>("Date")
                        .HasColumnType("Date");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("Time");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("Time");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Pending");

                    b.Property<decimal>("TotalCost")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("UserId");

                    b.ToTable("ClientBookings", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Resource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ResourceTypeId");

                    b.ToTable("Resource", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ResourceData", b =>
                {
                    b.Property<int>("ResourceId")
                        .HasColumnType("int");

                    b.Property<int>("AttributeId")
                        .HasColumnType("int");

                    b.Property<string>("AttributeValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("ResourceId", "AttributeId");

                    b.HasIndex("AttributeId");

                    b.ToTable("ResourceData", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ResourceMetadata", b =>
                {
                    b.Property<int>("AttributeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttributeId"));

                    b.Property<string>("AttributeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AttributeType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("int");

                    b.HasKey("AttributeId");

                    b.HasIndex("ResourceTypeId");

                    b.ToTable("ResourceMetadata", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ResourceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ResourceTypes", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Schedule", b =>
                {
                    b.Property<int>("ScheduleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleID"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("DATE");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("ResourceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("DATE");

                    b.HasKey("ScheduleID");

                    b.HasIndex("ResourceId");

                    b.ToTable("Schedule", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ScheduleItem", b =>
                {
                    b.Property<DateTime>("Day")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("TIME");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("TIME");

                    b.Property<bool>("Available")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<bool>("Shift")
                        .HasColumnType("bit");

                    b.HasKey("Day", "StartTime", "EndTime");

                    b.HasIndex("ScheduleId");

                    b.ToTable("ScheduleItem", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValue("Active");

                    b.HasKey("Id");

                    b.ToTable("Services", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.ServiceMetadata", b =>
                {
                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<int>("ResourceTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("ServiceId", "ResourceTypeId");

                    b.HasIndex("ResourceTypeId");

                    b.ToTable("ServiceMetadata", (string)null);
                });

            modelBuilder.Entity("Domain.Identity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastUpdatedById")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastUpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.BookingItem", b =>
                {
                    b.HasOne("Domain.Entities.ClientBooking", "ClientBooking")
                        .WithMany("BookingItems")
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ClientBooking");

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("Domain.Entities.ClientBooking", b =>
                {
                    b.HasOne("Domain.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Identity.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Resource", b =>
                {
                    b.HasOne("Domain.Entities.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ResourceType");
                });

            modelBuilder.Entity("Domain.Entities.ResourceData", b =>
                {
                    b.HasOne("Domain.Entities.ResourceMetadata", "ResourceMetadata")
                        .WithMany()
                        .HasForeignKey("AttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");

                    b.Navigation("ResourceMetadata");
                });

            modelBuilder.Entity("Domain.Entities.ResourceMetadata", b =>
                {
                    b.HasOne("Domain.Entities.ResourceType", "ResourceType")
                        .WithMany("Metadata")
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResourceType");
                });

            modelBuilder.Entity("Domain.Entities.Schedule", b =>
                {
                    b.HasOne("Domain.Entities.Resource", "Resource")
                        .WithMany("Schedules")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("Domain.Entities.ScheduleItem", b =>
                {
                    b.HasOne("Domain.Entities.Schedule", "Schedule")
                        .WithMany("ScheduleItems")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("Domain.Entities.ServiceMetadata", b =>
                {
                    b.HasOne("Domain.Entities.ResourceType", "ResourceType")
                        .WithMany()
                        .HasForeignKey("ResourceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Service", "Service")
                        .WithMany("Metadata")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResourceType");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Domain.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domain.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domain.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Entities.ClientBooking", b =>
                {
                    b.Navigation("BookingItems");
                });

            modelBuilder.Entity("Domain.Entities.Resource", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("Domain.Entities.ResourceType", b =>
                {
                    b.Navigation("Metadata");
                });

            modelBuilder.Entity("Domain.Entities.Schedule", b =>
                {
                    b.Navigation("ScheduleItems");
                });

            modelBuilder.Entity("Domain.Entities.Service", b =>
                {
                    b.Navigation("Metadata");
                });
#pragma warning restore 612, 618
        }
    }
}
