-- Convert normal user to Admin user by using email address

Declare @adminRoleId varchar(100) 
Declare @userEmail varchar(100) 

SET @userEmail = '' -- Email address need to convert user to admin

SET @adminRoleId = ( SELECT Id From AspNetRoles Where Name = 'Admin')

Update AspNetUserRoles Set RoleId = @adminRoleId Where UserId In (Select Id From AspNetUsers Where Email = @userEmail)