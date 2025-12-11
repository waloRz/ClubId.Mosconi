using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using ClubId.Models;

namespace ClubId.Data;

public partial class LigabdContext : DbContext
{
    public LigabdContext()
    {
    }

    public LigabdContext(DbContextOptions<LigabdContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Equipo> Equipos { get; set; }

    public virtual DbSet<Equipoxpartido> Equipoxpartidos { get; set; }

    public virtual DbSet<Equipoxtorneo> Equipoxtorneos { get; set; }

    public virtual DbSet<Fecha> Fechas { get; set; }

    public virtual DbSet<Jgrxequipo> Jgrxequipos { get; set; }

    public virtual DbSet<Jueqxsancion> Jueqxsancions { get; set; }

    public virtual DbSet<Jugadore> Jugadores { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Partido> Partidos { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Sancione> Sanciones { get; set; }

    public virtual DbSet<Torneo> Torneos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=ligabd;port=3306;uid=root;pwd=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategorias).HasName("PRIMARY");

            entity.ToTable("categorias");

            entity.Property(e => e.IdCategorias).HasColumnName("idCategorias");
            entity.Property(e => e.EstadoCat).HasColumnName("estadoCat");
            entity.Property(e => e.NombreCat)
                .HasMaxLength(50)
                .HasColumnName("nombreCat");
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.IdEquipo).HasName("PRIMARY");

            entity.ToTable("equipos", tb => tb.HasComment("Equipos de la Liga, el cual tiene una foranea que me lleva a la categoria donde juega"));

            entity.HasIndex(e => e.IdCategoria, "idCategoria");

            entity.Property(e => e.IdEquipo).HasColumnName("idEquipo");
            entity.Property(e => e.Celular)
                .HasMaxLength(50)
                .HasColumnName("celular");
            entity.Property(e => e.Delegado)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("delegado");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FotoEq)
                .HasMaxLength(255)
                .HasColumnName("fotoEq");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.NombreEq)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("nombreEq");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Equipos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK1_Categorias");
        });

        modelBuilder.Entity<Equipoxpartido>(entity =>
        {
            entity.HasKey(e => e.IdExP).HasName("PRIMARY");

            entity.ToTable("equipoxpartido");

            entity.HasIndex(e => e.IdEquipo, "FK_equipoxpartido_equipos");

            entity.HasIndex(e => e.IdEquipo2, "FK_equipoxpartido_equipos2");

            entity.HasIndex(e => e.IdPartido, "FK_equipoxpartido_partidos");

            entity.Property(e => e.IdExP).HasColumnName("idExP");
            entity.Property(e => e.IdEquipo).HasColumnName("idEquipo");
            entity.Property(e => e.IdEquipo2).HasColumnName("idEquipo2");
            entity.Property(e => e.IdPartido).HasColumnName("idPartido");
            entity.Property(e => e.Resultado).HasColumnName("resultado");
            entity.Property(e => e.Resultado2).HasColumnName("resultado2");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.EquipoxpartidoIdEquipoNavigations)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_equipoxpartido_equipos");

            entity.HasOne(d => d.IdEquipo2Navigation).WithMany(p => p.EquipoxpartidoIdEquipo2Navigations)
                .HasForeignKey(d => d.IdEquipo2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_equipoxpartido_partidos_2");

            entity.HasOne(d => d.IdPartidoNavigation).WithMany(p => p.Equipoxpartidos)
                .HasForeignKey(d => d.IdPartido)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_equipoxpartido_partidos");
        });

        modelBuilder.Entity<Equipoxtorneo>(entity =>
        {
            entity.HasKey(e => e.IdExT).HasName("PRIMARY");

            entity.ToTable("equipoxtorneo", tb => tb.HasComment("los equipos que jugan X torneo"));

            entity.HasIndex(e => e.IdTorneo, "FK_equipoxtorneo_torneos");

            entity.HasIndex(e => new { e.Idequipo, e.IdTorneo }, "idequipo_idToreno").IsUnique();

            entity.Property(e => e.IdExT).HasColumnName("idExT");
            entity.Property(e => e.Empatados)
                .HasDefaultValueSql("'0'")
                .HasColumnName("empatados");
            entity.Property(e => e.Ganados)
                .HasDefaultValueSql("'0'")
                .HasColumnName("ganados");
            entity.Property(e => e.GolesContra)
                .HasDefaultValueSql("'0'")
                .HasColumnName("golesContra");
            entity.Property(e => e.GolesFavor)
                .HasDefaultValueSql("'0'")
                .HasColumnName("golesFavor");
            entity.Property(e => e.IdTorneo).HasColumnName("idTorneo");
            entity.Property(e => e.Idequipo).HasColumnName("idequipo");
            entity.Property(e => e.Jugados)
                .HasDefaultValueSql("'0'")
                .HasColumnName("jugados");
            entity.Property(e => e.Perdidos)
                .HasDefaultValueSql("'0'")
                .HasColumnName("perdidos");
            entity.Property(e => e.Puntos)
                .HasDefaultValueSql("'0'")
                .HasColumnName("puntos");

            entity.HasOne(d => d.IdTorneoNavigation).WithMany(p => p.Equipoxtorneos)
                .HasForeignKey(d => d.IdTorneo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_equipoxtorneo_torneos");

            entity.HasOne(d => d.IdequipoNavigation).WithMany(p => p.Equipoxtorneos)
                .HasForeignKey(d => d.Idequipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_equipoxtorneo_equipos");
        });

        modelBuilder.Entity<Fecha>(entity =>
        {
            entity.HasKey(e => e.IdFecha).HasName("PRIMARY");

            entity.ToTable("fechas", tb => tb.HasComment("son las fechas de los torneos, su tamaño esta dado por la cantidad de equipos que tenga el torneo\r\n"));

            entity.HasIndex(e => e.IdTorneo, "FK__torneos");

            entity.HasIndex(e => new { e.NroFecha, e.IdTorneo }, "nroFecha_idTorneo").IsUnique();

            entity.Property(e => e.IdFecha).HasColumnName("idFecha");
            entity.Property(e => e.Fechora).HasColumnName("fechora");
            entity.Property(e => e.IdTorneo).HasColumnName("idTorneo");
            entity.Property(e => e.NroFecha).HasColumnName("nroFecha");

            entity.HasOne(d => d.IdTorneoNavigation).WithMany(p => p.Fechas)
                .HasForeignKey(d => d.IdTorneo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__torneos");
        });

        modelBuilder.Entity<Jgrxequipo>(entity =>
        {
            entity.HasKey(e => e.IdJxE).HasName("PRIMARY");

            entity.ToTable("jgrxequipo", tb => tb.HasComment("el jugador Y en el equipo X con FK de las talblas equipos y de jugadores"));

            entity.HasIndex(e => e.IdEquipo, "FK1_Equipos");

            entity.HasIndex(e => e.Idjugador, "FK2_Jugadores");

            entity.HasIndex(e => e.IdCategorias, "FK3_Categorias");

            entity.Property(e => e.IdJxE).HasColumnName("idJxE");
            entity.Property(e => e.FechaRecibo)
                .HasColumnType("datetime")
                .HasColumnName("fechaRecibo");
            entity.Property(e => e.IdCategorias).HasColumnName("idCategorias");
            entity.Property(e => e.IdEquipo).HasColumnName("idEquipo");
            entity.Property(e => e.Idjugador).HasColumnName("idjugador");

            entity.HasOne(d => d.IdCategoriasNavigation).WithMany(p => p.Jgrxequipos)
                .HasForeignKey(d => d.IdCategorias)
                .HasConstraintName("FK3_Categorias");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.Jgrxequipos)
                .HasForeignKey(d => d.IdEquipo)
                .HasConstraintName("FK1_Equipos");

            entity.HasOne(d => d.IdjugadorNavigation).WithMany(p => p.Jgrxequipos)
                .HasForeignKey(d => d.Idjugador)
                .HasConstraintName("FK2_Jugadores");
        });

        modelBuilder.Entity<Jueqxsancion>(entity =>
        {
            entity.HasKey(e => e.IdJexS).HasName("PRIMARY");

            entity.ToTable("jueqxsancion");

            entity.HasIndex(e => e.IdSanciones, "FK1_Sanciones");

            entity.HasIndex(e => e.Idjugador, "FK2_Jugador");

            entity.HasIndex(e => e.IdEquipo, "FK3_Equipo");

            entity.Property(e => e.IdJexS).HasColumnName("idJexS");
            entity.Property(e => e.IdEquipo).HasColumnName("idEquipo");
            entity.Property(e => e.IdSanciones).HasColumnName("idSanciones");
            entity.Property(e => e.Idjugador).HasColumnName("idjugador");
            entity.Property(e => e.Informe)
                .HasColumnType("text")
                .HasColumnName("informe");
            entity.Property(e => e.Sancion)
                .HasMaxLength(25)
                .HasColumnName("sancion");

            entity.HasOne(d => d.IdEquipoNavigation).WithMany(p => p.Jueqxsancions)
                .HasForeignKey(d => d.IdEquipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("jueqxsancion_ibfk_3");

            entity.HasOne(d => d.IdSancionesNavigation).WithMany(p => p.Jueqxsancions)
                .HasForeignKey(d => d.IdSanciones)
                .HasConstraintName("jueqxsancion_ibfk_1");

            entity.HasOne(d => d.IdjugadorNavigation).WithMany(p => p.Jueqxsancions)
                .HasForeignKey(d => d.Idjugador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("jueqxsancion_ibfk_2");
        });

        modelBuilder.Entity<Jugadore>(entity =>
        {
            entity.HasKey(e => e.Idjugador).HasName("PRIMARY");

            entity.ToTable("jugadores", tb => tb.HasComment("datos del jugador"));

            entity.Property(e => e.Idjugador).HasColumnName("idjugador");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Apellido)
                .HasMaxLength(30)
                .HasColumnName("apellido");
            entity.Property(e => e.Dni)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("dni");
            entity.Property(e => e.FechaNac)
                .HasColumnType("datetime")
                .HasColumnName("fechaNac");
            entity.Property(e => e.Foto)
                .HasMaxLength(255)
                .HasColumnName("foto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("login");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .HasDefaultValueSql("''")
                .HasColumnName("usuario");
        });

        modelBuilder.Entity<Partido>(entity =>
        {
            entity.HasKey(e => e.IdPartido).HasName("PRIMARY");

            entity.ToTable("partidos", tb => tb.HasComment("todos los partido de la fecha"));

            entity.HasIndex(e => new { e.IdFecha, e.NroPartido }, "idFecha_nroPartido").IsUnique();

            entity.Property(e => e.IdPartido).HasColumnName("idPartido");
            entity.Property(e => e.Cancha).HasColumnName("cancha");
            entity.Property(e => e.Horario)
                .HasColumnType("time")
                .HasColumnName("horario");
            entity.Property(e => e.IdFecha).HasColumnName("idFecha");
            entity.Property(e => e.NroPartido).HasColumnName("nroPartido");

            entity.HasOne(d => d.IdFechaNavigation).WithMany(p => p.Partidos)
                .HasForeignKey(d => d.IdFecha)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_partidos_fechas");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("players");

            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.Dni).HasMaxLength(15);
            entity.Property(e => e.FirstName).HasMaxLength(15);
            entity.Property(e => e.LastName).HasMaxLength(15);
            entity.Property(e => e.PhotoPath).HasMaxLength(255);
            entity.Property(e => e.Team).HasMaxLength(15);
        });

        modelBuilder.Entity<Sancione>(entity =>
        {
            entity.HasKey(e => e.IdSanciones).HasName("PRIMARY");

            entity.ToTable("sanciones");

            entity.HasIndex(e => e.IdCategorias, "FK1_SCategorias");

            entity.Property(e => e.IdSanciones).HasColumnName("idSanciones");
            entity.Property(e => e.Comunicado).HasColumnType("text");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.IdCategorias).HasColumnName("idCategorias");

            entity.HasOne(d => d.IdCategoriasNavigation).WithMany(p => p.Sanciones)
                .HasForeignKey(d => d.IdCategorias)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK1_SCategorias");
        });

        modelBuilder.Entity<Torneo>(entity =>
        {
            entity.HasKey(e => e.IdTorneo).HasName("PRIMARY");

            entity.ToTable("torneos");

            entity.HasIndex(e => e.IdCategoria, "FK_torneos_categorias");

            entity.Property(e => e.IdTorneo).HasColumnName("idTorneo");
            entity.Property(e => e.CantEquipos).HasColumnName("cantEquipos");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Torneos)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_torneos_categorias");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(300)
                .HasColumnName("avatar");
            entity.Property(e => e.Correo).HasMaxLength(50);
            entity.Property(e => e.Nombre)
                .HasMaxLength(300)
                .HasColumnName("nombre");
            entity.Property(e => e.Planeta)
                .HasMaxLength(300)
                .HasColumnName("planeta");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
