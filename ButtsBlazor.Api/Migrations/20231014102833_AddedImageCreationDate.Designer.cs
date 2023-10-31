﻿// <auto-generated />
using System;
using ButtsBlazor.Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ButtsBlazor.Api.Migrations
{
    [DbContext(typeof(ButtsDbContext))]
    [Migration("20231014102833_AddedImageCreationDate")]
    partial class AddedImageCreationDate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0-rc.2.23480.1");

            modelBuilder.Entity("ButtsBlazor.Api.Model.ImageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Base64Hash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("ButtsBlazor.Api.Model.PromptEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ArgsId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CannyImageId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ControlImageId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("Enqueued")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("OutputImageId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ProcessingCompleted")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ProcessingStart")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ArgsId");

                    b.HasIndex("CannyImageId");

                    b.HasIndex("ControlImageId");

                    b.HasIndex("OutputImageId");

                    b.ToTable("Prompts");
                });

            modelBuilder.Entity("ButtsBlazor.Services.PromptArgs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("CannyHigh")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CannyLow")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ControlFile")
                        .HasColumnType("TEXT");

                    b.Property<string>("ControlFilePath")
                        .HasColumnType("TEXT");

                    b.Property<double?>("ControlScale")
                        .HasColumnType("REAL");

                    b.Property<int?>("ControlSize")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Negative")
                        .HasColumnType("TEXT");

                    b.Property<int?>("NumSteps")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Prompt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PromptArgs");
                });

            modelBuilder.Entity("ButtsBlazor.Api.Model.PromptEntity", b =>
                {
                    b.HasOne("ButtsBlazor.Services.PromptArgs", "Args")
                        .WithMany()
                        .HasForeignKey("ArgsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ButtsBlazor.Api.Model.ImageEntity", "CannyImage")
                        .WithMany()
                        .HasForeignKey("CannyImageId");

                    b.HasOne("ButtsBlazor.Api.Model.ImageEntity", "ControlImage")
                        .WithMany()
                        .HasForeignKey("ControlImageId");

                    b.HasOne("ButtsBlazor.Api.Model.ImageEntity", "OutputImage")
                        .WithMany()
                        .HasForeignKey("OutputImageId");

                    b.Navigation("Args");

                    b.Navigation("CannyImage");

                    b.Navigation("ControlImage");

                    b.Navigation("OutputImage");
                });
#pragma warning restore 612, 618
        }
    }
}
