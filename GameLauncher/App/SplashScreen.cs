using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using System.Windows.Forms;


namespace GameLauncher.App.Classes
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();

            BackColor = Theming.SplashScreenTransparencyKey;
            TransparencyKey = Theming.SplashScreenTransparencyKey;
            BackgroundImage = Theming.LogoSplash;
        }

        private void SplashScreen_Load(object sender, System.EventArgs e)
        {
            Clock.Start();
            FunctionStatus.CenterScreen(this);
        }

        private void Clock_Tick(object sender, System.EventArgs e)
        {
            if (FunctionStatus.ServerListStatus == "Loaded")
            {
                Application.OpenForms["SplashScreen"].Close();
            }
        }
    }
}
