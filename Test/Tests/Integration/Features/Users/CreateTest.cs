﻿using Api.Features.Users;
using Bogus;
using Core.Enums;
using FluentAssertions;
using Nudes.Retornator.Core;
using System.Net.Http.Json;
using Xunit;

namespace Test.Tests.Integration.Features.Users
{
    public sealed class CreateTest : BaseTest
    {
        public CreateTest(ServerFixture serverFixture) : base(serverFixture)
        {

        }

        [Fact]
        public async Task Should_Create_User_On_Database_With_Correct_Data_And_Password()
        {
            var command = new Faker<Create.Command>()
                .RuleFor(d => d.Name, d => d.Person.FirstName)
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .RuleFor(d => d.Profile, d => d.PickRandom<Profile>())
                .Generate();

            var response = await client.PostAsJsonAsync($"users", command, default);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Not_Create_User_On_Database_With_Empty_Fields()
        {
            var command = new Faker<Create.Command>()
               .RuleFor(d => d.Name, d => string.Empty)
               .RuleFor(d => d.Email, d => string.Empty)
               .RuleFor(d => d.Profile, d => d.PickRandom<Profile>())
               .Generate();
            var response = await client.PostAsJsonAsync($"users", command, default);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Not_Create_User_With_Invalid_Email_Or_Password()
        {
            var command = new Faker<Create.Command>()
               .RuleFor(d => d.Name, d => d.Person.FirstName)
               .RuleFor(d => d.Profile, d => d.PickRandom<Profile>())
               .RuleFor(d => d.Email, d => d.Person.FirstName)
               .Generate();

            var response = await client.PostAsJsonAsync($"users", command, default);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Not_Create_User_With_Email_Already_Existent()
        {
            var command = new Faker<Create.Command>()
                .RuleFor(d => d.Name, d => d.Person.FirstName)
                .RuleFor(d => d.Profile, d => d.PickRandom<Profile>())
                .RuleFor(d => d.Email, d => d.Internet.Email())
                .Generate();

            var response = await client.PostAsJsonAsync($"users", command, default);
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
