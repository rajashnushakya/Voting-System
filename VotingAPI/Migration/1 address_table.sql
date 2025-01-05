

create table Address
(
	Id int primary key identity(1,1),
	VoterId int foreign key references Voters(VoterId),
	HouseNumber int,
	WardNumber int,
	StreetName nvarchar(255),
	Municipality nvarchar(255),
	District nvarchar(255),
	Province nvarchar(255)
)