using Gtk;
namespace GardenPlanner
{
    public class NumberInputWindow : Window
    {

        SpinButton spinButton;
        Label label;
        Button okButton;
        Button cancelButton;

        private NumberInputWindow(string title, string message, int min, int max) : base(WindowType.Toplevel)
        {
            Modal = true;
            Title = title;

            label = new Label(message);
            spinButton = new SpinButton(min, max, 1);
            okButton = new Button("Ok");
            cancelButton = new Button("Cancel");

            VBox vBox = new VBox();

            vBox.Add(label);
            vBox.Add(spinButton);

            HButtonBox hButtonBox = new HButtonBox();

            hButtonBox.Add(cancelButton);
            hButtonBox.Add(okButton);

            vBox.Add(hButtonBox);
            Add(vBox);

        }

        public static void ShowWindow(string title, string message, int min, int max, System.Func<int, bool> action)
        {
            NumberInputWindow numberInputWindow = new NumberInputWindow(title, message, min, max);
            numberInputWindow.okButton.Clicked += (object sender, System.EventArgs e) =>
            {
                action(numberInputWindow.spinButton.ValueAsInt);
                numberInputWindow.Destroy();
            };
            numberInputWindow.cancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                numberInputWindow.Destroy();
            };
            numberInputWindow.ShowAll();
        }
    }
}
