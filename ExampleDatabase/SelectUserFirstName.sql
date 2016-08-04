CREATE PROCEDURE [dbo].[SelectUserFirstName]

	@PhoneNumber PhoneNumber ,
	@FirstName VARCHAR(10) = NULL OUTPUT


AS

	SELECT @FirstName = FirstName FROM Users WHERE @PhoneNumber = PhoneNumber


	
