using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Questionnaire
{
    class Program
    {
        private static List<QuestionTemplate> questionTemplates = new List<QuestionTemplate>();
        private static List<string> generatedQuestions = new List<string>();

        static void Main()
        {
            while (true)
            {
                // Отображение основного меню
                Console.WriteLine("=== Генератор вопросов ===");
                Console.WriteLine("1. Добавить новый шаблон вопроса");
                Console.WriteLine("2. Сгенерировать вопросы");
                Console.WriteLine("3. Сохранить вопросы в файл");
                Console.WriteLine("4. Загрузить вопросы из файла");
                Console.WriteLine("5. Выйти");

                // Ввод выбора пользователя
                Console.Write("Ваш выбор: ");
                int choice;
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddQuestionTemplate();
                            break;
                        case 2:
                            GenerateQuestions();
                            break;
                        case 3:
                            SaveQuestionsToFile();
                            break;
                        case 4:
                            LoadQuestionsFromFile();
                            break;
                        case 5:
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Неверный выбор. Пожалуйста, выберите существующую опцию.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите существующую опцию.");
                }

                // Пауза перед повторным отображением меню
                Console.WriteLine("Нажмите Enter, чтобы продолжить...");
                Console.ReadLine();
            }
        }

        //Метод для добавления шаблонов
        static void AddQuestionTemplate()
        {
            Console.Write("Введите категорию вопроса: ");
            string category = Console.ReadLine();

            Console.Write("Введите шаблон вопроса, используя [ ] для обозначения пропусков: ");
            string questionText = Console.ReadLine();

            List<string> blanks = new List<string>();
            int startIndex = -1;

            // Извлекаем пропуски из текста шаблона и сохраняем их в список blanks
            for (int i = 0; i < questionText.Length; i++)
            {
                if (questionText[i] == '[')
                {
                    startIndex = i;
                }
                else if (questionText[i] == ']' && startIndex != -1)
                {
                    string blank = questionText.Substring(startIndex + 1, i - startIndex - 1).Trim();
                    if (!string.IsNullOrEmpty(blank))
                        blanks.Add(blank);
                    startIndex = -1;
                }
            }

            // Создаем объект QuestionTemplate и добавляем его в список questionTemplates
            QuestionTemplate template = new QuestionTemplate
            {
                Category = category,
                QuestionText = questionText,
                Blanks = blanks
            };

            questionTemplates.Add(template);
            Console.WriteLine("Шаблон добавлен!");
        }

        //Метод для генерации вопросов
        static void GenerateQuestions()
        {
            if (questionTemplates.Count == 0)
            {
                Console.WriteLine("Шаблоны вопросов отсутствуют. Пожалуйста, добавьте шаблоны.");
                return;
            }

            // Вывод категорий для выбора
            Console.WriteLine("Выберите категорию:");

            var categories = questionTemplates.Select(t => t.Category).Distinct().ToList();
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            // Ввод выбранной категории и количества вопросов
            Console.Write("Ваш выбор: ");
            int categoryChoice;
            if (int.TryParse(Console.ReadLine(), out categoryChoice) && categoryChoice >= 1 && categoryChoice <= categories.Count)
            {
                string selectedCategory = categories[categoryChoice - 1];

                Console.Write("Сколько вопросов вы хотите сгенерировать? ");
                int numQuestions;
                if (int.TryParse(Console.ReadLine(), out numQuestions) && numQuestions > 0)
                {
                    // Фильтрация шаблонов по выбранной категории
                    var templatesInCategory = questionTemplates.Where(t => t.Category == selectedCategory).ToList();
                    if (templatesInCategory.Count == 0)
                    {
                        Console.WriteLine("Шаблоны в указанной категории отсутствуют.");
                        return;
                    }

                    Random random = new Random();

                    // Генерация вопросов на основе выбранной категории и количества
                    for (int i = 0; i < numQuestions; i++)
                    {
                        QuestionTemplate template = templatesInCategory[random.Next(templatesInCategory.Count)];
                        List<string> filledBlanks = new List<string>();

                        Console.WriteLine($"--- Вопрос {i + 1} ---");
                        Console.WriteLine(template.QuestionText);

                        // Заполнение пропусков значениями, введенными пользователем
                        foreach (var blank in template.Blanks)
                        {
                            Console.Write($"Введите значение для {blank}: ");
                            string filledBlank = Console.ReadLine();
                            filledBlanks.Add(filledBlank);
                        }

                        // Замена пропусков в шаблоне с введенными значениями
                        string generatedQuestion = template.QuestionText;
                        for (int j = 0; j < filledBlanks.Count; j++)
                        {
                            generatedQuestion = generatedQuestion.Replace("[" + template.Blanks[j] + "]", filledBlanks[j]);
                        }

                        generatedQuestions.Add(generatedQuestion);
                    }

                    // Вывод сгенерированных вопросов
                    Console.WriteLine("\nВаши вопросы:");
                    for (int i = 0; i < generatedQuestions.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {generatedQuestions[i]}");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный ввод количества вопросов.");
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор категории.");
            }
        }

        // Метод для сохранения вопросов в файл
        static void SaveQuestionsToFile()
        {
            if (questionTemplates.Count == 0 || generatedQuestions.Count == 0)
            {
                Console.WriteLine("Нет сгенерированных вопросов для сохранения.");
                return;
            }

            // Ввод имени файла для сохранения
            Console.Write("Введите имя файла для сохранения: ");
            string fileName = Console.ReadLine();

            try
            {
                // Запись сгенерированных вопросов в файл
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.WriteLine("\nВаши вопросы:");
                    for (int i = 0; i < generatedQuestions.Count; i++)
                    {
                        writer.WriteLine($"{i + 1}. {generatedQuestions[i]}");
                    }
                }

                Console.WriteLine("Вопросы успешно сохранены в файле.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении вопросов: {ex.Message}");
            }
        }

        // Метод для загрузки вопросов из файла
        static void LoadQuestionsFromFile()
        {
            // Ввод имени файла для загрузки
            Console.Write("Введите имя файла для загрузки: ");
            string fileName = Console.ReadLine();

            try
            {
                // Чтение строк из файла
                List<string> lines = File.ReadAllLines(fileName).ToList();

                // Создаем новые объекты QuestionTemplate на основе строк из файла
                foreach (var line in lines)
                {
                    QuestionTemplate template = new QuestionTemplate
                    {
                        QuestionText = line
                    };

                    questionTemplates.Add(template);
                }

                Console.WriteLine("Вопросы успешно загружены из файла.");

                // Вывод загруженных вопросов
                Console.WriteLine("Загруженные вопросы:");
                foreach (var template in questionTemplates)
                {
                    Console.WriteLine(template.QuestionText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке вопросов: {ex.Message}");
            }
        }
    }
}