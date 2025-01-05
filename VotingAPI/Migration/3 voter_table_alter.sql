alter table Voters 
drop column IF EXISTS [Password],
	COLUMN IF EXISTS [Address], 
	COLUMN IF EXISTS City, 
	COLUMN IF EXISTS State, 
	COLUMN IF EXISTS ZipCode, 
	COLUMN IF EXISTS Country, 
	COLUMN IF EXISTS NationalId