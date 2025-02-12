using System.Windows;
using System.Windows.Input;

namespace POS.Views.Base
{
    public class WindowBase : Window
    {
        protected void DragWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
