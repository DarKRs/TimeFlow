using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeFlow.Presentation.CustomControl
{
    public class TimeEntry : Entry
    {
        public TimeEntry()
        {
            Keyboard = Keyboard.Numeric;
            TextChanged += OnTextChanged;
            Completed += OnCompleted;
            Unfocused += OnUnfocused;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            var entry = (Entry)sender;
            string input = e.NewTextValue;

            input = FormatInput(input, e.OldTextValue);

            entry.Text = input;
            entry.CursorPosition = entry.Text.Length;
        }

        private string FormatInput(string input, string previousInput)
        {
            // Ограничение длины текста (чч:мм)
            if (input.Length > 5)
                input = input.Substring(0, 5);

            // Проверка на допустимые символы
            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{0,2}:?\d{0,2}$"))
                return previousInput;

            // Автоматическое добавление двоеточия и корректировка часов и минут
            if (input.Length == 1 && int.TryParse(input, out int firstDigit) && firstDigit > 2)
                return $"{firstDigit}:";

            if (input.Length == 2 && !input.Contains(":"))
                input += ":";

            string[] parts = input.Split(':');
            if (parts.Length == 2)
            {
                input = CorrectHours(parts);
                input = CorrectMinutes(parts, input);
            }

            return input;
        }

        private string CorrectHours(string[] parts)
        {
            if (int.TryParse(parts[0], out int hours) && hours > 23)
                return "23:" + (parts.Length > 1 ? parts[1] : "00");
            return string.Join(":", parts);
        }

        private string CorrectMinutes(string[] parts, string input)
        {
            if (parts.Length < 2)
                return input;

            if (parts[1].Length == 1 && int.TryParse(parts[1], out int firstMinuteDigit) && firstMinuteDigit > 5)
                return parts[0] + ":59";

            if (int.TryParse(parts[1], out int minutes) && minutes > 59)
                return parts[0] + ":59";

            return input;
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            ValidateTime(entry);
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            var entry = (Entry)sender;
            ValidateTime(entry);
        }

        private void ValidateTime(Entry entry)
        {
            if (TimeSpan.TryParseExact(entry.Text, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan time))
            {
                if (time.TotalHours >= 0 && time.TotalHours < 24)
                {
                    entry.Text = time.ToString("hh\\:mm"); // Форматируем корректное время
                }
                else
                {
                    SetInvalidState(entry);
                }
            }
            else
            {
                SetInvalidState(entry);
            }
        }

        private void SetInvalidState(Entry entry)
        {
            entry.TextColor = Color.FromRgb(255, 0, 0);
            DisplayAlert("Ошибка", "Введите корректное время в формате чч:мм", "ОК");
            entry.Text = string.Empty;
        }

        private async void DisplayAlert(string title, string message, string cancel)
        {
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }
        }
    }

}
