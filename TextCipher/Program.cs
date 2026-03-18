using System;
using System.Text;
using System.IO;

class Program
{
    static void Main()
    {
        // Настраиваю кодировку, чтобы корректно отображались символы
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.Clear();

            // Основное меню программы
            Console.WriteLine("TextCipher");
            Console.WriteLine("1. Шифрование");
            Console.WriteLine("2. Дешифрование");
            Console.WriteLine("0. Выход");

            // Считываю выбор пользователя с проверкой
            int mode = ReadChoice(0, 2);
            if (mode == 0) break;

            // Получаю текст (либо из консоли, либо из файла)
            string text = GetText();

            // Пользователь выбирает алгоритм шифрования
            ICipher cipher = SelectCipher();

            // В зависимости от режима выполняю шифрование или дешифрование
            string result = mode == 1
                ? cipher.Encrypt(text)
                : cipher.Decrypt(text);

            // Вывожу результат
            Console.WriteLine("\nРезультат:");
            Console.WriteLine(result);

            // Предлагаю сохранить результат в файл
            SaveToFileOption(result);

            Console.WriteLine("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }

    // Тут я реализовал выбор источника текста: ввод вручную или чтение из файла
    static string GetText()
    {
        Console.WriteLine("\nИсточник данных:");
        Console.WriteLine("1. Ввести текст");
        Console.WriteLine("2. Загрузить из файла");

        int choice = ReadChoice(1, 2);

        // Ввод текста вручную
        if (choice == 1)
        {
            Console.WriteLine("Введите текст:");

            string? input = Console.ReadLine();

            // Проверяю, чтобы текст не был пустым
            while (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Текст не может быть пустым. Повторите:");
                input = Console.ReadLine();
            }

            return input;
        }
        else
        {
            // Тут я реализовал чтение текста из файла с обработкой ошибок
            while (true)
            {
                try
                {
                    Console.WriteLine("Введите путь к файлу:");
                    string? path = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(path))
                        throw new Exception();

                    // Читаю весь текст из файла
                    return File.ReadAllText(path);
                }
                catch
                {
                    Console.WriteLine("Ошибка чтения файла. Повторите:");
                }
            }
        }
    }

    // Тут я реализовал выбор алгоритма шифрования
    static ICipher SelectCipher()
    {
        Console.WriteLine("\nМетод:");
        Console.WriteLine("1. Цезарь");
        Console.WriteLine("2. Атбаш");
        Console.WriteLine("3. XOR");

        int choice = ReadChoice(1, 3);

        return choice switch
        {
            1 => CreateCaesar(),
            2 => new AtbashCipher(),
            3 => CreateXor(),
            _ => throw new Exception("Ошибка выбора")
        };
    }

    // Создание шифра Цезаря с вводом сдвига
    static ICipher CreateCaesar()
    {
        Console.WriteLine("Введите сдвиг:");
        int shift = int.Parse(Console.ReadLine()!);
        return new CaesarCipher(shift);
    }

    // Создание XOR-шифра с вводом ключа
    static ICipher CreateXor()
    {
        Console.WriteLine("Введите символ ключа:");
        char key = Console.ReadKey().KeyChar;
        Console.WriteLine();
        return new XorCipher(key);
    }

    // Тут я реализовал сохранение результата в файл по желанию пользователя
    static void SaveToFileOption(string content)
    {
        Console.WriteLine("\nСохранить результат в файл?");
        Console.WriteLine("1. Да");
        Console.WriteLine("2. Нет");

        int choice = ReadChoice(1, 2);

        if (choice == 1)
        {
            try
            {
                Console.WriteLine("Введите путь для сохранения:");
                string? path = Console.ReadLine();

                // Записываю результат в файл
                File.WriteAllText(path!, content);
                Console.WriteLine("Файл успешно сохранён.");
            }
            catch
            {
                Console.WriteLine("Ошибка при сохранении файла.");
            }
        }
    }

    // Универсальный метод для безопасного ввода чисел
    static int ReadChoice(int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int val)
                && val >= min && val <= max)
                return val;

            Console.WriteLine("Ошибка. Повторите:");
        }
    }
}

// Интерфейс для всех алгоритмов шифрования
interface ICipher
{
    string Encrypt(string text);
    string Decrypt(string text);
}

// Тут я реализовал шифр Цезаря (сдвиг символов)
class CaesarCipher : ICipher
{
    private readonly int _shift;

    public CaesarCipher(int shift)
    {
        _shift = shift;
    }

    public string Encrypt(string text) => Transform(text, _shift);
    public string Decrypt(string text) => Transform(text, -_shift);

    // Основная логика сдвига символов
    private string Transform(string text, int shift)
    {
        var result = new StringBuilder();

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                // Определяю, верхний или нижний регистр
                char offset = char.IsUpper(c) ? 'A' : 'a';

                // Выполняю циклический сдвиг
                char newChar = (char)((c - offset + shift + 26) % 26 + offset);
                result.Append(newChar);
            }
            else
            {
                // Не-буквы не изменяются
                result.Append(c);
            }
        }
        return result.ToString();
    }
}

// Тут я реализовал шифр Атбаш (зеркальный алфавит)
class AtbashCipher : ICipher
{
    public string Encrypt(string text) => Transform(text);
    public string Decrypt(string text) => Transform(text);

    private string Transform(string text)
    {
        var result = new StringBuilder();

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                char offset = char.IsUpper(c) ? 'A' : 'a';

                // Зеркально отражаю символ относительно алфавита
                char newChar = (char)(offset + (25 - (c - offset)));
                result.Append(newChar);
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}

// Тут я реализовал XOR-шифрование
class XorCipher : ICipher
{
    private readonly char _key;

    public XorCipher(char key)
    {
        _key = key;
    }

    public string Encrypt(string text) => Transform(text);
    public string Decrypt(string text) => Transform(text);

    private string Transform(string text)
    {
        var result = new StringBuilder();

        foreach (char c in text)
        {
            // Применяю побитовую операцию XOR
            result.Append((char)(c ^ _key));
        }

        return result.ToString();
    }
}