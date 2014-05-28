PRINT ''
PRINT 'STARTING CREATION OF DATABASE'
PRINT ''

:On Error exit

----INIT
--PRINT 'Initialize DB + Seed Method'

--PRINT 'FILLING OptimalEducation:'
----Update-Database -ProjectName:OptimalEducation -Script -SourceMigration: $InitialDatabase
--:r SQLScripts\Init\aspIdentity.sql
--GO

--PRINT 'FILLING OptimalEducation.DAL:'
----Update-Database -ProjectName:OptimalEducation.DAL -Script -SourceMigration: $InitialDatabase
--:r SQLScripts\Init\dalInit.sql
--GO

USE OptimalEducationDB

--FILLING DATABASE
PRINT 'FILLING DATABASE'

PRINT 'FILLING Cities:'
:r SQLScripts\dbo.Cities.data.sql
GO

PRINT 'FILLING ExamDisciplines:'
:r SQLScripts\dbo.ExamDisciplines.data.sql
GO

--PRINT 'FILLING Entrants:'
--:r SQLScripts\dbo.Entrants.data.sql
--GO

PRINT 'FILLING HigherEducationInstitutions:'
:r SQLScripts\dbo.HigherEducationInstitutions.data.sql
GO

PRINT 'FILLING Faculties:'
:r SQLScripts\dbo.Faculties.data.sql
GO

PRINT 'FILLING GeneralEducationLines:'
:r SQLScripts\dbo.GeneralEducationLines.data.sql
GO

PRINT 'FILLING EducationLines:'
:r SQLScripts\dbo.EducationLines.data.sql
GO

PRINT 'FILLING EducationLineRequirements:'
:r SQLScripts\dbo.EducationLineRequirements.data.sql
GO

--PRINT 'FILLING SchoolMarks:'
--:r SQLScripts\dbo.SchoolMarks.data.sql
--GO

--PRINT 'FILLING UnitedStateExams:'
--:r SQLScripts\dbo.UnitedStateExams.data.sql
--GO


PRINT ''
PRINT 'DATABASE CREATATION IS COMPLETE!'
