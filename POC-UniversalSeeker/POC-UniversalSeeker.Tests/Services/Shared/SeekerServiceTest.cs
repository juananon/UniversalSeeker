using LinqKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using POC_UniversalSeeker.Entities.Users;
using POC_UniversalSeeker.Services.Shared;
using POC_UniversalSeeker.Services.Shared.Interfaces;
using POC_UniversalSeeker.Utils.Helpers;
using POC_UniversalSeeker.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using POC_UniversalSeeker.Entities.Users.Enums;

namespace POC_UniversalSeeker.Tests.Services.Shared
{
    [TestClass]
    public class SeekerServiceTest
    {
        private ISeekerService _seekerService;
        private List<User> _users;
        private Role _role;
        private Role _roleBis;

        public void Setup()
        {
            _seekerService = new SeekerService();

            _role = new Role { Id = (int)RoleEnum.Admin, Name = "Admin", Description = "Administrator" };
            _roleBis = new Role { Id = (int)RoleEnum.SuperAdmin, Name = "SuperAdmin", Description = "Super Administrator" };
            _users = new List<User>
            {
                new User { Id = 1, Name = "Juan", SurName = "Añón", Mail = "juananon@gmail.com", BirthDate = DateTime.Now.AddYears(-33), Password = "pass", Role = _role},
                new User { Id = 2, Name = "Juan Francisco", SurName = "García", Mail = "juanf@gmail.com", BirthDate = DateTime.Now.AddYears(-40), Password = "pass", Role = _roleBis},
                new User { Id = 2, Name = "Gonzalo", SurName = "Pérez", Mail = "gonzalop@gmail.com", BirthDate = DateTime.Now.AddYears(-44), Password = "pass", Role = _roleBis }
            };
        }

        [TestMethod]
        public void Given_a_filtrable_elements_should_return_only_marked_properties()
        {
            Setup();

            var filters = _seekerService.GetFilters<User>("");

            Assert.IsNotNull(filters);
        }

        [TestMethod]
        public void Given_a_expression_should_filter_data_correctly()
        {
            Setup();

            var query = PredicateBuilder.New<User>();
            query = query.And(ExpressionBuilderHelper.CreateExpression<User>("Name", "Juan", OperatorEnum.Contains));
            query = query.And(ExpressionBuilderHelper.CreateExpression<User>("BirthDate", "01/01/1983", OperatorEnum.GreaterThan));
            query = query.And(ExpressionBuilderHelper.CreateExpression<User>("Role.Name", "Admin", OperatorEnum.Equal));

            var usersFiltered = _users.Where(query);
            
            Assert.IsNotNull(usersFiltered);

        }
    }
}
