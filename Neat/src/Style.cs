using System.Numerics;
using ImGuiNET;

namespace Neat;

public class Style
{
    private static Vector4 FromHex(int hex)
    {
        float r = ((hex >> 16) & 0xFF) / 255f;
        float g = ((hex >> 8) & 0xFF) / 255f;
        float b = (hex & 0xFF) / 255f;
        float a = ((hex >> 24) & 0xFF) / 255f;

        return new Vector4(r, g, b, a > 0 ? a : 1f);
    }
    
    public static Theme ActiveTheme;

    public static int RootSize = 38;
    
    public static Vector4 AxisColorX = FromHex(0xE50048);
    public static Vector4 AxisColorY = FromHex(0x83CD38);
    public static Vector4 AxisColorZ = FromHex(0x458AF2);
    
    public static void Apply(Theme theme)
    {
        ActiveTheme = theme;

        var style = ImGui.GetStyle();
        style.FrameRounding = 4;
        style.FramePadding = new Vector2(4,2);
        style.ChildRounding = 4;
        style.WindowRounding = 4;
        style.CellPadding = Vector2.Zero;
        style.PopupRounding = 4;
        var colors = style.Colors;

        colors[(int)ImGuiCol.Text] = theme.Text;
        colors[(int)ImGuiCol.TextLink] = theme.Text;
        colors[(int)ImGuiCol.TextDisabled] = theme.TextSecondary;
        
        colors[(int)ImGuiCol.WindowBg] = theme.Background;
        colors[(int)ImGuiCol.FrameBg] = theme.SurfaceBackground;
        colors[(int)ImGuiCol.ChildBg] = theme.Background;
        colors[(int)ImGuiCol.TitleBg] = theme.Background;
        colors[(int)ImGuiCol.PopupBg] = theme.Background;
        colors[(int)ImGuiCol.MenuBarBg] = theme.Background;
        
        colors[(int)ImGuiCol.CheckMark] = theme.AppAccent;
        colors[(int)ImGuiCol.ButtonActive] = theme.Active;
        colors[(int)ImGuiCol.ButtonHovered] = theme.Active;
        
        colors[(int)ImGuiCol.Border] = theme.BorderSurface;
        colors[(int)ImGuiCol.Separator] = theme.BorderSurface;
    }

    public class Theme
    {
        public Vector4 AccentBadge;
        public Vector4 Active;
        public Vector4 AltSurfaceBackground;
        public Vector4 AppAccent;
        public Vector4 Background;
        public Vector4 Black;
        public Vector4 BlackCol;
        public Vector4 Blue;
        public Vector4 BorderAltSurface;
        public Vector4 BorderControl;
        public Vector4 BorderSurface;
        public Vector4 Confirmation;
        public Vector4 ControlBackground;
        public Vector4 DBlue;
        public Vector4 DGrey;
        public Vector4 Error;
        public Vector4 Green;
        public Vector4 Inactive;
        public Vector4 LGrey;
        public Vector4 MBlue;
        public Vector4 MGrey;
        public Vector4 OnAccentBadge;
        public Vector4 OnActive;
        public Vector4 OnAltBackground;
        public Vector4 OnBackground;
        public Vector4 OnConfirmation;
        public Vector4 OnError;
        public Vector4 OnPrimary;
        public Vector4 OnSurface;
        public Vector4 Orange;
        public Vector4 Primary;
        public Vector4 Purple;
        public Vector4 PWhite;
        public Vector4 Red;
        public Vector4 ShadowColor;
        public Vector4 SurfaceBackground;
        public Vector4 Text;
        public Vector4 TextSecondary;
        public Vector4 TextualBackground;
        public Vector4 White;
        public Vector4 XLGrey;
        public Vector4 Yellow;
    }

    public static class Light
    {
        private static readonly Vector4 PWhite = FromHex(0xFFFFFF);
        private static readonly Vector4 WhiteCol = new Vector4(252, 252, 252, 255) / 255;
        private static readonly Vector4 White = WhiteCol;
        private static readonly Vector4 BlackCol = new Vector4(37, 37, 37, 255) / 255;
        private static readonly Vector4 Black = BlackCol;
        private static readonly Vector4 DGrey = FromHex(0x737373);
        private static Vector4 DarkMGrey = FromHex(0xB6B6B6);
        private static readonly Vector4 MGrey = FromHex(0xC4C4C4);
        private static readonly Vector4 LGrey = FromHex(0xE6E6E6);
        private static readonly Vector4 XLGreyCol = new Vector4(242, 242, 242, 255) / 255;
        private static readonly Vector4 XLGrey = XLGreyCol;
        private static readonly Vector4 Orange = FromHex(0xC65306);
        private static readonly Vector4 Blue = FromHex(0x0865C3);
        private static readonly Vector4 LBlue = FromHex(0x4297FF);
        private static readonly Vector4 XLBlue = FromHex(0xAAD0F5);
        private static readonly Vector4 Red = FromHex(0xF01346);
        private static readonly Vector4 Green = FromHex(0x15B76C);
        private static readonly Vector4 Purple = FromHex(0x6446E6);
        private static readonly Vector4 Yellow = FromHex(0xFF9E01);

        private static readonly Vector4 Text = Black;
        private static readonly Vector4 TextSecondary = DGrey;
        private static readonly Vector4 Primary = Blue;
        private static readonly Vector4 Active = LBlue;
        private static readonly Vector4 Inactive = XLBlue;
        private static readonly Vector4 AppAccent = Primary;

        private static readonly Vector4 Background = XLGrey;
        private static readonly Vector4 ControlBackground = LGrey;
        private static readonly Vector4 TextualBackground = White;
        private static readonly Vector4 SurfaceBackground = White;
        private static readonly Vector4 AltSurfaceBackground = DGrey;

        private static readonly Vector4 Error = Red;
        private static readonly Vector4 Confirmation = Green;
        private static readonly Vector4 AccentBadge = Orange;

        private static readonly Vector4 OnPrimary = PWhite;
        private static readonly Vector4 OnActive = PWhite;
        private static readonly Vector4 OnBackground = Black;
        private static readonly Vector4 OnAltBackground = PWhite;
        private static readonly Vector4 OnSurface = Black;
        private static readonly Vector4 OnError = White;
        private static readonly Vector4 OnConfirmation = White;
        private static readonly Vector4 OnAccentBadge = PWhite;

        private static readonly Vector4 BorderSurface = LGrey;
        private static readonly Vector4 BorderAltSurface = LGrey;
        private static readonly Vector4 BorderControl = DGrey;

        private static readonly Vector4 ShadowColor = BlackCol with { W = 0.3f };

        public static Theme GetTheme()
        {
            return new Theme
            {
                AccentBadge = AccentBadge,
                Active = Active,
                AltSurfaceBackground = AltSurfaceBackground,
                AppAccent = AppAccent,
                Background = Background,
                Black = Black,
                BlackCol = BlackCol,
                Blue = Blue,
                BorderAltSurface = BorderAltSurface,
                BorderControl = BorderControl,
                BorderSurface = BorderSurface,
                Confirmation = Confirmation,
                ControlBackground = ControlBackground,
                DBlue = Blue,
                DGrey = DGrey,
                Error = Error,
                Green = Green,
                Inactive = Inactive,
                LGrey = LGrey,
                MBlue = Blue,
                MGrey = MGrey,
                OnAccentBadge = OnAccentBadge,
                OnActive = OnActive,
                OnAltBackground = OnAltBackground,
                OnBackground = OnBackground,
                OnConfirmation = OnConfirmation,
                OnError = OnError,
                OnPrimary = OnPrimary,
                OnSurface = OnSurface,
                Orange = Orange,
                Primary = Primary,
                Purple = Purple,
                PWhite = PWhite,
                Red = Red,
                ShadowColor = ShadowColor,
                SurfaceBackground = SurfaceBackground,
                Text = Text,
                TextSecondary = TextSecondary,
                TextualBackground = TextualBackground,
                White = White,
                XLGrey = XLGrey,
                Yellow = Yellow
            };
        }
    }

    public static class Dark
    {
        private static readonly Vector4 PWhite = FromHex(0xFFFFFF);

        private static readonly Vector4 WhiteCol = new Vector4(240, 240, 240, 255) / 255; // soft white text
        private static readonly Vector4 White = WhiteCol;

        private static readonly Vector4 BlackCol = new Vector4(18, 18, 18, 255) / 255; // not pure black
        private static readonly Vector4 Black = BlackCol;

        private static readonly Vector4 DGrey = FromHex(0x9A9A9A);   // secondary text
        private static readonly Vector4 DarkMGrey = FromHex(0x2A2A2A);
        private static readonly Vector4 MGrey = FromHex(0x3A3A3A);
        private static readonly Vector4 LGrey = FromHex(0x4A4A4A);
        private static readonly Vector4 XLGrey = FromHex(0x1E1E1E);


        private static readonly Vector4 Orange = FromHex(0xE07A2F);
        private static readonly Vector4 Blue = FromHex(0x3A8DFF);
        private static readonly Vector4 LBlue = FromHex(0x63A4FF);
        private static readonly Vector4 XLBlue = FromHex(0x1F3A5C);
        private static readonly Vector4 Red = FromHex(0xFF4D6D);
        private static readonly Vector4 Green = FromHex(0x2ECC8B);
        private static readonly Vector4 Purple = FromHex(0x8B74FF);
        private static readonly Vector4 Yellow = FromHex(0xFFB020);


        private static readonly Vector4 Text = White;
        private static readonly Vector4 TextSecondary = DGrey;

        private static readonly Vector4 Primary = Blue;
        private static readonly Vector4 Active = LBlue;
        private static readonly Vector4 Inactive = XLBlue;
        private static readonly Vector4 AppAccent = Primary;

        private static readonly Vector4 Background = FromHex(0x181818);
        private static readonly Vector4 ControlBackground = FromHex(0x242424);
        private static readonly Vector4 TextualBackground = FromHex(0x2C2C2C);
        private static readonly Vector4 SurfaceBackground = FromHex(0x202020);
        private static readonly Vector4 AltSurfaceBackground = FromHex(0x303030);


        private static readonly Vector4 Error = Red;
        private static readonly Vector4 Confirmation = Green;
        private static readonly Vector4 AccentBadge = Orange;
        
        private static readonly Vector4 OnPrimary = PWhite;
        private static readonly Vector4 OnActive = PWhite;
        private static readonly Vector4 OnBackground = White;
        private static readonly Vector4 OnAltBackground = White;
        private static readonly Vector4 OnSurface = White;
        private static readonly Vector4 OnError = PWhite;
        private static readonly Vector4 OnConfirmation = PWhite;
        private static readonly Vector4 OnAccentBadge = PWhite;
        

        private static readonly Vector4 BorderSurface = FromHex(0x3A3A3A);
        private static readonly Vector4 BorderAltSurface = FromHex(0x444444);
        private static readonly Vector4 BorderControl = FromHex(0x555555);

        private static readonly Vector4 ShadowColor = BlackCol with { W = 0.6f };

        public static Theme GetTheme()
        {
            return new Theme
            {
                AccentBadge = AccentBadge,
                Active = Active,
                AltSurfaceBackground = AltSurfaceBackground,
                AppAccent = AppAccent,
                Background = Background,
                Black = Black,
                BlackCol = BlackCol,
                Blue = Blue,
                BorderAltSurface = BorderAltSurface,
                BorderControl = BorderControl,
                BorderSurface = BorderSurface,
                Confirmation = Confirmation,
                ControlBackground = ControlBackground,
                DBlue = Blue,
                DGrey = DGrey,
                Error = Error,
                Green = Green,
                Inactive = Inactive,
                LGrey = LGrey,
                MBlue = Blue,
                MGrey = MGrey,
                OnAccentBadge = OnAccentBadge,
                OnActive = OnActive,
                OnAltBackground = OnAltBackground,
                OnBackground = OnBackground,
                OnConfirmation = OnConfirmation,
                OnError = OnError,
                OnPrimary = OnPrimary,
                OnSurface = OnSurface,
                Orange = Orange,
                Primary = Primary,
                Purple = Purple,
                PWhite = PWhite,
                Red = Red,
                ShadowColor = ShadowColor,
                SurfaceBackground = SurfaceBackground,
                Text = Text,
                TextSecondary = TextSecondary,
                TextualBackground = TextualBackground,
                White = White,
                XLGrey = XLGrey,
                Yellow = Yellow
            };
        }
    }
}