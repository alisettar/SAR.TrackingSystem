using Microsoft.EntityFrameworkCore.Migrations;

namespace SAR.TrackingSystem.Infrastructure.Migrations
{
    /// <summary>
    /// Make ToSectorId nullable for state machine exit movements (BoO → null)
    /// </summary>
    public partial class MakeToSectorIdNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys = OFF;
                
                -- Create new table with nullable ToSectorId
                CREATE TABLE Movements_new (
                    Id TEXT NOT NULL PRIMARY KEY,
                    VolunteerId TEXT NOT NULL,
                    FromSectorId TEXT NULL,
                    ToSectorId TEXT NULL, -- Made nullable
                    MovementTime TEXT NOT NULL,
                    Type INTEGER NOT NULL,
                    IsGroupMovement INTEGER NOT NULL,
                    GroupId TEXT NULL,
                    Notes TEXT NULL,
                    FOREIGN KEY (VolunteerId) REFERENCES Volunteers (Id),
                    FOREIGN KEY (FromSectorId) REFERENCES Sectors (Id),
                    FOREIGN KEY (ToSectorId) REFERENCES Sectors (Id)
                );
                
                -- Copy data
                INSERT INTO Movements_new 
                SELECT Id, VolunteerId, FromSectorId, ToSectorId, MovementTime, Type, IsGroupMovement, GroupId, Notes 
                FROM Movements;
                
                -- Drop old table and rename new one
                DROP TABLE Movements;
                ALTER TABLE Movements_new RENAME TO Movements;
                
                -- Recreate indexes
                CREATE INDEX IX_Movements_MovementTime ON Movements (MovementTime);
                CREATE INDEX IX_Movements_GroupId ON Movements (GroupId) WHERE GroupId IS NOT NULL;
                
                PRAGMA foreign_keys = ON;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys = OFF;
                
                -- Create table with NOT NULL ToSectorId  
                CREATE TABLE Movements_old (
                    Id TEXT NOT NULL PRIMARY KEY,
                    VolunteerId TEXT NOT NULL,
                    FromSectorId TEXT NULL,
                    ToSectorId TEXT NOT NULL, -- Back to NOT NULL
                    MovementTime TEXT NOT NULL,
                    Type INTEGER NOT NULL,
                    IsGroupMovement INTEGER NOT NULL,
                    GroupId TEXT NULL,
                    Notes TEXT NULL,
                    FOREIGN KEY (VolunteerId) REFERENCES Volunteers (Id),
                    FOREIGN KEY (FromSectorId) REFERENCES Sectors (Id),
                    FOREIGN KEY (ToSectorId) REFERENCES Sectors (Id)
                );
                
                -- Copy only records with non-null ToSectorId
                INSERT INTO Movements_old 
                SELECT Id, VolunteerId, FromSectorId, ToSectorId, MovementTime, Type, IsGroupMovement, GroupId, Notes 
                FROM Movements 
                WHERE ToSectorId IS NOT NULL;
                
                DROP TABLE Movements;
                ALTER TABLE Movements_old RENAME TO Movements;
                
                -- Recreate indexes
                CREATE INDEX IX_Movements_MovementTime ON Movements (MovementTime);
                CREATE INDEX IX_Movements_GroupId ON Movements (GroupId) WHERE GroupId IS NOT NULL;
                
                PRAGMA foreign_keys = ON;
            ");
        }
    }
}
