using Model;
using Model.Controls;
using Model.Constants;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class UnitSupportBox : Listbox
{
    private readonly CityWindow _cityWindow;
    private float _oldScale = 0f;

    public UnitSupportBox(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        ItemSelected += OpenPopup;
    }

    public override void OnResize()
    {
        if (_oldScale != _cityWindow.Scale)
        {
            Definition = MakeListbox(_cityWindow);
            _oldScale = _cityWindow.Scale;
        }

        var pos = _cityWindow.CityWindowProps.UnitSupport.Box;
        Location = new(_cityWindow.LayoutPadding.Left + pos.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + pos.Y * _cityWindow.Scale);
        Width = (int)(pos.Width * _cityWindow.Scale);
        Height = (int)(pos.Height * _cityWindow.Scale);

        base.OnResize();
    }

    static ListboxDefinition MakeListbox(CityWindow cityWindow)
    {
        var units = GetSortedUnits(cityWindow)
            .ToList();
        var active = cityWindow.MainWindow.ActiveInterface;
        var properties = cityWindow.CityWindowProps.UnitSupport;

        List<ListboxGroup> groups = [];
        foreach (var unit in units)
        {
            var group = new ListboxGroup()
            {
                Elements = [new ListboxGroupElement { Unit = unit, Game = cityWindow.CurrentGameScreen.Game, ScaleIcon = UnitScaleFor(cityWindow, properties)}],
                Height = (int)Math.Ceiling(properties.Box.Height / properties.Rows * cityWindow.Scale)
            };
            groups.Add(group);
        }

        return new ListboxDefinition()
        {
            Rows = properties.Rows,
            Columns = properties.Columns,
            HorizontalStacking = true,
            Selectable = false,
            Looks = new ListboxLooks()
            {
                Font = active.Look.CityWindowFont,
                FontSize = Math.Max(10, (int)Math.Round(active.Look.CityWindowFontSize * cityWindow.Scale * 0.72f)),
                TextColorFront = Color.Black,
                TextColorShadow = Color.Gray
            },
            Groups = groups
        };
    }

    private static IEnumerable<Model.Core.Units.Unit> GetSortedUnits(CityWindow cityWindow) =>
        cityWindow.City.SupportedUnits
            .OrderBy(unit => unit.AiRole == AiRoleType.Settle ? 0 : unit.AttackBase > 0 ? 1 : 2)
            .ThenBy(unit => unit.Domain)
            .ThenByDescending(unit => unit.DefenseBase)
            .ThenByDescending(unit => unit.AttackBase)
            .ThenBy(unit => unit.Type);

    private void OpenPopup(object? sender, ListboxSelectionEventArgs args)
    {
        var units = GetSortedUnits(_cityWindow).ToList();
        if (args.Index < 0 || args.Index >= units.Count)
        {
            return;
        }

        CityUnitMenu.Show(_cityWindow, units[args.Index]);
    }

    /// <summary>
    /// How large to draw a unit in one cell of this box. The old value was a fixed
    /// 0.82 that took no account of the city window's scale, so at the default 1.5
    /// the box grew and the units in it did not. This fills the cell it is given
    /// and grows with the window.
    /// </summary>
    private static float UnitScaleFor(CityWindow cityWindow, UnitBox properties)
    {
        var unit = cityWindow.MainWindow.ActiveInterface.UnitImages.UnitRectangle;
        if (unit.Width <= 0 || unit.Height <= 0 || properties.Rows <= 0 || properties.Columns <= 0)
        {
            return cityWindow.Scale;
        }

        var cellWidth = properties.Box.Width / properties.Columns;
        var cellHeight = properties.Box.Height / properties.Rows;
        var fit = Math.Min(cellWidth / unit.Width, cellHeight / unit.Height);
        return Math.Max(0.1f, fit * cityWindow.Scale);
    }
}
