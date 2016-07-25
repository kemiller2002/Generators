CREATE PROCEDURE [dbo].[SelectUsers]
	@Id INT = NULL
AS

	SELECT Id, FirstName, LastName, EmailAddress, PhoneNumber FROM Users	
		WHERE
			Id = @Id OR @Id IS NULL
