using RhyCiv.Engine;
using Model.Constants;
using Model.Core;
using Model.Core.Cities;
using Model.Core.Units;

namespace RhyCiv.Tests;

/// <summary>
/// Losing every city is not by itself defeat — a surviving settler can found
/// another. Any other unit cannot, so a civilisation reduced to a couple of
/// warriors has lost, and used to be left playing on with no possible future and
/// no dialog to say so. Reported after the barbarians took a player's last city.
/// </summary>
public class GameEliminationTests
{
    [Fact]
    public void NoCitiesAndNoSettler_IsDefeat()
    {
        var civ = CivilisationWith(Unit(AiRoleType.Attack), Unit(AiRoleType.Defend));

        Assert.True(Game.IsDefeated(civ));
    }

    [Fact]
    public void NoCitiesButASettlerSurvives_IsNotDefeat()
    {
        var civ = CivilisationWith(Unit(AiRoleType.Attack), Unit(AiRoleType.Settle));

        Assert.False(Game.IsDefeated(civ));
    }

    [Fact]
    public void ADeadSettler_DoesNotCount()
    {
        var settler = Unit(AiRoleType.Settle);
        settler.Dead = true;
        var civ = CivilisationWith(settler, Unit(AiRoleType.Attack));

        Assert.True(Game.IsDefeated(civ));
    }

    [Fact]
    public void HoldingACity_IsNeverDefeat()
    {
        // Even with nothing left to move: the city can build a unit next turn.
        var civ = CivilisationWith();
        civ.Cities.Add(new City { Owner = civ, Name = "Last Stand" });

        Assert.False(Game.IsDefeated(civ));
    }

    [Fact]
    public void NoCitiesAndNoUnitsAtAll_IsDefeat()
    {
        Assert.True(Game.IsDefeated(CivilisationWith()));
    }

    private static Civilization CivilisationWith(params Unit[] units)
    {
        var civ = new Civilization { Id = 1 };
        civ.Units.AddRange(units);
        foreach (var unit in units)
        {
            unit.Owner = civ;
        }
        return civ;
    }

    private static Unit Unit(AiRoleType role) => new()
    {
        TypeDefinition = new UnitDefinition
        {
            AIrole = role,
            Flags = Enumerable.Repeat(false, 13).ToArray(),
        },
    };
}
