﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PodNoms.Api.Models;

namespace PodNoms.Api.Migrations
{
    [DbContext(typeof(PodnomsContext))]
    [Migration("20161114224247_AddedSlugFields")]
    partial class AddedSlugFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PodNoms.Api.Models.Podcast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Slug");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdateDate");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Podcasts");
                });

            modelBuilder.Entity("PodNoms.Api.Models.PodcastEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AudioFileSize");

                    b.Property<float>("AudioLength");

                    b.Property<string>("AudioUrl");

                    b.Property<string>("Author");

                    b.Property<DateTime>("CreateDate");

                    b.Property<string>("Description");

                    b.Property<string>("ImageUrl");

                    b.Property<int?>("PodcastId");

                    b.Property<string>("Slug");

                    b.Property<string>("SourceUrl");

                    b.Property<string>("Title");

                    b.Property<string>("Uid");

                    b.Property<DateTime>("UpdateDate");

                    b.HasKey("Id");

                    b.HasIndex("PodcastId");

                    b.ToTable("PodcastEntries");
                });

            modelBuilder.Entity("PodNoms.Api.Models.PodcastEntry", b =>
                {
                    b.HasOne("PodNoms.Api.Models.Podcast", "Podcast")
                        .WithMany("PodcastEntries")
                        .HasForeignKey("PodcastId");
                });
        }
    }
}