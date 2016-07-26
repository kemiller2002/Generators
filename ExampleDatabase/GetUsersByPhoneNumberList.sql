CREATE PROCEDURE [dbo].[GetUsersByPhoneNumberList]
	@PhoneNumbers dbo.PhoneNumbers READONLY

AS
	SELECT * FROM Users 
		JOIN @PhoneNumbers ph ON Users.PhoneNumber = ph.PhoneNumber
