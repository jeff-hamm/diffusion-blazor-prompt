using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtsBlazor.Shared.ViewModels;

public interface IButtonPressListener
{
    public Task OnButtonPressed(GamepadButton button);
}

[Flags]
public enum ConfirmationResult
{
    None = 0,
    Confirm = 0x1,
    Cancel = 0x2,
    Timeout = 0x4 | Cancel,
    Replaced = 0x8 | Cancel,
}
public enum GamepadButton
{
    Select,
    Right,
    LShoulder,
    RShoulder,
    Left,
    Up,
    Down,
    Start,
    Primary,
    Secondary,
}