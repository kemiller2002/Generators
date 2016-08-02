CREATE PROCEDURE [dbo].[SelectUserFirstName]

	@PhoneNumber PhoneNumber ,
	@FirstName VARCHAR(10) OUTPUT


AS

	SELECT @FirstName = FirstName FROM Users WHERE @PhoneNumber = PhoneNumber


	
