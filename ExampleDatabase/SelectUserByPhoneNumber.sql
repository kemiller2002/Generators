CREATE PROCEDURE [dbo].[SelectUserByPhoneNumber]
	@PhoneNumber dbo.PhoneNumber
AS
	SELECT * FROM Users WHERE PhoneNumber = @PhoneNumber

