CREATE PROCEDURE [dbo].[SelectUsers]
	@Id INT = NULL
	,@FirstName VARCHAR(100)
AS

	SELECT Id, FirstName, LastName, EmailAddress, PhoneNumber FROM Users	
		WHERE
			Id = @Id OR @Id IS NULL
