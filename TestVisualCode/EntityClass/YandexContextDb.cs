using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestVisualCode;

public partial class YC

 : DbContext
{
    public YC

    ()
    {
    }

    public YC

    (DbContextOptions<YC

    > options)
        : base(options)
    {
    }

    public virtual DbSet<TAlbum> TAlbums { get; set; }

    public virtual DbSet<TArtist> TArtists { get; set; }

    public virtual DbSet<TConfig> TConfigs { get; set; }

    public virtual DbSet<TLikeAlbum> TLikeAlbums { get; set; }

    public virtual DbSet<TLikeAlbumArtist> TLikeAlbumArtists { get; set; }

    public virtual DbSet<TLikeAlbumTrack> TLikeAlbumTracks { get; set; }

    public virtual DbSet<TLikeArtist> TLikeArtists { get; set; }

    public virtual DbSet<TLikeEntityDownloadedTrack> TLikeEntityDownloadedTracks { get; set; }

    public virtual DbSet<TLikePlaylist> TLikePlaylists { get; set; }

    public virtual DbSet<TLikePlaylistTrack> TLikePlaylistTracks { get; set; }

    public virtual DbSet<TPlayAudio> TPlayAudios { get; set; }

    public virtual DbSet<TPlayInformation> TPlayInformations { get; set; }

    public virtual DbSet<TPlaybackQueue> TPlaybackQueues { get; set; }

    public virtual DbSet<TPlaylist> TPlaylists { get; set; }

    public virtual DbSet<TPlaylistTrack> TPlaylistTracks { get; set; }

    public virtual DbSet<TQueueShuffleIndex> TQueueShuffleIndices { get; set; }

    public virtual DbSet<TQueueTrack> TQueueTracks { get; set; }

    public virtual DbSet<TRadioLike> TRadioLikes { get; set; }

    public virtual DbSet<TRadioSkip> TRadioSkips { get; set; }

    public virtual DbSet<TTrack> TTracks { get; set; }

    public virtual DbSet<TTrackAlbum> TTrackAlbums { get; set; }

    public virtual DbSet<TTrackArtist> TTrackArtists { get; set; }

    public virtual DbSet<TTrackLyric> TTrackLyrics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={YandexMusic.PathDBSqlite}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TAlbum>(entity =>
        {
            entity.ToTable("T_Album");

            entity.Property(e => e.Id).HasColumnType("varchar (512)");
            entity.Property(e => e.AlbumOptions).HasColumnType("varchar");
            entity.Property(e => e.AlbumVersion).HasColumnType("varchar");
            entity.Property(e => e.ArtistsString).HasColumnType("varchar");
            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.GenreId).HasColumnType("varchar");
            entity.Property(e => e.GenreTitle).HasColumnType("varchar");
            entity.Property(e => e.Title).HasColumnType("varchar");
            entity.Property(e => e.Year).HasColumnType("varchar");
        });

        modelBuilder.Entity<TArtist>(entity =>
        {
            entity.ToTable("T_Artist");

            entity.Property(e => e.Id).HasColumnType("varchar (512)");
            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.Name).HasColumnType("varchar");
        });

        modelBuilder.Entity<TConfig>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.ToTable("T_Config");

            entity.Property(e => e.Key).HasColumnType("varchar");
            entity.Property(e => e.Value).HasColumnType("varchar");
        });

        modelBuilder.Entity<TLikeAlbum>(entity =>
        {
            entity.ToTable("T_LikeAlbum");

            entity.Property(e => e.Id).HasColumnType("varchar (512)");
            entity.Property(e => e.AlbumVersion).HasColumnType("varchar");
            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.Genre).HasColumnType("varchar");
            entity.Property(e => e.Title).HasColumnType("varchar");
            entity.Property(e => e.Type).HasColumnType("varchar");
            entity.Property(e => e.Year).HasColumnType("varchar");
        });

        modelBuilder.Entity<TLikeAlbumArtist>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_LikeAlbumArtist");

            entity.HasIndex(e => new { e.LikeAlbumId, e.ArtistId }, "LikeAlbumArtist_Id").IsUnique();

            entity.Property(e => e.ArtistId).HasColumnType("varchar (512)");
            entity.Property(e => e.LikeAlbumId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TLikeAlbumTrack>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_LikeAlbumTrack");

            entity.HasIndex(e => new { e.TrackId, e.AlbumId }, "LikeAlbumTrack_Id");

            entity.HasIndex(e => e.LikeAlbumId, "LikeAlbum_Id");

            entity.Property(e => e.AlbumId).HasColumnType("varchar (512)");
            entity.Property(e => e.LikeAlbumId).HasColumnType("varchar (512)");
            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TLikeArtist>(entity =>
        {
            entity.ToTable("T_LikeArtist");

            entity.Property(e => e.Id).HasColumnType("varchar (512)");
            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.Genres).HasColumnType("varchar");
            entity.Property(e => e.Name).HasColumnType("varchar");
        });

        modelBuilder.Entity<TLikeEntityDownloadedTrack>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_LikeEntityDownloadedTrack");

            entity.HasIndex(e => new { e.TrackId, e.AlbumId }, "LikeEntityDownloadedTrack_Id").IsUnique();

            entity.HasIndex(e => e.LikeEntityId, "LikeEntity_Id");

            entity.Property(e => e.AlbumId).HasColumnType("varchar (512)");
            entity.Property(e => e.LikeEntityId).HasColumnType("varchar (512)");
            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TLikePlaylist>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_LikePlaylist");

            entity.HasIndex(e => new { e.OwnerUid, e.Kind }, "LikePlaylist_Id").IsUnique();

            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.Kind).HasColumnType("varchar (512)");
            entity.Property(e => e.OwnerName).HasColumnType("varchar");
            entity.Property(e => e.OwnerUid).HasColumnType("varchar (512)");
            entity.Property(e => e.PlaylistUuid).HasColumnType("varchar");
            entity.Property(e => e.Title).HasColumnType("varchar");
        });

        modelBuilder.Entity<TLikePlaylistTrack>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_LikePlaylistTrack");

            entity.HasIndex(e => new { e.TrackId, e.AlbumId }, "LikePlaylistTrack_Id");

            entity.Property(e => e.AlbumId).HasColumnType("varchar (512)");
            entity.Property(e => e.LikePlaylistId).HasColumnType("varchar (512)");
            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TPlayAudio>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_PlayAudio");

            entity.Property(e => e.Content).HasColumnType("varchar");
        });

        modelBuilder.Entity<TPlayInformation>(entity =>
        {
            entity.ToTable("T_PlayInformation");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Definition).HasColumnType("varchar");
        });

        modelBuilder.Entity<TPlaybackQueue>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_PlaybackQueue");

            entity.Property(e => e.QueueType).HasColumnType("varchar");
            entity.Property(e => e.RemoteId).HasColumnType("varchar");
            entity.Property(e => e.StationId).HasColumnType("varchar");
        });

        modelBuilder.Entity<TPlaylist>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_Playlist");

            entity.HasIndex(e => e.Kind, "Playlist_Kind");

            entity.HasIndex(e => e.Status, "Playlist_Status");

            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.Description).HasColumnType("varchar");
            entity.Property(e => e.DescriptionFormatted).HasColumnType("varchar");
            entity.Property(e => e.IdForFrom).HasColumnType("varchar");
            entity.Property(e => e.Kind).HasColumnType("varchar (512)");
            entity.Property(e => e.PlaulistUuid).HasColumnType("varchar");
            entity.Property(e => e.Title).HasColumnType("varchar");
        });

        modelBuilder.Entity<TPlaylistTrack>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_PlaylistTrack");

            entity.HasIndex(e => new { e.TrackId, e.Kind }, "PlaylistTrack_Id");

            entity.HasIndex(e => e.Status, "PlaylistTrack_Status");

            entity.Property(e => e.AlbumId).HasColumnType("varchar");
            entity.Property(e => e.Kind).HasColumnType("varchar (512)");
            entity.Property(e => e.Timestamp).HasColumnType("bigint");
            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TQueueShuffleIndex>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_QueueShuffleIndex");
        });

        modelBuilder.Entity<TQueueTrack>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("T_QueueTrack");

            entity.Property(e => e.AlbumId).HasColumnType("varchar");
            entity.Property(e => e.TrackId).HasColumnType("varchar");
        });

        modelBuilder.Entity<TRadioLike>(entity =>
        {
            entity.HasKey(e => e.TrackId);

            entity.ToTable("T_RadioLike");

            entity.Property(e => e.TrackId).HasColumnType("varchar");
        });

        modelBuilder.Entity<TRadioSkip>(entity =>
        {
            entity.ToTable("T_RadioSkip");

            entity.Property(e => e.Id).HasColumnType("varchar");
            entity.Property(e => e.ValidThrough).HasColumnType("bigint");
        });

        modelBuilder.Entity<TTrack>(entity =>
        {
            entity.ToTable("T_Track");

            entity.Property(e => e.Id).HasColumnType("varchar (512)");
            entity.Property(e => e.ContentWarning).HasColumnType("varchar");
            entity.Property(e => e.CoverUri).HasColumnType("varchar");
            entity.Property(e => e.PubDate).HasColumnType("varchar");
            entity.Property(e => e.RealId).HasColumnType("varchar");
            entity.Property(e => e.Title).HasColumnType("varchar");
            entity.Property(e => e.Token).HasColumnType("varchar (255)");
            entity.Property(e => e.TrackOptions).HasColumnType("varchar");
            entity.Property(e => e.Type).HasColumnType("varchar");
        });

        modelBuilder.Entity<TTrackAlbum>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_TrackAlbum");

            entity.HasIndex(e => new { e.TrackId, e.AlbumId }, "TrackAlbum_Id").IsUnique();

            entity.Property(e => e.AlbumId).HasColumnType("varchar (512)");
            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TTrackArtist>(entity =>
        {
            entity.HasKey(e => e.AutoId);

            entity.ToTable("T_TrackArtist");

            entity.HasIndex(e => new { e.TrackId, e.ArtistId }, "TrackArtist_Id").IsUnique();

            entity.Property(e => e.ArtistId).HasColumnType("varchar (512)");
            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
        });

        modelBuilder.Entity<TTrackLyric>(entity =>
        {
            entity.HasKey(e => e.TrackId);

            entity.ToTable("T_TrackLyrics");

            entity.Property(e => e.TrackId).HasColumnType("varchar (512)");
            entity.Property(e => e.FullLyrics).HasColumnType("varchar");
            entity.Property(e => e.Lyrics).HasColumnType("varchar");
            entity.Property(e => e.Url).HasColumnType("varchar");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
