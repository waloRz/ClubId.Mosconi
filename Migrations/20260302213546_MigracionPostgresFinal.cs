using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ClubId.Migrations
{
    /// <inheritdoc />
    public partial class MigracionPostgresFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    idCategorias = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombreCat = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    estadoCat = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.idCategorias);
                });

            migrationBuilder.CreateTable(
                name: "jugadores",
                columns: table => new
                {
                    idjugador = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nroCarnetOld = table.Column<int>(type: "integer", nullable: true),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    dni = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "''"),
                    fechaNac = table.Column<DateOnly>(type: "date", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false),
                    foto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadores", x => x.idjugador);
                },
                comment: "datos del jugador");

            migrationBuilder.CreateTable(
                name: "login",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "''"),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    LastName = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Dni = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Team = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    PhotoPath = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    avatar = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    nombre = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    planeta = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Correo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipos",
                columns: table => new
                {
                    idEquipo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idCategoria = table.Column<int>(type: "integer", nullable: false),
                    nombreEq = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "''"),
                    delegado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "''"),
                    celular = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: false),
                    fotoEq = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.idEquipo);
                    table.ForeignKey(
                        name: "FK1_Categorias",
                        column: x => x.idCategoria,
                        principalTable: "categorias",
                        principalColumn: "idCategorias",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Equipos de la Liga, el cual tiene una foranea que me lleva a la categoria donde juega");

            migrationBuilder.CreateTable(
                name: "sanciones",
                columns: table => new
                {
                    idSanciones = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    idCategorias = table.Column<int>(type: "integer", nullable: false),
                    NroFecha = table.Column<int>(type: "integer", nullable: false),
                    Comunicado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sanciones", x => x.idSanciones);
                    table.ForeignKey(
                        name: "FK1_SCategorias",
                        column: x => x.idCategorias,
                        principalTable: "categorias",
                        principalColumn: "idCategorias");
                });

            migrationBuilder.CreateTable(
                name: "torneos",
                columns: table => new
                {
                    idTorneo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    idCategoria = table.Column<int>(type: "integer", nullable: false),
                    cantEquipos = table.Column<int>(type: "integer", nullable: true),
                    estado = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torneos", x => x.idTorneo);
                    table.ForeignKey(
                        name: "FK_torneos_categorias",
                        column: x => x.idCategoria,
                        principalTable: "categorias",
                        principalColumn: "idCategorias");
                });

            migrationBuilder.CreateTable(
                name: "jgrxequipo",
                columns: table => new
                {
                    idJxE = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idjugador = table.Column<int>(type: "integer", nullable: false),
                    idEquipo = table.Column<int>(type: "integer", nullable: false),
                    fechaRecibo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    idCategorias = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jgrxequipos", x => x.idJxE);
                    table.ForeignKey(
                        name: "FK1_Equipos",
                        column: x => x.idEquipo,
                        principalTable: "equipos",
                        principalColumn: "idEquipo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK2_Jugadores",
                        column: x => x.idjugador,
                        principalTable: "jugadores",
                        principalColumn: "idjugador",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK3_Categorias",
                        column: x => x.idCategorias,
                        principalTable: "categorias",
                        principalColumn: "idCategorias",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "el jugador Y en el equipo X con FK de las talblas equipos y de jugadores");

            migrationBuilder.CreateTable(
                name: "jueqxsancion",
                columns: table => new
                {
                    idJexS = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idSanciones = table.Column<int>(type: "integer", nullable: false),
                    idjugador = table.Column<int>(type: "integer", nullable: false),
                    idEquipo = table.Column<int>(type: "integer", nullable: false),
                    sancion = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    informe = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jueqxsancion", x => x.idJexS);
                    table.ForeignKey(
                        name: "jueqxsancion_ibfk_1",
                        column: x => x.idSanciones,
                        principalTable: "sanciones",
                        principalColumn: "idSanciones",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "jueqxsancion_ibfk_2",
                        column: x => x.idjugador,
                        principalTable: "jugadores",
                        principalColumn: "idjugador");
                    table.ForeignKey(
                        name: "jueqxsancion_ibfk_3",
                        column: x => x.idEquipo,
                        principalTable: "equipos",
                        principalColumn: "idEquipo");
                });

            migrationBuilder.CreateTable(
                name: "equipoxtorneo",
                columns: table => new
                {
                    idExT = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idequipo = table.Column<int>(type: "integer", nullable: false),
                    idTorneo = table.Column<int>(type: "integer", nullable: false),
                    jugados = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    ganados = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    empatados = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    perdidos = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    golesFavor = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    golesContra = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'"),
                    puntos = table.Column<int>(type: "integer", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipoxtorneos", x => x.idExT);
                    table.ForeignKey(
                        name: "FK_equipoxtorneo_equipos",
                        column: x => x.idequipo,
                        principalTable: "equipos",
                        principalColumn: "idEquipo");
                    table.ForeignKey(
                        name: "FK_equipoxtorneo_torneos",
                        column: x => x.idTorneo,
                        principalTable: "torneos",
                        principalColumn: "idTorneo");
                },
                comment: "los equipos que jugan X torneo");

            migrationBuilder.CreateTable(
                name: "fechas",
                columns: table => new
                {
                    idFecha = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fechora = table.Column<DateOnly>(type: "date", nullable: true),
                    idTorneo = table.Column<int>(type: "integer", nullable: false),
                    nroFecha = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fechas", x => x.idFecha);
                    table.ForeignKey(
                        name: "FK__torneos",
                        column: x => x.idTorneo,
                        principalTable: "torneos",
                        principalColumn: "idTorneo");
                },
                comment: "son las fechas de los torneos, su tamaño esta dado por la cantidad de equipos que tenga el torneo\r\n");

            migrationBuilder.CreateTable(
                name: "partidos",
                columns: table => new
                {
                    idPartido = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idFecha = table.Column<int>(type: "integer", nullable: false),
                    nroPartido = table.Column<int>(type: "integer", nullable: true),
                    horario = table.Column<TimeOnly>(type: "time", nullable: true),
                    cancha = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.idPartido);
                    table.ForeignKey(
                        name: "FK_partidos_fechas",
                        column: x => x.idFecha,
                        principalTable: "fechas",
                        principalColumn: "idFecha");
                },
                comment: "todos los partido de la fecha");

            migrationBuilder.CreateTable(
                name: "equipoxpartido",
                columns: table => new
                {
                    idExP = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idPartido = table.Column<int>(type: "integer", nullable: false),
                    idEquipo = table.Column<int>(type: "integer", nullable: false),
                    resultado = table.Column<int>(type: "integer", nullable: true),
                    idEquipo2 = table.Column<int>(type: "integer", nullable: false),
                    resultado2 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipoxpartidos", x => x.idExP);
                    table.ForeignKey(
                        name: "FK_equipoxpartido_equipos",
                        column: x => x.idEquipo,
                        principalTable: "equipos",
                        principalColumn: "idEquipo");
                    table.ForeignKey(
                        name: "FK_equipoxpartido_partidos",
                        column: x => x.idPartido,
                        principalTable: "partidos",
                        principalColumn: "idPartido");
                    table.ForeignKey(
                        name: "FK_equipoxpartido_partidos_2",
                        column: x => x.idEquipo2,
                        principalTable: "equipos",
                        principalColumn: "idEquipo");
                });

            migrationBuilder.CreateIndex(
                name: "idCategoria",
                table: "equipos",
                column: "idCategoria");

            migrationBuilder.CreateIndex(
                name: "FK_equipoxpartido_equipos",
                table: "equipoxpartido",
                column: "idEquipo");

            migrationBuilder.CreateIndex(
                name: "FK_equipoxpartido_equipos2",
                table: "equipoxpartido",
                column: "idEquipo2");

            migrationBuilder.CreateIndex(
                name: "FK_equipoxpartido_partidos",
                table: "equipoxpartido",
                column: "idPartido");

            migrationBuilder.CreateIndex(
                name: "FK_equipoxtorneo_torneos",
                table: "equipoxtorneo",
                column: "idTorneo");

            migrationBuilder.CreateIndex(
                name: "idequipo_idToreno",
                table: "equipoxtorneo",
                columns: new[] { "idequipo", "idTorneo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK__torneos",
                table: "fechas",
                column: "idTorneo");

            migrationBuilder.CreateIndex(
                name: "nroFecha_idTorneo",
                table: "fechas",
                columns: new[] { "nroFecha", "idTorneo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK1_Equipos",
                table: "jgrxequipo",
                column: "idEquipo");

            migrationBuilder.CreateIndex(
                name: "FK2_Jugadores",
                table: "jgrxequipo",
                column: "idjugador");

            migrationBuilder.CreateIndex(
                name: "FK3_Categorias",
                table: "jgrxequipo",
                column: "idCategorias");

            migrationBuilder.CreateIndex(
                name: "FK1_Sanciones",
                table: "jueqxsancion",
                column: "idSanciones");

            migrationBuilder.CreateIndex(
                name: "FK2_Jugador",
                table: "jueqxsancion",
                column: "idjugador");

            migrationBuilder.CreateIndex(
                name: "FK3_Equipo",
                table: "jueqxsancion",
                column: "idEquipo");

            migrationBuilder.CreateIndex(
                name: "idFecha_nroPartido",
                table: "partidos",
                columns: new[] { "idFecha", "nroPartido" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK1_SCategorias",
                table: "sanciones",
                column: "idCategorias");

            migrationBuilder.CreateIndex(
                name: "FK_torneos_categorias",
                table: "torneos",
                column: "idCategoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipoxpartido");

            migrationBuilder.DropTable(
                name: "equipoxtorneo");

            migrationBuilder.DropTable(
                name: "jgrxequipo");

            migrationBuilder.DropTable(
                name: "jueqxsancion");

            migrationBuilder.DropTable(
                name: "login");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "partidos");

            migrationBuilder.DropTable(
                name: "sanciones");

            migrationBuilder.DropTable(
                name: "jugadores");

            migrationBuilder.DropTable(
                name: "equipos");

            migrationBuilder.DropTable(
                name: "fechas");

            migrationBuilder.DropTable(
                name: "torneos");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
