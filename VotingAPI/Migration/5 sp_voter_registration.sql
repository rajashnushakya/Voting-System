DROP PROCEDURE IF EXISTS sp_voter_registration;
GO

create procedure sp_voter_registration
	@FullName nvarchar(255),
	@FatherName nvarchar(255),
	@MotherName nvarchar(255),
	@GrandFatherName nvarchar(255),
	@GrandMotherName nvarchar(255),
	@Email nvarchar(255),
	@Gender nvarchar(20),
	@DateOfBirth date,
	@HouseNumber int,
	@WardNumber int,
	@StreetName nvarchar(255),
	@Municipality nvarchar(255),
	@District nvarchar(255),
	@Province nvarchar(255),
	@UserName nvarchar(255),
	@Password nvarchar(255),
	@RoleId int
as
begin
	begin try
		begin tran;
			declare @VoterId int

			insert into Voters (FullName, FatherName, MotherName, GrandFatherName, GrandMotherName, Email, DateOfBirth, Gender) values
				(@FullName, @FatherName, @MotherName, @GrandFatherName, @GrandMotherName, @Email, @DateOfBirth, @Gender)

			set @voterId = @@IDENTITY

			insert into [Address](VoterId, HouseNumber,WardNumber,StreetName, Municipality, District, Province) values
				(@VoterId, @HouseNumber,@WardNumber,@StreetName, @Municipality, @District, @Province)

			insert into Users (username, email, password, full_name, role_id) values
			(@UserName, @Email, @Password, @FullName, @RoleId)

			select 0 as status, 'Voter registered successfully', null as sys_error, null as error_severity;
		commit tran;
	end try
	begin catch
		IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Capture error details
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;
        
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        
        -- Log or handle the error
        SELECT 100 as status, 'Unexpected error occurred.' as error_msg, @ErrorMessage as sys_error, @ErrorSeverity as error_severity;
	end catch
end

