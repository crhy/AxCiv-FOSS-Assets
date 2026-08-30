using System;
using System.Linq;
using Civ2engine.Enums;
using Model;
using Model.Constants;
using Model.Controls;
using Model.Core.Production;
using Model.Core.Units;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Images;

namespace Civ2engine.Production
{
    public class UnitProductionOrder(UnitDefinition unitDefinition, int index) : ProductionOrder(unitDefinition.Cost,
        ItemType.Unit,
        index, unitDefinition.Prereq, unitDefinition.CivCanBuild, unitDefinition.Until)
    {
        public override string Title => unitDefinition.Name;

        public override bool CompleteProduction(City city, Rules rules)
        {
            if (unitDefinition.AIrole == AiRoleType.Settle && city.Size == 1)
            {
                return false;
            }

            var veteran = city.Improvements.Any(i =>
                i.Effects.ContainsKey(Effects.Veteran) &&
                i.Effects[Effects.Veteran] == (int)unitDefinition.Domain);

            var unit = new Unit
            {
                Id = city.Owner.Units.Any() ? city.Owner.Units.Max(u => u.Id) + 1 : 0,
                X = city.X,
                Y = city.Y,
                HomeCity = city,
                CurrentLocation = city.Location,
                Owner = city.Owner,
                TypeDefinition = unitDefinition,
                Veteran = veteran,
                Order = (int)OrderType.NoOrders
            };
            unit.Owner.Units.Add(unit);

            if (unitDefinition.AIrole == AiRoleType.Settle)
            {
                city.Size -= 1;
            }

            var government = rules.Governments[city.Owner.Government];
            if (!unit.FreeSupport(government.UnitTypesAlwaysFree))
            {
                city.SetUnitSupport(government);
            }

            return true;
        }

        public override IImageSource? GetIcon(IUserInterface activeInterface)
        {
            if (activeInterface.UnitImages.Units is { } unitImages &&
                unitDefinition.Type >= 0 && unitDefinition.Type < unitImages.Length)
            {
                return unitImages[unitDefinition.Type].MapImage ?? unitImages[unitDefinition.Type].UiImage;
            }

            return activeInterface.PicSources.TryGetValue("unit", out var unitIcons) &&
                   unitDefinition.Type >= 0 && unitDefinition.Type < unitIcons.Length
                ? unitIcons[unitDefinition.Type]
                : null;
        }

        private bool HasFossArtIcon(IUserInterface activeInterface)
        {
            return activeInterface.UnitImages.Units is { } unitImages &&
                   unitDefinition.Type >= 0 && unitDefinition.Type < unitImages.Length &&
                   unitImages[unitDefinition.Type].MapImage != null;
        }

        public override bool IsValidBuild(City city)
        {
            return unitDefinition.Domain != UnitGas.Sea || city.IsNextToOcean();
        }

        public override string GetDescription()
        {
            return unitDefinition.Name;
        }

        public override ListboxGroup GetBuildListEntry(IUserInterface activeInterface, City city)
        {
            var turns = Math.Max(1, (int)Math.Ceiling(Math.Max(0, 10 * unitDefinition.Cost - city.ShieldsProgress) /
                                                      (decimal)Math.Max(1, city.Production)));
            return new ListboxGroup
            {
                Elements =
                [
                    new() { Icon = GetIcon(activeInterface), Width = 70, ScaleIcon = HasFossArtIcon(activeInterface) ? 0.028f : 0.75f },
                    new() { Text = unitDefinition.Name, Width = 200, TextSizeOverride = 18, VerticalAlignment = VerticalAlignment.Center },
                    new()
                    {
                        Text = $"({turns} Turns, ADM: {unitDefinition.Attack}/{unitDefinition.Defense}/{unitDefinition.Move / 3} " +
                               $"HP: {unitDefinition.Hitp / 10}/{unitDefinition.Firepwr})",
                        TextSizeOverride = 16,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                ],
                Height = 38
            };
        }
    }
}
