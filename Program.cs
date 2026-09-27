using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using AP_World_Project;

public class Program {
    private static Dictionary<Topic, string>? _infoCorr;
    private static GameUI? _ui;

    public Program() {
        _infoCorr = new Dictionary<Topic, string>();
        _ui = new GameUI();
    }

    public static void Main() {
        var program = new Program();
        program.Populate(true);

        // Main menu loop
        while (true) {
            var choice = _ui?.DisplayMainMenu();
            
            switch (choice) {
                case 1:
                    program.Quiz();
                    break;
                case 2:
                    program.Flashcards();
                    break;
                case 3:
                    _ui?.DisplayExitMessage();
                    return;
                default:
                    _ui?.DisplayInvalidInput();
                    break;
            }
        }
    }
    
    /// <summary>
    /// Populates the dictionary from TopicMappings.csv file.
    /// Reads line-by-line and splits each line into exactly 3 parts:
    /// topic name, category letter, and definition.
    /// </summary>
    private void Populate(bool check) {
        try {
            var sr = new StreamReader("TopicMappings.csv");
            var line = sr.ReadLine();
        
            while (line != null) {
                var tempInfo = line.Split(new char[] {','}, 3);
            
                if (tempInfo.Length == 3) {
                    var topic = tempInfo[0].Trim();
                    var category = tempInfo[1].Trim();
                    var definition = tempInfo[2].Trim();
                
                    _infoCorr?.Add(new Topic(topic, category), definition);
                }
            
                line = sr.ReadLine();
            }
        
            sr.Close();
            _ui?.DisplayLoadSuccess(_infoCorr?.Count ?? 0);
        } catch (Exception e) {
            _ui?.DisplayLoadError(e.Message);
        }
    }

    /// <summary>
    /// Quiz mode: user is presented with a term and must select the correct definition
    /// from 4 multiple choice options (A, B, C, D).
    /// Scoring: 100 points per correct answer, multiplied by current streak.
    /// Tracks score and streak throughout the session.
    /// </summary>
    public void Quiz() {
        _ui?.DisplayQuizStart();

        var totalScore = 0;
        var streak = 0;
        const int pointsPerQuestion = 100;

        var topicArray = _infoCorr?.Keys.ToArray();
        
        if (topicArray == null || topicArray.Length == 0) {
            _ui?.DisplayNoTopicsError();
            return;
        }

        while (true) {
            var randomIndex = Random.Shared.Next(topicArray.Length);
            var topic = topicArray[randomIndex];
            var correctAnswer = _infoCorr?[topic];

            var allAnswers = _infoCorr?.Values.Distinct().ToList();

            if (allAnswers == null || allAnswers.Count < 4) {
                _ui?.DisplayNotEnoughAnswersError();
                break;
            }

            // Build answer set: start with correct answer, then add 3 random wrong answers
            var answerSet = new List<string> { correctAnswer };
            var wrongAnswers = allAnswers.Where(a => a != correctAnswer).ToList();

            while (answerSet.Count < 4) {
                var randomWrongIndex = Random.Shared.Next(wrongAnswers.Count);
                var candidate = wrongAnswers[randomWrongIndex];

                if (!answerSet.Contains(candidate)) {
                    answerSet.Add(candidate);
                }
            }

            // Shuffle the answer set
            answerSet = answerSet.OrderBy(_ => Random.Shared.Next()).ToList();

            // Display the question
            var userInput = _ui?.DisplayQuizQuestion(topic.GetTopic(), answerSet, totalScore, streak);

            // Check if user wants to quit
            if (userInput == "QUIT") {
                _ui?.DisplayQuizEnd(totalScore);
                break;
            }

            // Validate and check answer
            if (userInput != "A" && userInput != "B" && userInput != "C" && userInput != "D") {
                _ui?.DisplayInvalidInput();
                continue;
            }

            // Map letter to index (A=0, B=1, C=2, D=3)
            int answerIndex = userInput[0] - 'A';
            var userAnswer = answerSet[answerIndex];

            // Check if the answer is correct
            if (userAnswer == correctAnswer) {
                streak++;
                int pointsEarned = pointsPerQuestion * streak;
                totalScore += pointsEarned;
                _ui?.DisplayCorrectAnswer(pointsEarned, streak);
            } else {
                _ui?.DisplayIncorrectAnswer(correctAnswer);
                streak = 0;
            }

            System.Threading.Thread.Sleep(2000);
        }
    }

    /// <summary>
    /// Flashcards mode: displays a term, user can choose to reveal the definition.
    /// Allows user to mark cards as "known" to track progress.
    /// </summary>
    public void Flashcards() {
        _ui?.DisplayFlashcardsStart();

        var topicArray = _infoCorr?.Keys.ToArray();
        
        if (topicArray == null || topicArray.Length == 0) {
            _ui?.DisplayNoTopicsError();
            return;
        }

        var knownCards = new HashSet<int>();
        var currentCardIndex = 0;
        var isFlipped = false;

        while (true) {
            int unknownCount = topicArray.Length - knownCards.Count;

            if (unknownCount == 0) {
                _ui?.DisplayFlashcardsComplete(topicArray.Length);
                break;
            }

            var topic = topicArray[currentCardIndex];
            var definition = _infoCorr?[topic];

            // Display card and get user input
            var key = _ui?.DisplayFlashcard(
                topic.GetTopic(),
                definition,
                isFlipped,
                knownCards.Count,
                topicArray.Length
            );

            switch (key?.ToUpper()) {
                case " ":
                    // Flip the card
                    isFlipped = !isFlipped;
                    break;

                case "N":
                    // Move to next unknown card
                    isFlipped = false;
                    do {
                        currentCardIndex = (currentCardIndex + 1) % topicArray.Length;
                    } while (knownCards.Contains(currentCardIndex) && knownCards.Count < topicArray.Length);
                    break;

                case "K":
                    // Mark card as known
                    knownCards.Add(currentCardIndex);
                    isFlipped = false;
                    
                    // Auto-advance to next unknown card
                    do {
                        currentCardIndex = (currentCardIndex + 1) % topicArray.Length;
                    } while (knownCards.Contains(currentCardIndex) && knownCards.Count < topicArray.Length);
                    break;

                case "Q":
                    _ui?.DisplayFlashcardsExit(knownCards.Count, topicArray.Length);
                    return;
            }
        }
    }
}

/// <summary>
/// GameUI handles all console output and menu displays.
/// Centralizes UI logic away from the main program logic.
/// </summary>
public class GameUI {
    private const string BorderChar = "╔════════════════════════════════════╗";
    private const string BottomBorder = "╚════════════════════════════════════╝";
    private const string SideBorder = "║";

    /// <summary>
    /// Displays the main menu and returns the user's choice (1, 2, or 3)
    /// </summary>
    public int DisplayMainMenu() {
        Console.Clear();
        PrintHeader("Welcome to WorldLet! 🌍", "AP World History Study Tool");
        
        Console.WriteLine("What would you like to do?\n");
        Console.WriteLine("  [1] Quiz");
        Console.WriteLine("  [2] Flashcards");
        Console.WriteLine("  [3] Exit\n");
        Console.Write("Enter your choice (1-3): ");

        if (int.TryParse(Console.ReadLine(), out var choice) && choice >= 1 && choice <= 3) {
            return choice;
        }

        return -1;
    }

    /// <summary>
    /// Displays the quiz start screen with instructions
    /// </summary>
    public void DisplayQuizStart() {
        Console.Clear();
        PrintHeader("Quiz Mode 📝");
        Console.WriteLine("You will be given different terms, and you must pick the correct definition");
        Console.WriteLine("from four multiple choice options (A, B, C, D).\n");
        Console.WriteLine("Scoring: 100 points per correct answer × your current streak!\n");
        Console.WriteLine("Type 'quit' at any time to return to the main menu.\n");
        Console.WriteLine("Press any key to begin...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays a quiz question with 4 answer options
    /// Returns the user's answer (A, B, C, D, or QUIT)
    /// </summary>
    public string? DisplayQuizQuestion(string term, List<string> answers, int totalScore, int streak) {
        Console.Clear();
        Console.WriteLine($"Score: {totalScore}  |  Streak: {streak}\n");
        
        Console.WriteLine($"What is: {term}\n");
        Console.WriteLine("Select the correct definition:\n");
        Console.WriteLine($"A: {answers[0]}\n");
        Console.WriteLine($"B: {answers[1]}\n");
        Console.WriteLine($"C: {answers[2]}\n");
        Console.WriteLine($"D: {answers[3]}\n");
        Console.Write("Your answer (A/B/C/D) or 'quit': ");

        return Console.ReadLine()?.ToUpper();
    }

    /// <summary>
    /// Displays when the user answered correctly
    /// </summary>
    public void DisplayCorrectAnswer(int pointsEarned, int streak) {
        Console.WriteLine($"\n✓ Correct! You earned {pointsEarned} points! (100 × {streak} streak)");
    }

    /// <summary>
    /// Displays when the user answered incorrectly
    /// </summary>
    public void DisplayIncorrectAnswer(string correctAnswer) {
        Console.WriteLine($"\n✗ Incorrect. The correct answer was: {correctAnswer}");
        Console.WriteLine("Streak reset to 0.");
    }

    /// <summary>
    /// Displays when the user quits the quiz
    /// </summary>
    public void DisplayQuizEnd(int finalScore) {
        Console.Clear();
        PrintHeader("Quiz Complete! 🎉");
        Console.WriteLine($"Final Score: {finalScore}\n");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays the flashcards start screen
    /// </summary>
    public void DisplayFlashcardsStart() {
        Console.Clear();
        PrintHeader("Flashcards Mode 🃏");
        Console.WriteLine("Review your flashcards and mark the ones you've mastered.\n");
        Console.WriteLine("Controls:");
        Console.WriteLine("  [SPACE] - Flip card");
        Console.WriteLine("  [N]     - Next card");
        Console.WriteLine("  [K]     - Mark as known");
        Console.WriteLine("  [Q]     - Quit flashcards\n");
        Console.WriteLine("Press any key to begin...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays a single flashcard with term or definition
    /// Returns the key pressed by the user
    /// </summary>
    public string? DisplayFlashcard(string term, string definition, bool isFlipped, int knownCount, int totalCards) {
        Console.Clear();
        Console.WriteLine($"Progress: {knownCount}/{totalCards} known  |  {totalCards - knownCount} remaining\n");

        // Display card box
        Console.WriteLine("┌──────────────────────────────────┐");
        if (isFlipped) {
            Console.WriteLine("│ [DEFINITION]                     │");
            Console.WriteLine("├──────────────────────────────────┤");
            Console.WriteLine($"│ {definition}");
        } else {
            Console.WriteLine("│ [TERM]                           │");
            Console.WriteLine("├──────────────────────────────────┤");
            Console.WriteLine($"│ {term}");
        }
        Console.WriteLine("└──────────────────────────────────┘");

        Console.WriteLine("\nOptions:");
        Console.WriteLine("  [SPACE] - Flip card");
        Console.WriteLine("  [N]     - Next card");
        Console.WriteLine("  [K]     - Mark as known");
        Console.WriteLine("  [Q]     - Quit");
        Console.Write("\nPress a key: ");

        return Console.ReadKey(true).KeyChar.ToString();
    }

    /// <summary>
    /// Displays when all flashcards are mastered
    /// </summary>
    public void DisplayFlashcardsComplete(int totalCards) {
        Console.Clear();
        PrintHeader("Flashcards Complete! 🎉");
        Console.WriteLine($"You've mastered all {totalCards} flashcards!\n");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays when the user exits flashcards
    /// </summary>
    public void DisplayFlashcardsExit(int knownCount, int totalCards) {
        Console.Clear();
        PrintHeader("Flashcards Session End");
        Console.WriteLine($"You mastered {knownCount}/{totalCards} flashcards!\n");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays an error when no topics are loaded
    /// </summary>
    public void DisplayNoTopicsError() {
        Console.Clear();
        PrintHeader("Error ⚠️");
        Console.WriteLine("No topics loaded. Please check your TopicMappings.csv file.\n");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays an error when there aren't enough unique answers
    /// </summary>
    public void DisplayNotEnoughAnswersError() {
        Console.Clear();
        PrintHeader("Error ⚠️");
        Console.WriteLine("Not enough unique answers in the database to create a quiz.\n");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }

    /// <summary>
    /// Displays a generic invalid input message
    /// </summary>
    public void DisplayInvalidInput() {
        Console.WriteLine("Invalid input. Please try again.");
        System.Threading.Thread.Sleep(1000);
    }

    /// <summary>
    /// Displays a success message when topics are loaded
    /// </summary>
    public void DisplayLoadSuccess(int topicCount) {
        Console.WriteLine($"✓ Loaded {topicCount} topics successfully!\n");
        System.Threading.Thread.Sleep(1500);
    }

    /// <summary>
    /// Displays an error message when loading topics fails
    /// </summary>
    public void DisplayLoadError(string errorMessage) {
        Console.WriteLine($"✗ Error loading file: {errorMessage}");
        Console.WriteLine("Make sure TopicMappings.csv is in the same directory as the program.\n");
    }

    /// <summary>
    /// Displays the exit message
    /// </summary>
    public void DisplayExitMessage() {
        Console.Clear();
        PrintHeader("Thanks for studying!");
        Console.WriteLine("Keep learning! 📚\n");
        System.Threading.Thread.Sleep(1500);
    }

    /// <summary>
    /// Helper method to print a nice header with title and optional subtitle
    /// </summary>
    private void PrintHeader(string title, string? subtitle = null) {
        Console.WriteLine(BorderChar);
        Console.WriteLine($"{SideBorder} {CenterText(title, 34)} {SideBorder}");
        if (subtitle != null) {
            Console.WriteLine($"{SideBorder} {CenterText(subtitle, 34)} {SideBorder}");
        }
        Console.WriteLine(BottomBorder);
        Console.WriteLine();
    }

    /// <summary>
    /// Helper method to center text within a fixed width
    /// </summary>
    private string CenterText(string text, int width) {
        int padding = (width - text.Length) / 2;
        return text.PadLeft(text.Length + padding).PadRight(width);
    }
}
