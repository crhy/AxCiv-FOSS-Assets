using Civ2engine;
using Model;
using Model.Controls;
using Model.Core.Cities;
using Civ2engine.Enums;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityCitizensBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly CityWindowLayout _props;
    private readonly IUserInterface _active;
    private readonly ImageBox[] _icons;
    private readonly City _city;
    private readonly int _epoch, _specialistsStart;
    private readonly int[] _citizenIndex;
    
    public CityCitizensBox(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _city = _cityWindow.City;
        _active = cityWindow.MainWindow.ActiveInterface;
        _props = _cityWindow.CityWindowProps;

        _epoch = _city.Owner.Epoch;
        _specialistsStart = _city.Size - (_city.NoOfSpecialistsx4 / 4);
        _citizenIndex = new int[_city.Size];
        _icons = new ImageBox[_city.Size];
        for (var i = 0; i < _icons.Length; i++)
        {
            if (i >= _specialistsStart)
            {
                var specialistIndex = i - _specialistsStart;
                var specialists = _city.GetSpecialistTypes();
                _citizenIndex[i] = specialistIndex < specialists.Length
                    ? (int)specialists[specialistIndex]
                    : (int)PeopleType.Elvis;
            }

            _icons[i] = new ImageBox(_cityWindow, _active.PicSources["people"][0], eventTransparent: false);
            _icons[i].Click += OnClick;
            Controls.Add(_icons[i]);
        }
    }

    public override void OnResize()
    {
        var people = _city.GetPeopleTypes(_cityWindow.CurrentGameScreen.Game);
        var pos = _props.CitizensBox.ScaleAll(_cityWindow.Scale);
        Location = new(_cityWindow.LayoutPadding.Left + pos.X, _cityWindow.LayoutPadding.Top + pos.Y);
        Width = (int)pos.Width;
        Height = (int)pos.Height;
        base.OnResize();

        var sourceWidth = Images.GetImageWidth(_active.PicSources["people"][0], _active);
        var sourceHeight = Images.GetImageHeight(_active.PicSources["people"][0], _active);
        const float citizenHeight = 30f;
        var baseIconScale = citizenHeight / Math.Max(1, sourceHeight);
        var iconWidth = Math.Max(1, (int)MathF.Round(sourceWidth * baseIconScale));
        int spacing = 0;
        if (_city.Size > 2)
        {
            spacing = Math.Min(((int)_props.CitizensBox.Width - 4 - iconWidth) / (_city.Size - 1), iconWidth + 1);
        }

        for (var i = 0; i < _icons.Length; i++)
        {
            if (i < _specialistsStart)
            {
                _citizenIndex[i] = (int)people[i] + i % 2;
            }
            else
            {
                _citizenIndex[i] = (int)_city.GetSpecialistTypes()[i - _specialistsStart];
            }
            _icons[i].Image = [_active.PicSources["people"][_citizenIndex[i] + 11 * _epoch]];

            _icons[i].Location = new((2 + i * spacing) * _cityWindow.Scale, 7 * _cityWindow.Scale);
            _icons[i].Scale = baseIconScale * Math.Min(_cityWindow.Scale, 1.25f);
        }

        foreach (var child in Controls)
        {
            child.OnResize();
        }
    }

    private void OnClick(object? sender, MouseEventArgs e) 
    {
        // Change specialist
        var index = Array.IndexOf(_icons, sender);
        if (index >= _specialistsStart)
        {
            ChangeSpecialist(index, 1, IsShiftDown());
        }
    }

    public override bool OnMouseWheel(float amount)
    {
        var mouse = Input.GetMousePosition();
        var index = Array.FindIndex(_icons,
            icon => ShapeHelper.CheckCollisionPointRec(mouse, icon.Bounds));
        if (index < _specialistsStart)
        {
            return false;
        }

        ChangeSpecialist(index, amount > 0 ? 1 : -1, IsShiftDown());
        return true;
    }

    private void ChangeSpecialist(int citizenIndex, int direction, bool changeAll)
    {
        var specialistIndex = citizenIndex - _specialistsStart;
        var current = (int)_city.GetSpecialistTypes()[specialistIndex];
        var next = (int)PeopleType.Elvis +
                   ((current - (int)PeopleType.Elvis + direction + 3) % 3);

        if (changeAll)
        {
            for (var i = 0; i < _city.NoOfSpecialistsx4 / 4; i++)
            {
                _city.SetSpecialistType(i, (PeopleType)next);
            }
        }
        else
        {
            _city.SetSpecialistType(specialistIndex, (PeopleType)next);
        }

        _cityWindow.UpdateProduction();
        OnResize();
    }

    private static bool IsShiftDown() =>
        Input.IsKeyDown(KeyboardKey.LeftShift) || Input.IsKeyDown(KeyboardKey.RightShift);
}
