PRINT ''
PRINT 'STARTING CREATION OF DATABASE'
PRINT ''

:On Error exit

USE OptimalEducationDataBase

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
