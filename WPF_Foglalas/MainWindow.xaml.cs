using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Foglalas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool IsNameValid(string name, out string hibaUzenet)
        {
            hibaUzenet = "";

            if (name.Length < 3)
            {
                hibaUzenet = "A név minimum hossza 3 karakter!";
                return false;
            }

            return true;
        }

        private bool IsNumberValid(string numberText, int min, int max, out string hibaUzenet)
        {
            hibaUzenet = "";

            int number;

            try
            {
                number = Convert.ToInt32(numberText);
            }
            catch (FormatException)
            {
                hibaUzenet = "A szám formátuma hibás!";
                return false;
            }
            catch (OverflowException)
            {
                hibaUzenet = "A szám meghaladta az elfogadott határokat!";
                return false;
            }

            if (number < min)
            {
                hibaUzenet = $"A szám kisebb mint az elfogadott alsó határ, {min}.";
                return false;
            }

            if (number > max)
            {
                hibaUzenet = $"A szám nagyobb mint az elfogadott felső határ, {max}.";
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Hiba történt.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Foglalás", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ShowConfirm(string message)
        {
            return MessageBoxResult.OK == MessageBox.Show(message, "Foglalás", MessageBoxButton.OKCancel, MessageBoxImage.Question);
        }

        private int CountChecked(bool? isChecked, int value)
        {
            if (isChecked ?? false)
            {
                return value;
            }

            return 0;
        }

        private string ChooseChecked(string[] items, bool?[] conditions)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] ?? false)
                {
                    return items[i];
                }
            }

            return "-";
        }

        private List<string> CollectChecked(string[] items, bool?[] conditions)
        {
            List<string> collected = new();

            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i] ?? false)
                {
                    collected.Add(items[i]);
                }
            }

            return collected;
        }


        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            string hibaUzenet;

            // Név
            if (!IsNameValid(userName.Text, out hibaUzenet))
            {
                userName.Focus();
                ShowError(hibaUzenet);
                return;
            }

            // Életkor
            if (!IsNumberValid(userAge.Text, 1, 120, out hibaUzenet))
            {
                userAge.Focus();
                ShowError(hibaUzenet);
                return;
            }

            // Film választás
            if (movieChoice.SelectedIndex == 0)
            {
                movieChoice.Focus();
                ShowError("Egy film kiválasztása kötelező.");
                return;
            }

            // Jegyek száma
            if (!IsNumberValid(ticketCount.Text, 1, 10, out hibaUzenet))
            {
                ticketCount.Focus();
                ShowError(hibaUzenet);
                return;
            }

            // Vásárlási feltételek
            if (!acceptConditions.IsChecked ?? false)
            {
                acceptConditions.Focus();
                ShowError("A vásárlási feltételek elfogadása kötelező!");
                return;
            }

            string[] movieNames = { "-", "Backrooms: Hátsó szóbák", "Öt Éjjel Freddy Pizzázójában", "Star Wars I. rész – Baljós árnyak", "Harry Potter és a bölcsek köve" };

            string[] ticketNames = { "Normál", "Diák", "VIP" };
            bool?[] ticketConditions = { ticketNormal.IsChecked, ticketStudent.IsChecked, ticketVIP.IsChecked };

            string[] extraNames = { "Popcorn", "Üdítő", "3D Szemüveg" };
            bool?[] extraConditions = { extraPopcorn.IsChecked, extraDrink.IsChecked, extra3D.IsChecked };

            List<string> extras = CollectChecked(extraNames, extraConditions);
            string extraString = extras.Count == 0 ? "Nincs." : string.Join(", ", extras);

            int ticketNumber = Convert.ToInt32(ticketCount.Text);
            int finalPrice = 0 +
                // Jegy árak
                (
                    CountChecked(ticketNormal.IsChecked, 2500) +
                    CountChecked(ticketStudent.IsChecked, 1900) +
                    CountChecked(ticketVIP.IsChecked, 4000)
                ) * ticketNumber +
                // Extrák
                CountChecked(extraPopcorn.IsChecked, 1200) +
                CountChecked(extraDrink.IsChecked, 800) +
                CountChecked(extra3D.IsChecked, 500);

            if (
                ShowConfirm($"A bevitt foglalási adatok:\n\n" +
                            $"Foglaló adatai:" +
                            $"  Név: {userName.Text}\n" +
                            $"  Életkor: {userAge.Text}\n\n" +
                            $"Foglalás adatai:\n" +
                            $"  Film: {movieNames[movieChoice.SelectedIndex]}\n" +
                            $"  Jegyek száma: {ticketCount.Text}\n" +
                            $"  Jegyek típusa: {ChooseChecked(ticketNames, ticketConditions)}\n" +
                            $"  Extrák: {extraString}\n\n" +
                            $"Fizetendő: {finalPrice} Ft")
            )
            {
                ShowInfo("A foglalása sikeres!");
            }
        }
    }
}