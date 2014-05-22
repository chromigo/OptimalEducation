PRINT ''
PRINT 'STARTING RECREATION OF DATABASE'
PRINT ''

:On Error exit

PRINT 'FILLING DATABASE'

PRINT 'FILLING Cities:'
:r SQLScripts\Table(separately)\dbo.Cities.data.sql
GO

--PRINT 'FILLING Entrants:'
--:r SQLScripts\Table(separately)\dbo.Entrants.data.sql
--GO

PRINT 'FILLING HigherEducationInstitutions:'
:r SQLScripts\Table(separately)\dbo.HigherEducationInstitutions.data.sql
GO

PRINT 'FILLING Faculties:'
:r SQLScripts\Table(separately)\dbo.Faculties.data.sql
GO

PRINT 'FILLING GeneralEducationLines:'
:r SQLScripts\Table(separately)\dbo.GeneralEducationLines.data.data.sql
GO

PRINT 'FILLING EducationLines:'
:r SQLScripts\Table(separately)\dbo.EducationLines.data.sql
GO

PRINT 'FILLING EducationLineRequirements:'
:r SQLScripts\Table(separately)\dbo.EducationLineRequirements.data.sql
GO

--PRINT 'FILLING SchoolMarks:'
--:r SQLScripts\Table(separately)\dbo.SchoolMarks.data.sql
--GO

--PRINT 'FILLING UnitedStateExams:'
--:r SQLScripts\Table(separately)\dbo.UnitedStateExams.data.sql
--GO


PRINT ''
PRINT 'DATABASE CREATE IS COMPLETE!'
