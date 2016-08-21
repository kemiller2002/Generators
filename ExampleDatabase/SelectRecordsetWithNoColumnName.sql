CREATE PROCEDURE [dbo].[SelectRecordsetWithNoColumnName]
	@param1 int = 0,
	@param2 int
AS
	SELECT @param1, @param2

	Return 0