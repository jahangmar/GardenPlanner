using Gtk;
namespace GardenPlanner
{
    public class DateInputWindow : NumberInputWindow
    {
        Label label2;
        SpinButton spinButtonMonth;

        private DateInputWindow(string title, string message_year, string message_month) : base(title, message_year, 2000, 2100)
        {
            label2 = new Label(message_month);
            spinButtonMonth = new SpinButton(1, 12, 1);
            vBox.Remove(hButtonBox);
            vBox.Add(label2);
            vBox.Add(spinButtonMonth);
            vBox.Add(hButtonBox);
        }

        public static void ShowWindow(string title, System.Action<int, int> action)
        {
            DateInputWindow dateInputWindow = new DateInputWindow(title, "Set the year", "Set the month");
            dateInputWindow.okButton.Clicked += (object sender, System.EventArgs e) =>
            {
                action(dateInputWindow.spinButton.ValueAsInt, dateInputWindow.spinButtonMonth.ValueAsInt);
                dateInputWindow.Destroy();
            };
            dateInputWindow.cancelButton.Clicked += (object sender, System.EventArgs e) =>
            {
                dateInputWindow.Destroy();
            };
            dateInputWindow.ShowAll();
        }
    }
}
