using RhyCiv.Engine.Production;
using RhyCiv.Engine.Enums;
using Model.Constants;
using Model.Core;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RhyCiv.Tests.Production;

public class UnitProductionOrderTests
{
    [Fact]
    public void CompleteProductionCreatesSupportedUnitInCity()
    {
        var civilization = new Civilization { Id = 0, Government = 0 };
        var city = CreateCity(civilization, size: 2);
        var rules = new Rules
        {
            Governments = [new Government { NumberOfFreeUnitsPerCity = 0 }]
        };
        var definition = new UnitDefinition
        {
            Name = "Warriors",
            Type = 0,
            AIrole = AiRoleType.Attack,
            Domain = UnitGas.Ground,
            Cost = 1
        };

        var completed = new UnitProductionOrder(definition, 0).CompleteProduction(city, rules);

        Assert.True(completed);
        var unit = Assert.Single(civilization.Units);
        Assert.Equal(0, unit.Id);
        Assert.Equal(city.X, unit.X);
        Assert.Equal(city.Y, unit.Y);
        Assert.Same(city, unit.HomeCity);
        Assert.Same(city.Location, unit.CurrentLocation);
        Assert.Same(civilization, unit.Owner);
        Assert.True(unit.NeedsSupport);
    }

    [Fact]
    public void CompleteProductionDoesNotConsumeSizeOneCityForSettler()
    {
        var civilization = new Civilization { Id = 0, Government = 0 };
        var city = CreateCity(civilization, size: 1);
        var rules = new Rules { Governments = [new Government()] };
        var definition = new UnitDefinition
        {
            Name = "Settlers",
            AIrole = AiRoleType.Settle,
            Domain = UnitGas.Ground,
            Cost = 4
        };

        var completed = new UnitProductionOrder(definition, 0).CompleteProduction(city, rules);

        Assert.False(completed);
        Assert.Equal(1, city.Size);
        Assert.Empty(civilization.Units);
    }

    [Fact]
    public void CompleteProductionConsumesOnePopulationForSettler()
    {
        var civilization = new Civilization { Id = 0, Government = 0 };
        var city = CreateCity(civilization, size: 2);
        var rules = new Rules { Governments = [new Government()] };
        var definition = new UnitDefinition
        {
            Name = "Settlers",
            AIrole = AiRoleType.Settle,
            Domain = UnitGas.Ground,
            Cost = 4
        };

        var completed = new UnitProductionOrder(definition, 0).CompleteProduction(city, rules);

        Assert.True(completed);
        Assert.Equal(1, city.Size);
        Assert.Single(civilization.Units);
    }

    private static City CreateCity(Civilization owner, int size)
    {
        var map = new Map(true, 0);
        var terrain = new Terrain
        {
            Name = "Grassland",
            Type = TerrainType.Grassland,
            Specials = []
        };
        var tile = new Tile(4, 3, terrain, 0, map, 2, [true]);
        var city = new City
        {
            X = tile.X,
            Y = tile.Y,
            Location = tile,
            Owner = owner,
            Size = size
        };
        owner.Cities.Add(city);
        tile.CityHere = city;
        return city;
    }
}
