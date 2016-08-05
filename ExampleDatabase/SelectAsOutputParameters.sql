CREATE PROCEDURE [dbo].[SelectAsOutputParameters]
	@Int INT = NULL OUTPUT
	, @Guid UNIQUEIDENTIFIER = NULL OUTPUT
	, @VarChar VARCHAR(100) = NULL OUTPUT
	, @nVarchar NVARCHAR(100) = NULL OUTPUT
	, @smallInt SMALLINT = NULL OUTPUT
	, @timyInt TINYINT = NULL OUTPUT
	, @DateTime DATETIME = NULL OUTPUT
	, @DateTime2 DATETIME2(2) = NULL OUTPUT
	, @SmallDateTime SMALLDATETIME = NULL OUTPUT
	, @float FLOAT = NULL OUTPUT
	, @numeric NUMERIC = NULL OUTPUT
	, @real REAL = NULL OUTPUT

AS


SET @Int = 23
SET @Guid = NEWID()
SET @VarChar = 'asdflkajsdlfkjasdf'

SET @nVarchar = '34rqwerqwerqwer'

SET @smallInt = 2
SET @timyInt = 3

SET @DateTime = CAST ('10/10/2012' AS DATETIME)

SET @DateTime2 = CAST ('10/10/2012' AS DATETIME2)

SET @SmallDateTime = CAST ('10/10/2012' AS smalldatetime)

SET @float = 1.0
SET @numeric = 23.3
SET @real = 32323.234234234

