﻿using System.Numerics;

namespace VotingAPI.Model
{
    public class Voter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string GrandFatherName { get; set; }
        public string GrandMotherName { get; set; }
        public string Email { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalId { get; set; }

        public Address Address { get; set; }

        public User User { get; set; }
    }


    public class Address
    {
        public int Id { get; set; }
        public int VoterId { get; set; }
        public int HouseNumber { get; set; }
        public int WardId { get; set; }
        public string StreetName { get; set; }
        public int MunicipalityId { get; set; }
        public int DistrictId { get; set; }
        public string Province { get; set; }
    }

  
    public class User
    {
        public int RoleId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
